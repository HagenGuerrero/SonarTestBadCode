"""
AI auto-fix loop: reads output/batches.json, calls the GitHub Copilot /
GitHub Models API for each batch, writes corrected files, commits per file.

Batches with more issues than MAX_ISSUES_PER_CALL are split into sequential
sub-batches. Each sub-batch reads the already-modified file so line numbers
stay accurate for the model.

Environment variables:
    WORKSPACE_ROOT        Absolute path to the checked-out repo (default: cwd)
    COPILOT_TOKEN         GitHub PAT with copilot scope (optional)
    GITHUB_TOKEN          Built-in Actions token (GitHub Models fallback)
    MAX_ISSUES_PER_CALL   Issues per AI request — triggers sub-batching (default: 15)
    GITHUB_RUN_ID         Actions run ID written into commit messages

Reads:   output/batches.json
Writes:  source files (in-place), output/fix_results/batch_N.json
Commits: one git commit per file across all sub-batch chunks
Exits:   0  at least one file fixed
         2  all batches skipped (non-fatal)
         3  fatal error (unreadable workspace, batches.json missing)
"""

import json
import os
import re
import subprocess
import sys
import time
from pathlib import Path

sys.path.insert(0, os.path.join(os.path.dirname(__file__), ".."))
from shared.constants import (
    AI_INTER_FILE_DELAY_SEC,
    AI_MODEL,
    AI_RETRY_MAX,
    BATCHES_FILE,
    COPILOT_URL,
    EXIT_CODE_ALL_SKIPPED,
    EXIT_CODE_FATAL,
    EXIT_CODE_SUCCESS,
    FIX_RESULTS_DIR,
    MAX_ISSUES_PER_CALL,
    MODELS_URL,
    VALIDATION_MAX_SIZE_RATIO,
    VALIDATION_MIN_CHARS,
    VALIDATION_MIN_SIZE_RATIO,
)

try:
    import requests
except ImportError:
    print("ERROR: 'requests' not installed — run: pip install requests", file=sys.stderr)
    sys.exit(EXIT_CODE_FATAL)

# ---------------------------------------------------------------------------
# Configuration
# ---------------------------------------------------------------------------

_SCRIPT_DIR = Path(__file__).parent
_PROMPTS_DIR = _SCRIPT_DIR / "prompts"

_COPILOT_TOKEN = os.environ.get("COPILOT_TOKEN", "")
_GITHUB_TOKEN = os.environ.get("GITHUB_TOKEN", "")
_copilot_available = bool(_COPILOT_TOKEN)

# Short-lived session token exchanged from _COPILOT_TOKEN at first use.
# api.githubcopilot.com requires this token, not the raw PAT.
_copilot_session_token: str | None = None
_copilot_session_fetched: bool = False

_WORKSPACE = os.environ.get("WORKSPACE_ROOT", os.getcwd())
_RUN_ID = os.environ.get("GITHUB_RUN_ID", "local")
_MAX_PER_CALL = int(os.environ.get("MAX_ISSUES_PER_CALL", str(MAX_ISSUES_PER_CALL)))

# ---------------------------------------------------------------------------
# Prompt loading
# ---------------------------------------------------------------------------

_SYSTEM_PROMPT: str = ""


def _ensure_prompts_loaded() -> None:
    global _SYSTEM_PROMPT
    if not _SYSTEM_PROMPT:
        try:
            system = (_PROMPTS_DIR / "system_prompt.md").read_text(encoding="utf-8")
            agent = (_PROMPTS_DIR / "agent.md").read_text(encoding="utf-8")
            _SYSTEM_PROMPT = system + "\n\n---\n\n" + agent
        except OSError as e:
            print(f"ERROR: cannot load prompt files from {_PROMPTS_DIR}: {e}", file=sys.stderr)
            sys.exit(EXIT_CODE_FATAL)


# ---------------------------------------------------------------------------
# User prompt builder
# ---------------------------------------------------------------------------

def _build_user_prompt(file_path: str, content: str, issues: list[dict]) -> str:
    lines = [f"File: {file_path}", "", "Issues to fix:"]
    for issue in issues:
        rule = issue.get("rule", "")
        line = issue.get("line", "?")
        severity = issue.get("severity", "")
        message = issue.get("message", "")
        lines.append(f"  Line {line}: [{rule}] {severity} — {message}")
    lines += ["", "File content:", content]
    return "\n".join(lines)


# ---------------------------------------------------------------------------
# API call helpers
# ---------------------------------------------------------------------------

