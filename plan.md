# Plan: SonarQube AI Auto-Fix GitHub Actions Workflow

## Context

The project (`PortalVisorCfdi.sln`) has 7,000+ SonarQube findings across 3 project keys. The goal is a GitHub Actions workflow that fetches issues from the self-hosted SonarQube instance, batches them by file, sends each file+issues to an AI API for remediation, commits the fixes to a dedicated branch, and opens a PR for team review — fully automated, requiring only human review at the end.

This is a new workflow alongside the existing `.github/workflows/build.yaml` (MSBuild on `windows-latest`). No existing scripts in the repo to reuse.

---

## Architecture Overview

SonarQube runs as an ephemeral service container inside the GH Actions job. It starts fresh every run — no persistent instance or self-hosted runner required.

```
[workflow_dispatch]
      |
      v
sonarqube service container starts (sonarqube:lts-community, H2, port 9000)
      |
      v
bootstrap: wait for UP → generate token → create project
      |
      v
dotnet sonarscanner begin / dotnet build / dotnet sonarscanner end
      |
      v
fetch_issues.py ──> output/issues.json
      |
      v
batch_issues.py ──> output/batches.json
      |
      v
fix_batch.py (loop per batch) ──> writes files + git commits
      |
      v
create_pr.py ──> GitHub PR on ai/sonarqube-fixes-{run_number}
```

> **Option B (not chosen):** Keep a persistent SonarQube instance (local Docker or hosted VM) and use a self-hosted GH Actions runner that can reach it. Project is created once, token is stored as a secret, and issue history accumulates across runs. Preferred if run speed or historical tracking matters.

---

## Files to Create

```
.github/workflows/sonar-auto-fix.yml    ← main workflow
scripts/sonar/fetch_issues.py           ← SonarQube REST API paginator
scripts/sonar/batch_issues.py           ← groups by file, creates batches
scripts/ai/fix_batch.py                 ← calls AI API, writes fixes, commits
scripts/git/create_pr.py                ← pushes branch, opens PR
scripts/shared/constants.py             ← shared config (no magic strings)
```

---

## 1. Workflow (`sonar-auto-fix.yml`)

### Trigger inputs (all `workflow_dispatch`)

| Input | Default | Purpose |
|-------|---------|---------|
| `sonarqube_url` | `http://localhost:9000` | Base URL of SonarQube (fixed for service container) |
| `project_keys` | all 3 keys | Comma-separated SonarQube project keys |
| `severities` | `BLOCKER,CRITICAL,MAJOR` | Issues to target |
| `language_filter` | `cs` | Only fix these SonarQube language keys |
| `batch_size` | `5` | Files per batch sent to AI |
| `dry_run` | `false` | Fetch+batch only, skip AI calls |
| `max_files` | `0` (unlimited) | Cap execution time for large runs |

### Permissions block
```yaml
permissions:
  contents: write       # push branch, create commits
  pull-requests: write  # create PR
  models: read          # GitHub Models API via GITHUB_TOKEN
```

### Required secrets
- `COPILOT_TOKEN` — PAT with `copilot` scope (for GitHub Copilot Enterprise endpoint)
- `secrets.GITHUB_TOKEN` — auto-injected; used for GitHub Models API and PR creation

> `SONARQUBE_TOKEN` is no longer a pre-configured secret. It is generated at runtime during the bootstrap step using the default `admin:admin` credentials of the ephemeral container, then passed as an env var to subsequent steps.

### Service container

```yaml
services:
  sonarqube:
    image: sonarqube:lts-community
    ports:
      - 9000:9000
    options: >-
      --health-cmd "curl -f http://localhost:9000/api/system/status || exit 1"
      --health-interval 10s
      --health-timeout 5s
      --health-retries 18
```

Uses the embedded H2 database — no PostgreSQL needed since data does not need to persist between runs.

### Job structure (single job, `ubuntu-latest`)

```
Step 1:  actions/checkout@v4 (fetch-depth: 0 for full history)
Step 2:  actions/setup-python@v5 (python-version: '3.12')
Step 3:  actions/setup-java@v4 (java-version: '17')
Step 4:  dotnet tool install --global dotnet-sonarscanner
Step 5:  pip install requests
Step 6:  Bootstrap: wait for SonarQube UP → generate token → create project keys → set SONARQUBE_TOKEN env var
Step 7:  dotnet sonarscanner begin (project keys, host=http://localhost:9000, login=SONARQUBE_TOKEN)
Step 8:  dotnet build
Step 9:  dotnet sonarscanner end
Step 10: Configure git identity (github-actions[bot])
Step 11: Create branch ai/sonarqube-fixes-{run_number}
Step 12: python scripts/sonar/fetch_issues.py  → output/issues.json
Step 13: python scripts/sonar/batch_issues.py  → output/batches.json
Step 14: Upload output/issues.json + output/batches.json as artifacts
Step 15: (if not dry_run) Loop: python scripts/ai/fix_batch.py --batch-id N
Step 16: (if not dry_run) python scripts/git/create_pr.py
Step 17: Upload output/fix_results/ as artifact
```

