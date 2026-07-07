"""
Batch SonarQube issues from output/issues.json → output/batches.json.

Batching strategy (three tiers):
  1. Per-class  — one class/file = one batch; all issues for that file go together.
  2. Per-type   — within each batch the issue list is sorted by rule ID so that
                  related violations are adjacent when the AI prompt is built.
  3. Numeric    — batches are assigned sequential integer IDs (0, 1, 2 …),
                  ordered by descending max-severity so the most critical files
                  are processed first.

Usage:
    python scripts/sonar/batch_issues.py

Environment variables:
    WORKSPACE_ROOT   Absolute path to the checked-out repo (default: cwd)
    MAX_FILES        Cap on how many files to include; 0 = unlimited (default: 0)

Reads:  output/issues.json   (written by fetch_issues.py)
Writes: output/batches.json
"""

import json
import os
import sys
from collections import defaultdict

sys.path.insert(0, os.path.join(os.path.dirname(__file__), ".."))
from shared.constants import (
    BATCHES_FILE,
    EXIT_CODE_FATAL,
    EXIT_CODE_SUCCESS,
    ISSUES_FILE,
    MAX_TOKENS_PER_FILE,
    OUTPUT_DIR,
    SEVERITY_SCORE,
    TOKEN_CHARS_PER_TOKEN,
)

WORKSPACE_ROOT = os.environ.get("WORKSPACE_ROOT", os.getcwd())
MAX_FILES = int(os.environ.get("MAX_FILES", "0"))
SCAN_COMMIT_SHA = os.environ.get("SCAN_COMMIT_SHA", "")


# ---------------------------------------------------------------------------
# Tier 1: Per-class grouping
# ---------------------------------------------------------------------------

def _component_to_local_path(component: str) -> str:
    """Strip 'projectKey:' prefix → relative workspace path."""
    if ":" in component:
        return component[component.index(":") + 1:]
    return component


def _group_by_class(issues: list[dict]) -> dict[str, dict]:
    """
    Returns a dict keyed by component where each value is:
        {
            "component": str,
            "local_path": str,
            "issues": [<issue>, ...]   # raw SonarQube issue objects
        }
    """
    groups: dict[str, dict] = {}
    for issue in issues:
        component = issue.get("component", "")
        if component not in groups:
            groups[component] = {
                "component": component,
                "local_path": _component_to_local_path(component),
                "issues": [],
            }
        groups[component]["issues"].append(issue)
    return groups


# ---------------------------------------------------------------------------
# Tier 2: Per-type sorting within each class
# ---------------------------------------------------------------------------

def _sort_issues_by_type(issues: list[dict]) -> list[dict]:
    """
    Sort issues within a single file so that violations of the same rule
    are grouped together (primary key: rule ID, secondary key: line number).
    This makes the AI prompt easier to reason about and allows it to handle
    all instances of a rule as a coherent set.
    """
    return sorted(issues, key=lambda i: (i.get("rule", ""), i.get("line", 0)))


def _summarize_issue(issue: dict) -> dict:
    """Trim a raw SonarQube issue to the fields the AI prompt needs."""
    return {
        "line": issue.get("line"),
        "rule": issue.get("rule", ""),
        "severity": issue.get("severity", ""),
        "message": issue.get("message", ""),
        "type": issue.get("type", ""),
    }


# ---------------------------------------------------------------------------
# Tier 3: Numeric batch assignment
# ---------------------------------------------------------------------------

def _max_severity_score(issues: list[dict]) -> int:
    return max((SEVERITY_SCORE.get(i.get("severity", ""), 0) for i in issues), default=0)


def _max_severity_label(issues: list[dict]) -> str:
    score = _max_severity_score(issues)
    for label, s in SEVERITY_SCORE.items():
        if s == score:
            return label
    return "UNKNOWN"


def _estimate_tokens(file_bytes: int) -> int:
    return file_bytes // TOKEN_CHARS_PER_TOKEN


def _build_file_entry(group: dict, workspace_root: str) -> dict:
    """
    Produce the per-file record that goes inside a batch.
    Applies tier-2 sorting and annotates with metadata.
    """
    local_path = group["local_path"]
    abs_path = os.path.join(workspace_root, local_path)

    # Tier 2: sort issues by rule type then line
    sorted_issues = _sort_issues_by_type(group["issues"])
    summarized = [_summarize_issue(i) for i in sorted_issues]

    # Collect distinct rules present (useful for the AI prompt header)
    rules_present = sorted({i["rule"] for i in summarized})

    # Token / size estimation
    large_file = False
    file_exists = os.path.isfile(abs_path)
    if file_exists:
        file_size = os.path.getsize(abs_path)
        large_file = _estimate_tokens(file_size) > MAX_TOKENS_PER_FILE
    else:
        file_size = 0

    return {
        "component": group["component"],
        "local_path": local_path,
        "abs_path": abs_path,
        "file_exists": file_exists,
        "file_size_bytes": file_size,
        "large_file": large_file,
        "max_severity": _max_severity_label(group["issues"]),
        "issue_count": len(summarized),
        "rules_present": rules_present,
        "issues": summarized,
    }


