---
title: Python Scripts Reference
tags: [scripts, python, fetch, batch, fix, create-pr, constants]
aliases: [scripts, python-scripts, script-reference]
---

# Python Scripts Reference

All scripts live under `scripts/` and share configuration from `scripts/shared/constants.py`. They communicate through JSON files under `output/`.

---

## `scripts/shared/constants.py`

Central configuration — no magic strings anywhere else.

```python
# Severity ranking (higher = more severe)
SEVERITY_SCORE = { "BLOCKER": 5, "CRITICAL": 4, "MAJOR": 3, "MINOR": 2, "INFO": 1 }

# SonarQube pagination
SONAR_PAGE_SIZE = 500
SONAR_MAX_OFFSET = 10_000   # hard cap — decompose by type above this

# Token estimation: ~4 UTF-8 bytes per token
TOKEN_CHARS_PER_TOKEN = 4
MAX_TOKENS_PER_FILE = 3_000  # files above this are flagged large_file=true

# AI API endpoints
COPILOT_URL = "https://api.githubcopilot.com/chat/completions"
MODELS_URL  = "https://models.inference.ai.azure.com/chat/completions"
AI_MODEL    = "gpt-4o"

# Retry / rate-limit
AI_RETRY_MAX = 3
AI_INTER_FILE_DELAY_SEC = 3  # sleep between files

# Response validation thresholds
VALIDATION_MIN_CHARS      = 50
VALIDATION_MAX_SIZE_RATIO = 2.5
VALIDATION_MIN_SIZE_RATIO = 0.3

# Exit codes
EXIT_CODE_SUCCESS     = 0
EXIT_CODE_ALL_SKIPPED = 2
EXIT_CODE_FATAL       = 3

# Sub-batching: max issues per single AI call
MAX_ISSUES_PER_CALL = 15

# Output paths
OUTPUT_DIR      = "output"
ISSUES_FILE     = "output/issues.json"
BATCHES_FILE    = "output/batches.json"
FIX_RESULTS_DIR = "output/fix_results"
```

---

## `scripts/sonar/fetch_issues.py`

Paginates the SonarQube REST API and writes all issues to `output/issues.json`.

### Environment variables

| Variable | Required | Description |
|----------|----------|-------------|
| `SONARQUBE_URL` | Yes | Base URL e.g. `http://localhost:9000` |
| `SONARQUBE_TOKEN` | Yes | Token as HTTP Basic username (empty password) |
| `SONAR_PROJECT_KEYS` | Yes | Comma-separated project keys |
| `SONAR_SEVERITIES` | Yes | Comma-separated: `BLOCKER,CRITICAL,MAJOR,MINOR,INFO` |
| `SONAR_LANGUAGE` | No | Language filter e.g. `cs` |

### Pagination strategy

1. Query `(project_key, severity)` pair — check `paging.total`
2. If ≤ 10,000: paginate normally (500/page, up to 20 pages)
3. If > 10,000: decompose by issue type (`BUG`, `VULNERABILITY`, `CODE_SMELL`)
4. Deduplication: tracks seen issue keys across all queries

### Auth

```python
session.auth = (SONARQUBE_TOKEN, "")  # token as username, empty password
```

### Output — `output/issues.json`

```json
{
  "fetched_at": "2026-07-07T10:00:00Z",
  "total_fetched": 481,
  "project_keys": ["sonar-test-bad-code"],
  "severities": ["BLOCKER", "CRITICAL", "MAJOR"],
  "language": "cs",
  "issues": [
    {
      "key": "AXmJ...",
      "rule": "csharpsquid:S1481",
      "severity": "MAJOR",
      "component": "sonar-test-bad-code:SonarTestBadCode/Controllers/HomeController.cs",
      "line": 42,
      "message": "Remove the unused local variable 'resultado'.",
      "status": "OPEN",
      "type": "CODE_SMELL"
    }
  ]
}
```

---

## `scripts/sonar/batch_issues.py`

Groups issues by source file, estimates token counts, and assigns sequential batch IDs.

### Environment variables

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `WORKSPACE_ROOT` | No | `cwd` | Absolute path to checked-out repo |
| `MAX_FILES` | No | `0` | Cap on output batches; 0 = unlimited |
| `SCAN_COMMIT_SHA` | No | `""` | Commit SHA embedded in output for downstream validation |

### Three-tier batching strategy

**Tier 1 — Per-class:** One source file = one batch. Issues for the same file are grouped together.

**Tier 2 — Per-type:** Within each file, issues are sorted by rule ID then line number. Related violations are adjacent, making the AI prompt easier to reason about.

**Tier 3 — Numeric:** Batches are numbered 0, 1, 2 … ordered by descending severity. Batch 0 = highest-severity file. Tie-breaks: more issues first, then alphabetical.

### `MAX_FILES` capping