def _get_copilot_session_token() -> str:
    """Exchange the PAT for a short-lived Copilot API session token (~30 min TTL).

    This is the same auth flow VS Code / JetBrains use before every Copilot request.
    Falls back to the raw PAT if the exchange endpoint is unreachable or returns an error.
    """
    global _copilot_session_token, _copilot_session_fetched
    if _copilot_session_fetched:
        return _copilot_session_token or _COPILOT_TOKEN

    _copilot_session_fetched = True
    try:
        resp = requests.get(
            "https://api.github.com/copilot_internal/v2/token",
            headers={
                "Authorization": f"token {_COPILOT_TOKEN}",
                "Accept": "application/json",
                "User-Agent": "GitHubCopilotChat/1.0",
                "Editor-Version": "vscode/1.85.0",
                "Editor-Plugin-Version": "copilot-chat/0.12.0",
            },
            timeout=15,
        )
        if resp.status_code == 200:
            _copilot_session_token = resp.json().get("token")
            print("  Copilot session token acquired (valid ~30 min)")
            return _copilot_session_token or _COPILOT_TOKEN
        else:
            print(f"  Copilot token exchange failed ({resp.status_code}) — using PAT directly")
    except requests.RequestException as e:
        print(f"  Copilot token exchange error: {e} — using PAT directly")

    return _COPILOT_TOKEN


def _copilot_headers() -> dict:
    return {
        "Authorization": f"Bearer {_get_copilot_session_token()}",
        "Content-Type": "application/json",
        "Editor-Version": "vscode/1.85.0",
        "Editor-Plugin-Version": "copilot-chat/0.12.0",
        "Copilot-Integration-Id": "vscode-chat",
    }


def _models_headers() -> dict:
    return {
        "Authorization": f"Bearer {_GITHUB_TOKEN}",
        "Content-Type": "application/json",
    }


def _post(url: str, payload: dict, headers: dict):
    try:
        return requests.post(url, json=payload, headers=headers, timeout=120)
    except requests.RequestException as e:
        print(f"  Network error: {e}")
        return None


def _sleep_for_retry(resp, attempt: int) -> None:
    try:
        wait = int(resp.headers.get("retry-after", 0))
    except (ValueError, AttributeError):
        wait = 0
    wait = max(wait, 2 ** attempt)
    print(f"  Rate limited — waiting {wait}s before retry (attempt {attempt + 1})")
    time.sleep(wait)