---

## 2. `scripts/sonar/fetch_issues.py`

Paginates `/api/issues/search` across all project keys × severities.

### 10k cap strategy (hierarchical decomposition)
1. Query with `(project_key, severity)` → check `paging.total`
2. If ≤ 10,000: paginate normally (500/page, up to 20 pages)
3. If > 10,000: decompose by `types` (BUG, VULNERABILITY, CODE_SMELL)
4. If still > 10,000: decompose by top-level directory via `/api/components/tree`

Given ~7,574 total issues (all languages) with a `language=cs` filter, Level 1 will handle this in practice. Levels 2–3 are defensive.

### Auth
```python
session = requests.Session()
session.auth = (SONARQUBE_TOKEN, "")  # token as username, empty password
```

### Output: `output/issues.json`
```json
{
  "fetched_at": "...", "total_fetched": 1823,
  "issues": [
    {
      "key": "AXmJ...", "rule": "csharpsquid:S1481", "severity": "MAJOR",
      "component": "ACSI-CSES-VisorNominaRetenedor:Proyectos/Cloud/.../HomeController.cs",
      "project": "ACSI-CSES-VisorNominaRetenedor",
      "line": 42, "message": "Remove the unused local variable 'resultado'.",
      "status": "OPEN", "type": "CODE_SMELL", "language": "cs"
    }
  ]
}
```

---

## 3. `scripts/sonar/batch_issues.py`

Groups issues by `component` (file), maps to local workspace path, sorts by severity, and creates batches of N files.

### Component → local path mapping
```python
# "ACSI-CSES-VisorNominaRetenedor:Proyectos/Cloud/.../HomeController.cs"
# → "Proyectos/Cloud/.../HomeController.cs"
local_path = component[component.index(":") + 1:]
```
Files missing from workspace (renamed/deleted) are skipped with `status: not_found`.

### Severity score for sorting
`BLOCKER=5, CRITICAL=4, MAJOR=3, MINOR=2, INFO=1`
Each file's score = max severity score of its issues. Files sorted descending.

### Token estimation heuristic
`estimated_tokens = len(file_content_utf8) // 4`
Files >3,000 estimated tokens get `large_file: true` — the AI script will handle them with a warning rather than skipping.

### Output: `output/batches.json`
```json
{
  "total_files_with_issues": 234, "total_batches": 47,
  "batches": [
    {
      "batch_id": 0,
      "files": [
        {
          "component": "ACSI-CSES-...:Proyectos/Cloud/.../HomeController.cs",
          "local_path": "Proyectos/Cloud/.../HomeController.cs",
          "max_severity": "CRITICAL", "issue_count": 7, "large_file": false,
          "issues": [
            {"line": 95, "rule": "csharpsquid:S112",
             "message": "'System.Exception' should not be thrown.", "severity": "CRITICAL"}
          ]
        }
      ]
    }
  ]
}
```

---

## 4. `scripts/ai/fix_batch.py`

Called once per batch. For each file: reads content, calls AI API, validates response, writes file, `git add` + `git commit`.

### AI API — try Copilot Enterprise first, fall back to GitHub Models

```python
COPILOT_URL  = "https://api.githubcopilot.com/chat/completions"
COPILOT_HDR  = {"Authorization": f"Bearer {COPILOT_TOKEN}",
                "Copilot-Integration-Id": "vscode-chat"}

MODELS_URL   = "https://models.inference.ai.azure.com/chat/completions"
MODELS_HDR   = {"Authorization": f"Bearer {GH_TOKEN}"}

MODEL = "gpt-4o"
```

Try Copilot; on HTTP 401/403 fall back to GitHub Models. On 429 retry with `retry-after` header (exponential backoff, max 3 retries). Delay 3 s between files.

### System prompt (sent to AI)
```
You are an expert C# .NET Framework 4.8 developer performing SonarQube code quality remediation.

Rules:
1. Fix ONLY the reported SonarQube issues and nothing else.
2. Do NOT add features, refactor beyond the issue, or change behavior.
3. Preserve all existing comments, XML docs, and whitespace style.
4. Return ONLY the complete fixed file content — no markdown fences, no explanations.
5. Output must be valid C# that compiles under .NET Framework 4.8.
6. If a fix is ambiguous or risky (e.g. changing a public API), leave that issue unfixed.
```