Applied **before** Tier 3 numeric IDs are assigned, not after. If `MAX_FILES > 0` and the file count exceeds it, entries are re-sorted by descending severity then descending issue count (no alphabetical tie-break at this stage) and truncated to the top `MAX_FILES`. This ensures the cap keeps the most severe/impactful files rather than an arbitrary or first-seen subset. Tier 3's own sort (with the alphabetical tie-break) then runs on the already-capped list.

### Large-file flagging

Each file's size is estimated in tokens (`file_size_bytes // TOKEN_CHARS_PER_TOKEN`) and compared against `MAX_TOKENS_PER_FILE` (3,000). Files above the threshold get `large_file: true` in the output. This is informational metadata only — `batch_issues.py` never excludes or resizes a file because of it, and `fix_batch.py` does not read this flag; large files still go through normal sub-batching by issue count via `MAX_ISSUES_PER_CALL`.

### Component path mapping

```python
# "sonar-test-bad-code:SonarTestBadCode/Controllers/HomeController.cs"
# → "SonarTestBadCode/Controllers/HomeController.cs"
local_path = component[component.index(":") + 1:]
```

Files not found in the workspace are logged and excluded from output.

### Output — `output/batches.json`

```json
{
  "scan_commit": "7f08ff5a3b2c...",
  "total_files_with_issues": 7,
  "total_batches": 7,
  "total_issues": 481,
  "skipped_not_found": 0,
  "severity_distribution": { "MAJOR": 5, "MINOR": 2 },
  "batches": [
    {
      "batch_id": 0,
      "component": "sonar-test-bad-code:SonarTestBadCode/Repositories/UserRepository.cs",
      "local_path": "SonarTestBadCode/Repositories/UserRepository.cs",
      "abs_path": "/home/runner/work/.../UserRepository.cs",
      "file_exists": true,
      "file_size_bytes": 12450,
      "large_file": false,
      "max_severity": "MAJOR",
      "issue_count": 79,
      "rules_present": ["csharpsquid:S112", "csharpsquid:S1481"],
      "issues": [
        { "line": 42, "rule": "csharpsquid:S1481", "severity": "MAJOR",
          "message": "Remove the unused local variable 'x'.", "type": "CODE_SMELL" }
      ]
    }
  ]
}
```

---

## `scripts/ai/fix_batch.py`

The AI remediation loop. Reads `output/batches.json`, calls the AI API for each batch, validates the response, writes fixed files, and commits each one.

### Environment variables

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `WORKSPACE_ROOT` | No | `cwd` | Absolute path to workspace |
| `COPILOT_TOKEN` | No | `""` | GitHub PAT with `copilot` scope (primary API) |
| `GITHUB_TOKEN` | Yes | — | Built-in Actions token (GitHub Models fallback) |
| `MAX_ISSUES_PER_CALL` | No | `15` | Issues per AI request; controls sub-batching |
| `GITHUB_RUN_ID` | No | `local` | Written into commit messages |
| `SEVERITY_FILTER` | No | `""` | Comma-separated severities to process (e.g. `BLOCKER,CRITICAL`). Empty = all severities |
| `ISSUE_TYPE_FILTER` | No | `""` | Comma-separated types to process (e.g. `BUG,VULNERABILITY`). Empty = all types |

### AI API — two-tier fallback

**Primary: GitHub Copilot Enterprise**
- Endpoint: `https://api.githubcopilot.com/chat/completions`
- Auth: exchanges `COPILOT_TOKEN` PAT for a short-lived session token (~30 min TTL) via `GET https://api.github.com/copilot_internal/v2/token`
- Headers: `Copilot-Integration-Id: vscode-chat`

**Fallback: GitHub Models (GPT-4o)**
- Endpoint: `https://models.inference.ai.azure.com/chat/completions`
- Auth: `Authorization: Bearer {GITHUB_TOKEN}`
- Triggered by: Copilot returning 401/403, unreachable, or any non-200

**Rate limiting:** On 429, respects the `retry-after` header, falls back to exponential backoff (`2^attempt`). Max `AI_RETRY_MAX` (3) retries.

### Issue filtering

Before sub-batching, each file's issue list is passed through `_filter_issues()`:

```python
# Both filters are AND-combined. Empty set = accept all.
if _SEVERITY_FILTER:
    issues = [i for i in issues if i["severity"].upper() in _SEVERITY_FILTER]
if _ISSUE_TYPE_FILTER:
    issues = [i for i in issues if i["type"].upper() in _ISSUE_TYPE_FILTER]
```

| Outcome | What happens |
|---------|-------------|
| All issues pass the filter | Normal processing |
| Some issues pass | Only the passing issues are sent to the AI; the rest are silently omitted from the prompt |
| Zero issues pass | Batch status set to `skipped_filtered`; file is counted as skipped, no API call made |