def call_ai(content: str, file_path: str, issues: list[dict]) -> tuple[str | None, str | None, dict]:
    """Call the AI API with Copilot → GitHub Models fallback.

    Returns (text, error_reason, usage) where usage is the API's token-count
    dict (prompt_tokens, completion_tokens, total_tokens) or {} on failure.
    """
    global _copilot_available

    # Scale max_tokens to the file size — output is roughly the same length as input.
    # Cap at 16384 (GitHub Models gpt-4o limit). Floor at 4096 for small files.
    estimated_output_tokens = (len(content) // 4) + 1024
    max_tokens = min(max(4096, estimated_output_tokens), 16384)

    payload = {
        "model": AI_MODEL,
        "messages": [
            {"role": "system", "content": _SYSTEM_PROMPT},
            {"role": "user", "content": _build_user_prompt(file_path, content, issues)},
        ],
        "temperature": 0.2,
        "max_tokens": max_tokens,
    }

    last_error: str = "all attempts exhausted"

    for attempt in range(AI_RETRY_MAX + 1):
        if _copilot_available:
            resp = _post(COPILOT_URL, payload, _copilot_headers())
            if resp is None:
                _copilot_available = False
                last_error = "Copilot API unreachable"
            elif resp.status_code in (401, 403):
                print(f"  Copilot auth failed ({resp.status_code}) — switching to GitHub Models")
                _copilot_available = False
            elif resp.status_code == 200:
                data = resp.json()
                return data["choices"][0]["message"]["content"], None, data.get("usage", {})
            elif resp.status_code == 429:
                _sleep_for_retry(resp, attempt)
                continue
            else:
                print(f"  Copilot error {resp.status_code} — switching to GitHub Models")
                _copilot_available = False

        # GitHub Models fallback
        resp = _post(MODELS_URL, payload, _models_headers())
        if resp is None:
            last_error = "GitHub Models unreachable"
            print(f"  {last_error}")
            break
        if resp.status_code == 200:
            data = resp.json()
            return data["choices"][0]["message"]["content"], None, data.get("usage", {})
        elif resp.status_code == 429:
            _sleep_for_retry(resp, attempt)
            continue
        else:
            try:
                detail = resp.json().get("error", {}).get("message", resp.text[:120])
            except Exception:
                detail = resp.text[:120]
            last_error = f"HTTP {resp.status_code}: {detail}"
            print(f"  GitHub Models error {resp.status_code}: {detail}")
            break

    print(f"  All {AI_RETRY_MAX + 1} attempt(s) exhausted — skipping chunk")
    return None, last_error, {}


# ---------------------------------------------------------------------------
# Response parsing
# ---------------------------------------------------------------------------

def parse_response(text: str) -> tuple[str | None, list[str]]:
    """Return (code, fixes) extracted from the structured AI response.

    Primary: expects <CODE>...</CODE> tags as instructed in the system prompt.
    Fallback 1: strips markdown fences (```csharp ... ```) if present.
    Fallback 2: treats the entire response as raw code.
    Validation downstream will reject anything that isn't valid C#.
    """
    code_start = text.find("<CODE>")
    code_end = text.find("</CODE>")
    fixes_start = text.find("<FIXES>")
    fixes_end = text.find("</FIXES>")

    if code_start != -1 and code_end != -1:
        code = text[code_start + len("<CODE>"):code_end].strip()
    else:
        stripped = text.strip()
        fence_match = re.match(r"^```[^\n]*\n(.*?)```\s*$", stripped, re.DOTALL)
        if fence_match:
            code = fence_match.group(1).strip()
        else:
            code = stripped

    fixes: list[str] = []
    if fixes_start != -1 and fixes_end != -1:
        raw = text[fixes_start + len("<FIXES>"):fixes_end].strip()
        fixes = [ln.strip() for ln in raw.splitlines() if ln.strip()]

    return code if code else None, fixes


# ---------------------------------------------------------------------------
# Validation
# ---------------------------------------------------------------------------

def validate_code(code: str, original: str) -> tuple[bool, str]:
    """Five-check heuristic validation of the AI-returned code."""
    if not code:
        return False, "empty"
    if len(code) < VALIDATION_MIN_CHARS:
        return False, f"too short ({len(code)} chars)"
    ratio = len(code) / max(len(original), 1)
    if not (VALIDATION_MIN_SIZE_RATIO <= ratio <= VALIDATION_MAX_SIZE_RATIO):
        return False, f"size ratio {ratio:.2f} outside [{VALIDATION_MIN_SIZE_RATIO}, {VALIDATION_MAX_SIZE_RATIO}]"
    ns_match = re.search(r"namespace\s+\S+", original)
    if ns_match and ns_match.group(0) not in code:
        return False, "namespace declaration missing"
    if abs(code.count("{") - code.count("}")) > 2:
        return False, f"unbalanced braces"
    if "```" in code:
        return False, "contains markdown fences"
    return True, "ok"


# ---------------------------------------------------------------------------
# Commit message
# ---------------------------------------------------------------------------

def build_commit_message(batch: dict, all_fixes: list[str]) -> str:
    fname = os.path.basename(batch["local_path"])
    n = batch["issue_count"]
    bid = batch["batch_id"]
    subject = f"fix(sonar): {n} issues in {fname} [batch-{bid}, run-{_RUN_ID}]"
    body = "\n".join(all_fixes) if all_fixes else "(no fix details returned)"
    return f"{subject}\n\n{body}"


# ---------------------------------------------------------------------------
# Git helpers
# ---------------------------------------------------------------------------

def _git_add(path: str) -> None:
    subprocess.run(["git", "add", path], check=True, capture_output=True)


def _git_commit(message: str) -> bool:
    result = subprocess.run(
        ["git", "commit", "-m", message],
        capture_output=True,
        text=True,
    )
    if result.returncode == 0:
        return True
    if "nothing to commit" in (result.stdout + result.stderr):
        return False
    raise RuntimeError(f"git commit failed:\n{result.stderr.strip()}")


# ---------------------------------------------------------------------------
# Fix result writer
# ---------------------------------------------------------------------------

def _write_fix_result(
    batch: dict,
    fixes: list[str],
    status: str,
    chunks_done: int,
    chunks_total: int,
    skip_reason: str | None = None,
    token_usage: dict | None = None,
) -> None:
    record = {
        "batch_id": batch["batch_id"],
        "local_path": batch["local_path"],
        "status": status,
        "issues_total": batch["issue_count"],
        "chunks_total": chunks_total,
        "chunks_succeeded": chunks_done,
        "fixes": fixes,
    }
    if skip_reason:
        record["skip_reason"] = skip_reason
    if token_usage:
        record["token_usage"] = token_usage
    out = os.path.join(FIX_RESULTS_DIR, f"batch_{batch['batch_id']}.json")
    with open(out, "w", encoding="utf-8") as f:
        json.dump(record, f, indent=2)


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main() -> int:
    _ensure_prompts_loaded()

    if not os.path.isfile(BATCHES_FILE):
        print(f"ERROR: {BATCHES_FILE} not found — run batch_issues.py first", file=sys.stderr)
        return EXIT_CODE_FATAL

    with open(BATCHES_FILE, encoding="utf-8") as f:
        data = json.load(f)

    batches: list[dict] = data.get("batches", [])
    print(f"Loaded {len(batches)} batches | MAX_ISSUES_PER_CALL={_MAX_PER_CALL}")
    print(f"API: {'Copilot Enterprise → ' if _copilot_available else ''}GitHub Models")

    os.makedirs(FIX_RESULTS_DIR, exist_ok=True)

    files_fixed = 0
    files_skipped = 0

    for batch in batches:
        bid = batch["batch_id"]
        local_path = batch["local_path"]
        abs_path = os.path.join(_WORKSPACE, local_path)

        print(f"\n[batch {bid}] {local_path} ({batch['issue_count']} issues)")

        if not os.path.isfile(abs_path):
            print(f"  SKIP: file not found at {abs_path}")
            _write_fix_result(batch, [], "skipped_not_found", 0, 0, f"file not found: {abs_path}")
            files_skipped += 1
            continue

        try:
            original_content = Path(abs_path).read_text(encoding="utf-8")
        except OSError as e:
            print(f"  SKIP: cannot read file: {e}")
            _write_fix_result(batch, [], "read_error", 0, 0, f"cannot read file: {e}")
            files_skipped += 1
            continue

        issues = batch["issues"]
        chunks = [issues[i:i + _MAX_PER_CALL] for i in range(0, len(issues), _MAX_PER_CALL)]
        total_chunks = len(chunks)
        print(f"  {total_chunks} chunk(s) of up to {_MAX_PER_CALL} issues each")

        current_content = original_content
        all_fixes: list[str] = []
        chunks_done = 0
        skip_reason: str | None = None
        file_tokens: dict = {"prompt_tokens": 0, "completion_tokens": 0, "total_tokens": 0}

        for chunk_idx, chunk in enumerate(chunks):
            print(f"  chunk {chunk_idx + 1}/{total_chunks}: {len(chunk)} issues | ~{len(current_content)//4} file tokens")

            raw, api_err, usage = call_ai(current_content, local_path, chunk)
            if raw is None:
                skip_reason = api_err or "API call returned no data"
                print(f"  chunk {chunk_idx + 1}: API failed ({skip_reason}) — stopping sub-batch for this file")
                break

            for k in file_tokens:
                file_tokens[k] += usage.get(k, 0)

            code, fixes = parse_response(raw)
            if code is None:
                skip_reason = "AI response missing <CODE> section"
                print(f"  chunk {chunk_idx + 1}: {skip_reason} — stopping")
                break

            ok, val_reason = validate_code(code, current_content)
            if not ok:
                skip_reason = f"validation failed: {val_reason}"
                print(f"  chunk {chunk_idx + 1}: {skip_reason} — stopping")
                break

            Path(abs_path).write_text(code, encoding="utf-8")
            current_content = code
            all_fixes.extend(fixes)
            chunks_done += 1
            print(f"  chunk {chunk_idx + 1}: OK — {len(fixes)} fix line(s) | "
                  f"{usage.get('total_tokens', 0):,} tokens")

        usage_to_store = file_tokens if any(file_tokens.values()) else None

        if chunks_done > 0:
            _git_add(abs_path)
            msg = build_commit_message(batch, all_fixes)
            committed = _git_commit(msg)
            status = "fixed" if committed else "fixed_no_change"
            _write_fix_result(batch, all_fixes, status, chunks_done, total_chunks,
                              token_usage=usage_to_store)
            files_fixed += 1
            print(f"  committed ({files_fixed} file(s) fixed so far)")
        else:
            _write_fix_result(batch, [], "skipped", 0, total_chunks, skip_reason,
                              token_usage=usage_to_store)
            files_skipped += 1
            print(f"  no changes committed")

        time.sleep(AI_INTER_FILE_DELAY_SEC)

    print(f"\n--- Summary ---")
    print(f"Files fixed:   {files_fixed}")
    print(f"Files skipped: {files_skipped}")

    if files_fixed == 0:
        print("No fixes applied — all batches skipped")
        return EXIT_CODE_ALL_SKIPPED
    return EXIT_CODE_SUCCESS


if __name__ == "__main__":
    sys.exit(main())
