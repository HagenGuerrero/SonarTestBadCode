# SonarTestBadCode

A .NET Framework 4.8 class library intentionally written with code quality violations. It serves as the SonarQube scan target for an AI auto-fix GitHub Actions workflow — the violations are input data, not bugs.

**Do not fix the violations manually.** They are the fixture data the workflow operates on.

---

## Purpose

Produces a predictable set of ~400 SonarQube findings across 7 files. Two GitHub Actions workflows operate on this project:

- **`sonar-batch.yml`** (`agent/batching` branch) — scans, fetches findings, batches by file, opens a review PR with the raw output.
- **`sonar-fix.yml`** (`agent/fix` branch) — full pipeline: scan → batch → AI fix loop → PR with all fixes committed.

---

## Project structure

```
SonarTestBadCode.sln
SonarTestBadCode/
  Controllers/HomeController.cs      (~68 findings)
  Services/DataService.cs            (~74 findings)
  Models/UserModel.cs                (~70 findings)
  Utilities/StringHelper.cs          (~63 findings)
  Repositories/UserRepository.cs     (~79 findings)
  Helpers/ValidationHelper.cs        (~70 findings)
  ProcessorClass.cs                  (~57 findings)
scripts/
  shared/constants.py                shared config and exit codes
  sonar/fetch_issues.py              SonarQube REST API paginator
  sonar/batch_issues.py              groups issues into per-file batches
  ai/fix_batch.py                    AI fix loop — calls API, writes fixes, commits
  ai/prompts/system_prompt.md        behavioral rules and output format for the AI
  ai/prompts/agent.md                per-rule fix strategies and guardrails
  ai/create_copilot_tasks.py         fires Copilot Agent Tasks for skipped files
  git/create_pr.py                   pushes fix branch, opens aggregated PR
sonar/
  quality-profile-cs.xml             SonarQube quality profile with all 28 rules
```

---

## Workflows

### `sonar-batch.yml` — Scan & Batch

Triggered manually from the `agent/batching` branch.

| Input | Default | Description |
|-------|---------|-------------|
| `project_keys` | `sonar-test-bad-code` | SonarQube project key(s), comma-separated |
| `severity_blocker` | `true` | Include BLOCKER issues |
| `severity_critical` | `true` | Include CRITICAL issues |
| `severity_major` | `true` | Include MAJOR issues |
| `severity_minor` | `true` | Include MINOR issues |
| `severity_info` | `true` | Include INFO issues |
| `language_filter` | `cs` | SonarQube language key |
| `max_files` | `0` | Max files to batch (0 = unlimited) |

**Output:** `output/issues.json` + `output/batches.json` committed to a `sonar/batch-output-N` branch with a PR targeting `agent/batching`.

---

### `sonar-fix.yml` — Scan, Batch & AI Auto-Fix

Triggered manually from the `agent/fix` branch.

Includes all inputs from `sonar-batch.yml` plus:

| Input | Default | Description |
|-------|---------|-------------|
| `max_issues_per_call` | `15` | Max issues per AI API call (sub-batching) |
| `run_copilot_agent` | `false` | Fire Copilot Agent Tasks for skipped files |

**Output:** Source files fixed in-place, one git commit per file with full fix log in the commit message. An aggregated PR targeting `agent/fix` includes a summary table (fixed/skipped files, token usage).

#### Required secrets

| Secret | Required | Description |
|--------|----------|-------------|
| `GITHUB_TOKEN` | Auto | Built-in Actions token — GitHub Models fallback |
| `COPILOT_TOKEN` | Recommended | Classic PAT with `copilot` scope — unlocks full GPT-4o context (128k) via `api.githubcopilot.com` |
| `COPILOT_PAT` | Optional | Classic PAT with `repo` + `workflow` scopes (Copilot-enabled account) — required for the Copilot Agent Tasks step |

#### Provider chain

`fix_batch.py` tries providers in order, falling back on failure:

```
GitHub Copilot API  →  GitHub Models (gpt-4o-mini, free tier)
(COPILOT_TOKEN)         (GITHUB_TOKEN, 8k token cap)
```

#### Sub-batching

Files with more issues than `max_issues_per_call` are split into sequential chunks. Each chunk reads the already-modified file so line numbers stay accurate across chunks.

#### Copilot Agent Tasks (optional)

When `run_copilot_agent` is enabled, files that the AI fix loop marks as `skipped` are sent to the GitHub Copilot coding agent via the Agent Tasks REST API (`POST /agents/repos/{owner}/{repo}/tasks`). Each task targets `agent/fix` as its base branch and opens its own draft PR. Requires `COPILOT_PAT` and Copilot coding agent enabled for the repo.

---

## SonarQube rules violated (28 rules)

