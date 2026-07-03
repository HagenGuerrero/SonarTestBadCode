"""
Create GitHub Copilot Agent Tasks for SonarQube issues that fix_batch.py skipped.

Reads output/fix_results/batch_*.json for status=skipped batches,
cross-references output/batches.json for the original issue list, and fires
one Agent Task per skipped file via the GitHub Agent Tasks REST API.

Skips gracefully (exit 0) when:
  - COPILOT_PAT is not set (PAT required — GITHUB_TOKEN is rejected by the API)
  - No skipped batches exist

Environment variables:
    COPILOT_PAT        Fine-grained PAT with actions:write, contents:write,
                       issues:write, pull-requests:write scopes
                       (GitHub App / GITHUB_TOKEN tokens are explicitly rejected)
    GITHUB_REPOSITORY  e.g. "owner/repo"
    GITHUB_BASE_BRANCH Target branch for the coding agent's PR (default: agent/fix)
    GITHUB_RUN_NUMBER  Actions run number (informational)
"""

import glob
import json
import os
import sys
import time
from pathlib import Path

sys.path.insert(0, os.path.join(os.path.dirname(__file__), ".."))
from shared.constants import (
    BATCHES_FILE,
    EXIT_CODE_FATAL,
    EXIT_CODE_SUCCESS,
    FIX_RESULTS_DIR,
)

try:
    import requests
except ImportError:
    print("ERROR: 'requests' not installed — run: pip install requests", file=sys.stderr)
    sys.exit(EXIT_CODE_FATAL)

_PAT = os.environ.get("COPILOT_PAT", "")
_REPO = os.environ.get("GITHUB_REPOSITORY", "")
_BASE_BRANCH = os.environ.get("GITHUB_BASE_BRANCH", "agent/fix")
_RUN_NUMBER = os.environ.get("GITHUB_RUN_NUMBER", "0")

_AGENT_TASKS_URL = "https://api.github.com/agents/repos/{owner}/{repo}/tasks"
_API_VERSION = "2026-03-10"


# ---------------------------------------------------------------------------
# Data loading
# ---------------------------------------------------------------------------

def _load_skipped_batch_ids() -> list[int]:
    pattern = os.path.join(FIX_RESULTS_DIR, "batch_*.json")
    skipped = []
    for path in sorted(glob.glob(pattern)):
        with open(path, encoding="utf-8") as f:
            result = json.load(f)
        if result.get("status") in ("skipped", "skipped_not_found", "read_error"):
            # Only queue "skipped" (AI couldn't fix it) — not infrastructure errors
            if result.get("status") == "skipped":
                skipped.append(result["batch_id"])
    return skipped


def _load_batches_index() -> dict[int, dict]:
    try:
        with open(BATCHES_FILE, encoding="utf-8") as f:
            data = json.load(f)
        return {b["batch_id"]: b for b in data.get("batches", [])}
    except (OSError, json.JSONDecodeError) as e:
        print(f"ERROR: cannot read {BATCHES_FILE}: {e}", file=sys.stderr)
        return {}


# ---------------------------------------------------------------------------
# Prompt builder
# ---------------------------------------------------------------------------

def _build_task_prompt(batch: dict) -> str:
    issues = batch.get("issues", [])
    local_path = batch["local_path"]

    lines = [
        f"Fix the following SonarQube violations in `{local_path}`.",
        "",
        "These issues were attempted by an automated AI pass but could not be resolved",
        "because they require broader codebase context (e.g. call graphs, control flow,",
        "logic dependencies, or deeply nested branching).",
        "",
        "## Issues to fix",
        "",
        "| Line | Rule | Severity | Message |",
        "|------|------|----------|---------|",
    ]
    for issue in issues:
        rule = issue.get("rule", "")
        line_no = issue.get("line", "?")
        sev = issue.get("severity", "")
        msg = issue.get("message", "").replace("|", "\\|")
        lines.append(f"| {line_no} | {rule} | {sev} | {msg} |")

    lines += [
        "",
        "## Requirements",
        "",
        "- Fix **only** the listed issues — do not refactor or change unrelated code",
        "- Preserve all existing public API, method signatures, and behavior",
        "- The file must compile as .NET Framework 4.8 C#",
        "- Each fix must be minimal and targeted to the specific line/rule listed",
        f"- This is part of SonarQube auto-remediation run #{_RUN_NUMBER}",
    ]

    return "\n".join(lines)


# ---------------------------------------------------------------------------
# API call
# ---------------------------------------------------------------------------