### User prompt structure
```
File: {local_path}
Project: {project_key}

## SonarQube Issues ({issue_count}):
Line 95  [CRITICAL] csharpsquid:S112  — 'System.Exception' should not be thrown.
Line 104 [MAJOR]    csharpsquid:S1481 — Remove unused variable 'resultado'.

## Current File Content:
{file_content}
```

### Response validation (heuristic — no MSBuild compile)

| Check | Rejection reason |
|-------|-----------------|
| Response < 50 chars | `response_too_short` |
| `{` count ≠ `}` count | `unbalanced_braces` |
| Original namespace declaration missing | `namespace_removed` |
| Size ratio vs original > 2.5× or < 0.3× | `size_ratio_suspicious` |
| Contains ` ``` ` | `contains_markdown_fences` |

If validation fails, the original file is NOT overwritten. Status recorded as `skipped`.

### Git commit per successfully fixed file
```
fix(sonar): auto-fix N issue(s) in HomeController.cs

Component: ACSI-CSES-VisorNominaRetenedor:Proyectos/Cloud/.../HomeController.cs
Batch: 0
Workflow run: {GITHUB_RUN_ID}
Tool: SonarQube AI Auto-Fix
```

### Output: `output/fix_results/batch_000.json`
```json
{
  "batch_id": 0,
  "results": [
    {"component": "...", "local_path": "...", "status": "fixed",
     "validation_result": "ok", "commit_sha": "abc1234", "skip_reason": null},
    {"component": "...", "local_path": "...", "status": "skipped",
     "validation_result": "unbalanced_braces", "commit_sha": null,
     "skip_reason": "ai_returned_invalid_csharp"}
  ]
}
```

Exit codes: `0` = at least one file fixed, `2` = all files skipped (non-fatal), `3` = fatal (API down).

---

## 5. `scripts/git/create_pr.py`

- Aggregates all `fix_results/batch_*.json`
- Pushes the branch: `git push -u origin ai/sonarqube-fixes-{run_number}`
- Creates PR via `POST https://api.github.com/repos/{repo}/pulls`
- Base branch: `main`; draft: `false`

### PR title
```
[AI] SonarQube fix: {N} issues across {M} files ({severities}) — run #{run_number}
```

### PR body includes
- Fix statistics table (files fixed, files skipped, issues addressed, total commits)
- Table of skipped files with skip reasons
- Review notes (each commit = one file; re-run SonarQube pipeline after merge)
- Link back to the workflow run

---

## 6. `scripts/shared/constants.py`

Shared config for all scripts:
- `SEVERITY_SCORE`, `SEVERITY_ORDER`
- `TOKEN_CHARS_PER_TOKEN = 4`, `MAX_TOKENS_PER_FILE = 3000`
- `SONAR_PAGE_SIZE = 500`, `SONAR_MAX_OFFSET = 10000`
- `EXIT_CODE_SUCCESS = 0`, `EXIT_CODE_ALL_SKIPPED = 2`, `EXIT_CODE_FATAL = 3`

---

## Verification / Testing Order

1. **Dry run first**: trigger with `dry_run: true` — inspect `output/issues.json` and `output/batches.json` artifacts to confirm SonarQube connectivity and batching
2. **Single-batch smoke test**: set `max_files: 5`, `dry_run: false`, `severities: MINOR` — verify one PR is created with 5 file commits
3. **Spot-check fixes**: open the PR, compare commits against originals, confirm plausible C# changes
4. **Merge and re-scan**: merge a batch PR, re-run SonarQube scan via existing Azure Pipelines to measure issue reduction
5. **Full run**: once confident, run without `max_files` cap targeting `BLOCKER,CRITICAL,MAJOR`

---

## Key Decisions

- **One PR per workflow run** — easier for team; distinct branch per run; no force-push ever
- **Full-file replacement, not diff** — simpler to apply; validated with heuristics
- **No MSBuild compile in fix loop** — too slow; post-merge SonarQube scan is the gate
- **SonarQube as service container (Option A)**: ephemeral, no persistent instance needed; project is created and token is generated fresh each run using default `admin:admin` credentials; H2 database used since no data persistence is required
- **Option B (not chosen)**: persistent SonarQube + self-hosted runner; project created once, token stored as secret; preferred if historical tracking or run speed matters
- **`output/` directory**: add to `.gitignore` to prevent accidental commits of intermediate files
- **AI fallback order**: GitHub Copilot Enterprise → GitHub Models (gpt-4o); configured via secrets