| Rule | Description |
|------|-------------|
| S112 | `System.Exception` should not be thrown |
| S125 | Commented-out code |
| S1066 | Collapsible `if` statements |
| S1116 | Empty statements (`;;`) |
| S1118 | Utility class with public constructor |
| S1144 | Unused private members |
| S1172 | Unused method parameters |
| S1186 | Empty method bodies |
| S1192 | Duplicate string literals |
| S1481 | Unused local variables |
| S1643 | String concatenation in a loop |
| S1764 | Identical expressions on both sides of an operator |
| S1871 | Two branches with identical implementation |
| S2221 | Catching `Exception` too broadly |
| S2386 | Mutable `public static` fields |
| S2583 | Boolean expression is always `false` |
| S2589 | Boolean expression is always `true` |
| S2696 | Instance method writes to a `static` field |
| S3400 | Method returns only a constant |
| S3717 | `NotImplementedException` thrown |
| S3963 | Static field initialized to its default value |
| S107 | Too many parameters |
| S134 | Control flow nested too deeply |
| S138 | Method has too many lines |
| S1541 | Cyclomatic complexity too high |
| S3358 | Nested ternary operators |
| S3776 | Cognitive complexity too high |
| S4144 | Two methods with identical implementations |

Rules S107, S134, S138, S1541, S3358, S3776, and S4144 are intentionally present to exercise the AI's hallucination-mitigation path — they require structural reasoning and are skipped by the fix loop, falling through to the Copilot Agent Tasks step.

---

## SonarQube version

Targets **SonarQube 9.9.x LTS**.

| Do | Don't |
|----|-------|
| `/d:sonar.login="TOKEN"` | `/d:sonar.token="TOKEN"` — 10.x syntax |
| `projectKeys=KEY` in REST API | `componentKeys=KEY` — returns incomplete results on 9.9.x |
| `auth=(token, "")` in Python requests | No other auth formats needed |

---

## Prerequisites

- .NET Framework 4.8 SDK / MSBuild
- Python 3.12+ with `requests`
- Docker Desktop (for local SonarQube 9.9.x)
- `dotnet-sonarscanner` global tool

```powershell
dotnet tool install --global dotnet-sonarscanner
pip install requests
```

---

## Build

```powershell
msbuild SonarTestBadCode.sln /p:Configuration=Release
```

---

## Running a local SonarQube scan

### 1. Start SonarQube

```powershell
cd C:\repos\sonarqube
docker compose up -d
```

Open http://localhost:9000 — login: `admin` / `Admin1234!`

### 2. Generate a token

```powershell
curl.exe -u admin:Admin1234! -X POST "http://localhost:9000/api/user_tokens/generate" -d "name=test-token"
```

### 3. Create the project (first time only)

```powershell
curl.exe -u admin:Admin1234! -X POST "http://localhost:9000/api/projects/create" `
  -d "project=sonar-test-bad-code&name=SonarTestBadCode"
```

### 4. Scan

```powershell
cd C:\repos\SonarTestBadCode

dotnet sonarscanner begin /k:"sonar-test-bad-code" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="YOUR-TOKEN"
dotnet build
dotnet sonarscanner end /d:sonar.login="YOUR-TOKEN"
```

---

## Running the scripts locally

### Fetch issues

```powershell
$env:SONARQUBE_URL      = "http://localhost:9000"
$env:SONARQUBE_TOKEN    = "YOUR-TOKEN"
$env:SONAR_PROJECT_KEYS = "sonar-test-bad-code"
$env:SONAR_SEVERITIES   = "BLOCKER,CRITICAL,MAJOR,MINOR,INFO"
$env:SONAR_LANGUAGE     = "cs"

python scripts/sonar/fetch_issues.py
# Output: output/issues.json
```

### Batch issues

```powershell
$env:WORKSPACE_ROOT = "C:\repos\SonarTestBadCode"
$env:MAX_FILES      = "0"

python scripts/sonar/batch_issues.py
# Output: output/batches.json
```

### Run the AI fix loop

```powershell
$env:GITHUB_TOKEN        = "YOUR-GITHUB-TOKEN"
$env:COPILOT_TOKEN       = "YOUR-COPILOT-PAT"   # optional
$env:MAX_ISSUES_PER_CALL = "15"
$env:WORKSPACE_ROOT      = "C:\repos\SonarTestBadCode"
$env:GITHUB_RUN_ID       = "local"

python scripts/ai/fix_batch.py
```

---

## Reference

- `plan.md` — original architecture notes for the scan + batch workflow
- `ghAPIPlan.md` — AI auto-fix pipeline design (API strategy, scaling, hallucination mitigation)
- `COMMANDS.md` — quick reference for SonarQube local instance commands