The active filters are printed at startup and appear in the workflow step summary.

### Sub-batching for large files

When `issue_count > MAX_ISSUES_PER_CALL`, the issues are split into chunks:

```
Chunk 1: read original file → send issues 1-15 → validate → write
Chunk 2: read MODIFIED file → send issues 16-30 → validate → write
...
```

Each chunk reads the already-modified file so line numbers remain accurate for the model. If any chunk fails validation, the remaining chunks for that file are skipped (non-fatal).

### Response parsing

The AI is instructed to return responses in a structured format:

```
<CODE>
(complete corrected file)
</CODE>
<FIXES>
S1481:42: Removed unused variable 'result'.
S125:67: Deleted commented-out code block.
</FIXES>
```

Fallback parsing: if `<CODE>` tags are missing, strips markdown fences; if no fences, uses the raw response.

### Validation chain (five checks)

| Check | Failure reason |
|-------|----------------|
| Response length ≥ 50 chars | `too short` |
| Size ratio: `0.3 ≤ len(response)/len(original) ≤ 2.5` | `size ratio X outside [0.3, 2.5]` |
| Original namespace declaration still present | `namespace declaration missing` |
| `abs('{' count - '}' count) ≤ 2` | `unbalanced braces` |
| Response does not contain ` ``` ` | `contains markdown fences` |

On size ratio failure (first attempt only): retries with an explicit "return the COMPLETE file" note appended to the prompt.

### Commit message format

```
fix(sonar): 79 issues in UserRepository.cs [batch-0, run-{RUN_ID}]

S1481:42: Removed unused variable 'result'.
S125:67: Deleted commented-out code block.
```

### Exit codes

| Code | Meaning |
|------|---------|
| `0` | At least one file was fixed |
| `2` | All batches skipped (non-fatal — PR is still created) |
| `3` | Fatal error (batches.json missing, workspace unreadable) |

### Output — `output/fix_results/batch_N.json`

```json
{
  "batch_id": 0,
  "local_path": "SonarTestBadCode/Repositories/UserRepository.cs",
  "status": "fixed",
  "issues_total": 79,
  "chunks_total": 6,
  "chunks_succeeded": 6,
  "fixes": ["S1481:42: Removed unused variable 'result'."],
  "token_usage": { "prompt_tokens": 4200, "completion_tokens": 3800, "total_tokens": 8000 }
}
```

Status values: `fixed`, `fixed_no_change`, `skipped`, `skipped_not_found`, `read_error`, `skipped_filtered`

---

## `scripts/git/create_pr.py`

Aggregates fix results, pushes the fix branch, and opens a GitHub PR.

### Environment variables

| Variable | Description |
|----------|-------------|
| `GITHUB_TOKEN` | Built-in Actions token (for `gh` auth) |
| `GITHUB_RUN_ID` | Workflow run ID (informational) |
| `GITHUB_RUN_NUMBER` | Used in branch and PR names |
| `GITHUB_BASE_BRANCH` | PR target branch (default: `agent/fix`) |
| `FIX_EXIT_CODE` | Exit code from `fix_batch.py` — affects PR body warning |
| `WORKFLOW_NAME` | Workflow display name written into PR body |

### Steps

1. Loads all `output/fix_results/batch_*.json`
2. Force-adds `output/issues.json` and `output/batches.json` (gitignored)
3. Commits them: `ci: sonar batch output and fix results — run #{N}`
4. Creates branch `ai/sonarqube-fixes-{run_number}` and pushes
5. Builds PR body with fixed/skipped file tables + token usage summary
6. Runs `gh pr create --base {GITHUB_BASE_BRANCH} --head ai/sonarqube-fixes-{N}`
7. Writes PR URL to `$GITHUB_STEP_SUMMARY`

### PR title format

```
fix(sonar): AI auto-remediation — {fixed}/{total} files fixed [run #{N}]
```

### PR body includes

- Warning banner if all files were skipped (`exit_code = 2`)
- Summary table: files fixed, files skipped, issues addressed, **issues skipped by AI**, total issues scanned
- Fixed files table: file name, issue count, **per-file skipped count** (bolded when non-zero), chunks used, rules applied
- "Issues skipped by AI" table: file, rule, line, reason — for issues the AI individually marked unresolvable within otherwise-fixed files
- Skipped files table: file name, issue count, skip reason
- Token usage table: prompt tokens, completion tokens, total, API call count
- Review reminder: "A follow-up SonarQube scan is the authoritative quality gate"

---

## Related documents

- [[04-workflow-batch]] — how scripts are invoked in the batch workflow
- [[05-workflow-fix]] — how scripts are invoked in the fix workflow
- [[08-ai-prompts]] — the prompts `fix_batch.py` loads
- [[12-hallucination-mitigation]] — validation rationale
