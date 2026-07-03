"""
Create a GitHub PR containing all AI-generated SonarQube fixes.

Stages the batch output JSON files, commits them, creates a new branch
(ai/sonarqube-fixes-{run_number}), pushes it, and opens a PR via `gh`.

Environment variables:
    GITHUB_TOKEN          Built-in Actions token (for gh auth)
    GITHUB_RUN_ID         Actions run ID (informational)
    GITHUB_RUN_NUMBER     Actions run number (branch name suffix)
    GITHUB_BASE_BRANCH    PR target branch (default: agent/fix)
    FIX_EXIT_CODE         Exit code from fix_batch.py (0=fixed, 2=all_skipped)
"""

import glob
import json
import os
import subprocess
import sys
from pathlib import Path

sys.path.insert(0, os.path.join(os.path.dirname(__file__), ".."))
from shared.constants import (
    BATCHES_FILE,
    EXIT_CODE_FATAL,
    EXIT_CODE_SUCCESS,
    FIX_RESULTS_DIR,
    ISSUES_FILE,
)

_RUN_NUMBER = os.environ.get("GITHUB_RUN_NUMBER", "0")
_RUN_ID = os.environ.get("GITHUB_RUN_ID", "local")
_BASE_BRANCH = os.environ.get("GITHUB_BASE_BRANCH", "agent/fix")
_FIX_EXIT_CODE = os.environ.get("FIX_EXIT_CODE", "0")


# ---------------------------------------------------------------------------
# Git / shell helpers
# ---------------------------------------------------------------------------

def _run(cmd: list[str], **kwargs) -> subprocess.CompletedProcess:
    return subprocess.run(cmd, check=True, **kwargs)


def _commit_if_staged(message: str) -> bool:
    """Commit only if there are staged changes. Returns True if committed."""
    diff = subprocess.run(
        ["git", "diff", "--cached", "--quiet"],
        capture_output=True,
    )
    if diff.returncode != 0:
        _run(["git", "commit", "-m", message])
        return True
    return False


# ---------------------------------------------------------------------------
# PR body builder
# ---------------------------------------------------------------------------

def _load_fix_results() -> list[dict]:
    pattern = os.path.join(FIX_RESULTS_DIR, "batch_*.json")
    results = []
    for path in sorted(glob.glob(pattern)):
        with open(path, encoding="utf-8") as f:
            results.append(json.load(f))
    return results


def _load_issue_count() -> int:
    try:
        with open(ISSUES_FILE, encoding="utf-8") as f:
            return json.load(f).get("total_fetched", 0)
    except (OSError, json.JSONDecodeError):
        return 0


def _build_pr_body(results: list[dict]) -> str:
    fixed = [r for r in results if r.get("status") in ("fixed", "fixed_no_change")]
    skipped = [r for r in results if r not in fixed]
    total_issues_addressed = sum(r.get("issues_total", 0) for r in fixed)
    all_skipped = _FIX_EXIT_CODE == "2"

    lines = [
        "## SonarQube AI Auto-Remediation",
        "",
        f"**Run:** #{_RUN_NUMBER} &nbsp;|&nbsp; **ID:** `{_RUN_ID}`",
        "",
    ]

    if all_skipped:
        lines += [
            "> ⚠️ **No fixes were applied** — all batches were skipped (ambiguous fixes "
            "or API unavailable). The batch output below shows what was scanned.",
            "",
        ]

    lines += [
        "### Summary",
        "",
        "| Metric | Value |",
        "|--------|-------|",
        f"| Files fixed | {len(fixed)} |",
        f"| Files skipped | {len(skipped)} |",
        f"| Issues addressed | {total_issues_addressed} |",
        f"| Total issues scanned | {_load_issue_count()} |",
        "",
    ]

    if fixed:
        lines += [
            "### Fixed files",
            "",
            "| File | Issues | Chunks | Rules applied |",
            "|------|--------|--------|---------------|",
        ]
        for r in fixed:
            fname = os.path.basename(r["local_path"])
            n = r.get("issues_total", "?")
            chunks = f"{r.get('chunks_succeeded', '?')}/{r.get('chunks_total', '?')}"
            rules: set[str] = set()
            for fix_line in r.get("fixes", []):
                parts = fix_line.split(":")
                if parts and parts[0]:
                    rules.add(parts[0])
            rules_str = ", ".join(sorted(rules)) if rules else "—"
            lines.append(f"| `{fname}` | {n} | {chunks} | {rules_str} |")
        lines.append("")

    if skipped:
        lines += [
            "### Skipped files",
            "",
            "| File | Reason |",
            "|------|--------|",
        ]
        for r in skipped:
            fname = os.path.basename(r["local_path"])
            status = r.get("status", "unknown")
            lines.append(f"| `{fname}` | {status} |")
        lines.append("")

    lines += [
        "---",
        "> ⚠️ **Review all changes before merging.** "
        "A follow-up SonarQube scan is the authoritative quality gate — "
        "remaining or newly introduced issues will appear in the next run.",
        "",
        "🤖 Generated with [Claude Code](https://claude.ai/claude-code)",
    ]

    return "\n".join(lines)


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main() -> int:
    fix_branch = f"ai/sonarqube-fixes-{_RUN_NUMBER}"
    results = _load_fix_results()

    # Stage batch output files (fix_results/ stays on disk for PR body but is not committed)
    stage_targets = []
    for path in [ISSUES_FILE, BATCHES_FILE]:
        if os.path.isfile(path):
            stage_targets.append(path)

    if stage_targets:
        _run(["git", "add", "-f"] + stage_targets)

    _commit_if_staged(f"ci: sonar batch output and fix results — run #{_RUN_NUMBER}")

    # Create and push the fix branch
    print(f"Creating branch: {fix_branch}")
    _run(["git", "checkout", "-b", fix_branch])
    _run(["git", "push", "-u", "origin", fix_branch])

    # Build PR
    fixed_count = sum(1 for r in results if r.get("status") in ("fixed", "fixed_no_change"))
    total = len(results)
    title = (
        f"fix(sonar): AI auto-remediation — {fixed_count}/{total} files fixed "
        f"[run #{_RUN_NUMBER}]"
    )
    body = _build_pr_body(results)

    pr_result = subprocess.run(
        ["gh", "pr", "create",
         "--title", title,
         "--body", body,
         "--base", _BASE_BRANCH,
         "--head", fix_branch],
        capture_output=True,
        text=True,
    )

    if pr_result.returncode != 0:
        print(f"ERROR: gh pr create failed:\n{pr_result.stderr}", file=sys.stderr)
        return EXIT_CODE_FATAL

    pr_url = pr_result.stdout.strip()
    print(f"PR created: {pr_url}")

    summary_file = os.environ.get("GITHUB_STEP_SUMMARY", "")
    if summary_file:
        with open(summary_file, "a", encoding="utf-8") as f:
            f.write(
                f"## AI Auto-Fix — Run #{_RUN_NUMBER}\n\n"
                f"| | |\n|---|---|\n"
                f"| Pull request | {pr_url} |\n"
                f"| Files fixed | {fixed_count}/{total} |\n"
                f"| Issues addressed | "
                f"{sum(r.get('issues_total', 0) for r in results if r.get('status') in ('fixed', 'fixed_no_change'))} |\n"
            )

    return EXIT_CODE_SUCCESS


if __name__ == "__main__":
    sys.exit(main())