def _create_task(owner: str, repo: str, prompt: str, batch_id: int) -> dict | None:
    url = _AGENT_TASKS_URL.format(owner=owner, repo=repo)
    headers = {
        "Authorization": f"Bearer {_PAT}",
        "Accept": "application/vnd.github+json",
        "X-GitHub-Api-Version": _API_VERSION,
        "Content-Type": "application/json",
    }
    payload = {
        "prompt": prompt,
        "base_ref": _BASE_BRANCH,
    }

    try:
        resp = requests.post(url, json=payload, headers=headers, timeout=30)
    except requests.RequestException as e:
        print(f"  [batch {batch_id}] Network error: {e}")
        return None

    if resp.status_code == 201:
        return resp.json()
    elif resp.status_code == 429:
        retry_after = int(resp.headers.get("retry-after", 60))
        print(f"  [batch {batch_id}] Rate limited — waiting {retry_after}s")
        time.sleep(retry_after)
        # One retry after rate limit
        try:
            resp = requests.post(url, json=payload, headers=headers, timeout=30)
            if resp.status_code == 201:
                return resp.json()
        except requests.RequestException:
            pass

    print(f"  [batch {batch_id}] API error {resp.status_code}: {resp.text[:300]}")
    return None


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main() -> int:
    if not _PAT:
        print(
            "COPILOT_PAT not set — skipping Copilot Agent Task creation.\n"
            "To enable: add a fine-grained PAT with actions:write, contents:write,\n"
            "issues:write, pull-requests:write as repository secret 'COPILOT_PAT'.\n"
            "Note: GITHUB_TOKEN is explicitly rejected by the Agent Tasks API."
        )
        return EXIT_CODE_SUCCESS

    if not _REPO or "/" not in _REPO:
        print(f"ERROR: GITHUB_REPOSITORY not set or invalid: '{_REPO}'", file=sys.stderr)
        return EXIT_CODE_FATAL

    owner, repo = _REPO.split("/", 1)

    skipped_ids = _load_skipped_batch_ids()
    if not skipped_ids:
        print("No skipped batches found — nothing to send to the Copilot coding agent.")
        return EXIT_CODE_SUCCESS

    batches_index = _load_batches_index()
    if not batches_index:
        return EXIT_CODE_FATAL

    print(f"Creating Copilot Agent Tasks for {len(skipped_ids)} skipped batch(es)...")
    print(f"Target branch: {_BASE_BRANCH}")

    tasks_created = []
    tasks_failed = []

    for bid in skipped_ids:
        batch = batches_index.get(bid)
        if not batch:
            print(f"  [batch {bid}] Not found in batches.json — skipping")
            tasks_failed.append(bid)
            continue

        fname = os.path.basename(batch["local_path"])
        n_issues = batch.get("issue_count", len(batch.get("issues", [])))
        print(f"  [batch {bid}] {fname} ({n_issues} issues)")

        prompt = _build_task_prompt(batch)
        result = _create_task(owner, repo, prompt, bid)

        if result:
            task_id = result.get("id", "unknown")
            task_url = result.get("html_url", "")
            print(f"  [batch {bid}] Task created: {task_url or task_id}")
            tasks_created.append({"batch_id": bid, "task_id": task_id, "url": task_url, "file": fname})
            time.sleep(2)  # avoid burst rate limiting between tasks
        else:
            tasks_failed.append(bid)

    print(f"\n--- Copilot Agent Tasks ---")
    print(f"Created: {len(tasks_created)}")
    print(f"Failed:  {len(tasks_failed)}")

    if tasks_created:
        print("\nTasks created:")
        for t in tasks_created:
            print(f"  {t['file']}: {t['url'] or t['task_id']}")

    summary_file = os.environ.get("GITHUB_STEP_SUMMARY", "")
    if summary_file and tasks_created:
        with open(summary_file, "a", encoding="utf-8") as f:
            f.write("\n## Copilot Agent Tasks (skipped files)\n\n")
            f.write("| File | Task |\n|------|------|\n")
            for t in tasks_created:
                link = f"[{t['task_id']}]({t['url']})" if t["url"] else t["task_id"]
                f.write(f"| `{t['file']}` | {link} |\n")
            f.write(
                f"\nThe coding agent will open draft PRs targeting `{_BASE_BRANCH}`. "
                "Review and merge each PR independently.\n"
            )

    return EXIT_CODE_SUCCESS


if __name__ == "__main__":
    sys.exit(main())
