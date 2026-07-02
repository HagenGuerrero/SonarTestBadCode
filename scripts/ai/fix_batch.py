"""
AI auto-fix loop: reads output/batches.json, calls the GitHub Copilot / GitHub
Models API for each batch, writes corrected files, commits per file.

Batches with more issues than MAX_ISSUES_PER_CALL are split into sequential
sub-batches.  Each sub-batch reads the already-modified file so line numbers
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
         2  all batches skipped (non-fatal — phase still "passed")
         3  fatal error (unreadable workspace, batches.json missing)
"""

import json
import os
import re
import subprocess
import sys
import time
from collections import Counter
from pathlib import Path

sys.path.insert(0, os.path.join(os.path.dirname(__file__), ".."))
from shared.constants import (
    AI_INTER_FILE_DELAY_SEC,
    AI_MODEL,
    AI_RETRY_MAX,
    BATCHES_FILE,
    COMMIT_FULL_LIST_THRESHOLD,
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

_WORKSPACE = os.environ.get("WORKSPACE_ROOT", os.getcwd())
_RUN_ID = os.environ.get("GITHUB_RUN_ID", "local")
_MAX_PER_CALL = int(os.environ.get("MAX_ISSUES_PER_CALL", str(MAX_ISSUES_PER_CALL)))


# ---------------------------------------------------------------------------
# Prompt loading
# ---------------------------------------------------------------------------

def _load_system_prompt() -> str:
    system = (_PROMPTS_DIR / "system_prompt.md").read_text(encoding="utf-8")
    agent = (_PROMPTS_DIR / "agent.md").read_text(encoding="utf-8")
    return system + "\n\n---\n\n" + agent


_SYSTEM_PROMPT: str = ""


def _ensure_prompts_loaded() -> None:
    global _SYSTEM_PROMPT
    if not _SYSTEM_PROMPT:
        try:
            _SYSTEM_PROMPT = _load_system_prompt()
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

def _copilot_headers() -> dict:
    return {
        "Authorization": f"Bearer {_COPILOT_TOKEN}",
        "Copilot-Integration-Id": "vscode-chat",
        "Content-Type": "application/json",
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


def call_ai(content: str, file_path: str, issues: list[dict]) -> str | None:
    """Call the AI API with fallback chain. Returns raw response text or None."""
    global _copilot_available

    payload = {
        "model": AI_MODEL,
        "messages": [
            {"role": "system", "content": _SYSTEM_PROMPT},
            {"role": "user", "content": _build_user_prompt(file_path, content, issues)},
        ],
        "temperature": 0.2,
        "max_tokens": 4096,
    }

    for attempt in range(AI_RETRY_MAX + 1):
        if _copilot_available:
            resp = _post(COPILOT_URL, payload, _copilot_headers())
            if resp is None:
                _copilot_available = False
            elif resp.status_code in (401, 403):
                print(f"  Copilot auth failed ({resp.status_code}) — switching to GitHub Models")
                _copilot_available = False
            elif resp.status_code == 200:
                return resp.json()["choices"][0]["message"]["content"]
            elif resp.status_code == 429:
                _sleep_for_retry(resp, attempt)
                continue
            else:
                print(f"  Copilot error {resp.status_code} — switching to GitHub Models")
                _copilot_available = False

        # GitHub Models fallback
        resp = _post(MODELS_URL, payload, _models_headers())
        if resp is None:
            print(f"  GitHub Models unreachable")
            break
        if resp.status_code == 200:
            return resp.json()["choices"][0]["message"]["content"]
        elif resp.status_code == 429:
            _sleep_for_retry(resp, attempt)
            continue
        else:
            print(f"  GitHub Models error {resp.status_code}: {resp.text[:200]}")
            break

    print(f"  All {AI_RETRY_MAX + 1} attempt(s) exhausted — skipping chunk")
    return None


# ---------------------------------------------------------------------------
# Response parsing
# ---------------------------------------------------------------------------

def parse_response(text: str) -> tuple[str | None, list[str]]:
    """Return (code, fixes) extracted from the structured AI response."""
    code_start = text.find("<CODE>")
    code_end = text.find("</CODE>")
    fixes_start = text.find("<FIXES>")
    fixes_end = text.find("</FIXES>")

    if code_start == -1 or code_end == -1:
        return None, []

    code = text[code_start + len("<CODE>"):code_end].strip()

    fixes: list[str] = []
    if fixes_start != -1 and fixes_end != -1:
        raw = text[fixes_start + len("<FIXES>"):fixes_end].strip()
        fixes = [ln.strip() for ln in raw.splitlines() if ln.strip()]

    return code, fixes


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

    brace_diff = abs(code.count("{") - code.count("}"))
    if brace_diff > 2:
        return False, f"unbalanced braces (diff={brace_diff})"

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

    if len(all_fixes) <= COMMIT_FULL_LIST_THRESHOLD:
        body = "\n".join(all_fixes)
    else:
        rule_counts = Counter(
            ln.split(":")[0] for ln in all_fixes if ln and ":" in ln
        )
        top5 = rule_counts.most_common(5)
        top_str = ", ".join(f"{r} ×{c}" for r, c in top5)
        remaining = len(rule_counts) - len(top5)
        if remaining > 0:
            top_str += f", +{remaining} more rule(s)"
        skipped = sum(1 for ln in all_fixes if "SKIPPED" in ln)
        body = f"Rules: {top_str}"
        if skipped:
            body += f"\nSkipped: {skipped} issue(s) — ambiguous or risky, left unchanged"
        body += f"\nFull log: output/fix_results/batch_{bid}.json"

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
) -> None:
    record = {
        "batch_id": batch["batch_id"],
        "local_path": batch["local_path"],
        "status": status,
        "issues_total": batch["issue_count"],
        "chunks_total": chunks_total,
        "chunks_succeeded": chunks_done,
        "copilot_was_available": _copilot_available,
        "fixes": fixes,
    }
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
        # Always recompute abs_path from current workspace so stored paths
        # from a different machine don't break the run.
        abs_path = os.path.join(_WORKSPACE, local_path)

        print(f"\n[batch {bid}] {local_path} ({batch['issue_count']} issues)")

        if not os.path.isfile(abs_path):
            print(f"  SKIP: file not found at {abs_path}")
            _write_fix_result(batch, [], "skipped_not_found", 0, 0)
            files_skipped += 1
            continue

        try:
            original_content = Path(abs_path).read_text(encoding="utf-8")
        except OSError as e:
            print(f"  SKIP: cannot read file: {e}")
            _write_fix_result(batch, [], "read_error", 0, 0)
            files_skipped += 1
            continue

        issues = batch["issues"]
        chunks = [issues[i:i + _MAX_PER_CALL] for i in range(0, len(issues), _MAX_PER_CALL)]
        total_chunks = len(chunks)
        print(f"  {total_chunks} chunk(s) of up to {_MAX_PER_CALL} issues each")

        current_content = original_content
        all_fixes: list[str] = []
        chunks_done = 0

        for chunk_idx, chunk in enumerate(chunks):
            print(f"  chunk {chunk_idx + 1}/{total_chunks}: {len(chunk)} issues")

            raw = call_ai(current_content, local_path, chunk)
            if raw is None:
                print(f"  chunk {chunk_idx + 1}: API failed — stopping sub-batch for this file")
                break

            code, fixes = parse_response(raw)
            if code is None:
                print(f"  chunk {chunk_idx + 1}: response missing <CODE> tags — stopping")
                break

            ok, reason = validate_code(code, current_content)
            if not ok:
                print(f"  chunk {chunk_idx + 1}: validation failed ({reason}) — stopping")
                break

            Path(abs_path).write_text(code, encoding="utf-8")
            current_content = code
            all_fixes.extend(fixes)
            chunks_done += 1
            print(f"  chunk {chunk_idx + 1}: OK — {len(fixes)} fix line(s)")

        if chunks_done > 0:
            _git_add(abs_path)
            msg = build_commit_message(batch, all_fixes)
            committed = _git_commit(msg)
            status = "fixed" if committed else "fixed_no_change"
            _write_fix_result(batch, all_fixes, status, chunks_done, total_chunks)
            files_fixed += 1
            print(f"  committed ({files_fixed} file(s) fixed so far)")
        else:
            _write_fix_result(batch, [], "skipped", 0, total_chunks)
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