def _assign_batch_ids(file_entries: list[dict]) -> list[dict]:
    """
    Tier 3: assign a sequential numeric batch_id to each file entry,
    ordered by descending severity (most critical files first).
    Each file is its own batch (batch_id == index after sorting).
    """
    sorted_entries = sorted(
        file_entries,
        key=lambda e: (
            -SEVERITY_SCORE.get(e["max_severity"], 0),  # descending severity
            -e["issue_count"],                           # tie-break: more issues first
            e["local_path"],                             # final tie-break: alphabetical
        ),
    )

    batches = []
    for idx, entry in enumerate(sorted_entries):
        batches.append({
            "batch_id": idx,
            **entry,
        })
    return batches


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main() -> int:
    if not os.path.isfile(ISSUES_FILE):
        print(f"ERROR: {ISSUES_FILE} not found — run fetch_issues.py first", file=sys.stderr)
        return EXIT_CODE_FATAL

    with open(ISSUES_FILE, encoding="utf-8") as f:
        data = json.load(f)

    raw_issues: list[dict] = data.get("issues", [])
    print(f"Loaded {len(raw_issues)} issues from {ISSUES_FILE}")

    # -----------------------------------------------------------------------
    # Tier 1: group by class (file / component)
    # -----------------------------------------------------------------------
    groups = _group_by_class(raw_issues)
    print(f"Tier 1 (per-class): {len(groups)} distinct files")

    # -----------------------------------------------------------------------
    # Build per-file entries (applies tier-2 sorting internally)
    # -----------------------------------------------------------------------
    file_entries: list[dict] = []
    skipped_not_found: list[str] = []

    for group in groups.values():
        entry = _build_file_entry(group, WORKSPACE_ROOT)
        if not entry["file_exists"]:
            skipped_not_found.append(entry["local_path"])
            continue
        file_entries.append(entry)

    if skipped_not_found:
        print(f"Skipped {len(skipped_not_found)} files not found in workspace:")
        for p in skipped_not_found[:10]:
            print(f"  {p}")
        if len(skipped_not_found) > 10:
            print(f"  ... and {len(skipped_not_found) - 10} more")

    # Apply MAX_FILES cap before numeric assignment
    if MAX_FILES > 0 and len(file_entries) > MAX_FILES:
        # Sort by severity first so we keep the most important files when capping
        file_entries.sort(
            key=lambda e: (
                -SEVERITY_SCORE.get(e["max_severity"], 0),
                -e["issue_count"],
            )
        )
        print(f"MAX_FILES={MAX_FILES}: capping from {len(file_entries)} → {MAX_FILES} files")
        file_entries = file_entries[:MAX_FILES]

    # -----------------------------------------------------------------------
    # Tier 3: assign numeric batch IDs
    # -----------------------------------------------------------------------
    batches = _assign_batch_ids(file_entries)
    print(f"Tier 3 (numeric):   {len(batches)} batches assigned (batch 0 = highest severity)")

    # -----------------------------------------------------------------------
    # Write output
    # -----------------------------------------------------------------------
    os.makedirs(OUTPUT_DIR, exist_ok=True)

    # Per-severity breakdown for the summary
    severity_counts: dict[str, int] = defaultdict(int)
    total_issues = 0
    for b in batches:
        severity_counts[b["max_severity"]] += 1
        total_issues += b["issue_count"]

    output = {
        "scan_commit": SCAN_COMMIT_SHA,
        "total_files_with_issues": len(file_entries),
        "total_batches": len(batches),
        "total_issues": total_issues,
        "skipped_not_found": len(skipped_not_found),
        "severity_distribution": dict(severity_counts),
        "batching_strategy": {
            "tier1": "per-class (one file per batch)",
            "tier2": "per-type (issues sorted by rule within each batch)",
            "tier3": "numeric (batch_id 0..N ordered by descending severity)",
        },
        "batches": batches,
    }

    with open(BATCHES_FILE, "w", encoding="utf-8") as f:
        json.dump(output, f, indent=2)

    print(f"\nWrote {len(batches)} batches ({total_issues} issues) to {BATCHES_FILE}")
    _print_summary(batches)
    return EXIT_CODE_SUCCESS


def _print_summary(batches: list[dict]) -> None:
    print("\nTop 10 batches by severity:")
    header = f"  {'ID':>4}  {'Severity':<10}  {'Issues':>6}  {'Rules':>5}  File"
    print(header)
    print("  " + "-" * (len(header) - 2))
    for b in batches[:10]:
        rules = len(b["rules_present"])
        path = b["local_path"]
        if len(path) > 60:
            path = "..." + path[-57:]
        print(f"  {b['batch_id']:>4}  {b['max_severity']:<10}  {b['issue_count']:>6}  {rules:>5}  {path}")
    if len(batches) > 10:
        print(f"  ... and {len(batches) - 10} more batches")


if __name__ == "__main__":
    sys.exit(main())
