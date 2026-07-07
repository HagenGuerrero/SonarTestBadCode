# SonarTestBadCode

A .NET Framework 4.8 class library intentionally written with code quality violations. It is the scan target for an AI-powered GitHub Actions pipeline that automatically remediates SonarQube findings and opens a pull request with the fixes.

> **Do not fix the violations manually.** They are fixture data — the input the workflow operates on.

---

## How it works

```
push to dev
    │
    ▼
sonar-batch.yml          ← scans, batches findings, opens PR to agent-queue
    │
    │  merge PR into agent-queue
    ▼
sonar-fix-only.yml       ← AI fix loop, opens fix PR  (lightweight — no re-scan)
    │
    ▼
sonar-verify.yml         ← build check on every fix PR
```

Full architecture and design decisions: [`docs/01-architecture.md`](docs/01-architecture.md)

---

## Project structure

```
.github/workflows/
  sonar-batch.yml          triggered on push to dev — scan + batch + PR to agent-queue
  sonar-fix-only.yml       triggered manually from agent-queue — AI fix loop only
  sonar-fix-v3.yml         all-in-one alternative (scan + batch + fix in one dispatch)
  sonar-verify.yml         build verification on fix PRs
scripts/
  shared/constants.py      shared config — no magic strings
  sonar/fetch_issues.py    SonarQube REST API paginator → output/issues.json
  sonar/batch_issues.py    groups issues by file → output/batches.json
  ai/fix_batch.py          AI fix loop — calls API, validates, writes files, commits
  ai/prompts/system_prompt.md   behavioral rules and output format for the AI
  ai/prompts/agent.md           per-rule fix strategies and guardrails
  ai/create_copilot_tasks.py    fires Copilot Agent Tasks for skipped files
  git/create_pr.py         pushes fix branch, opens aggregated PR
sonar/
  quality-profile-cs.xml   custom SonarQube quality profile (27 rules)
docs/                      full RAG documentation — see below
SonarTestBadCode/
  Controllers/HomeController.cs      (~68 findings)
  Services/DataService.cs            (~74 findings)
  Models/UserModel.cs                (~70 findings)
  Utilities/StringHelper.cs          (~63 findings)
  Repositories/UserRepository.cs     (~79 findings)
  Helpers/ValidationHelper.cs        (~70 findings)
  ProcessorClass.cs                  (~57 findings)
```

---

## Workflows

### `sonar-batch.yml` — Scan & Batch (auto on push to `dev`)

Triggers on every push to `dev`. Scans with an ephemeral SonarQube container, batches findings by file, and opens a PR from `sonar/batch-{N}` to `agent-queue`. The PR branch carries both the latest code and the pre-built batch files so the fix workflow needs no re-scan.

| Input | Default | Description |
|-------|---------|-------------|
| `project_keys` | `sonar-test-bad-code` | SonarQube project key(s), comma-separated |
| `severity_blocker/critical/major/minor/info` | `true` | Severity filters (all default true on push) |
| `language_filter` | `cs` | SonarQube language key |
| `max_files` | `0` | Max files to batch (0 = unlimited) |
| `quality_profile_path` | `sonar/quality-profile-cs.xml` | Path to quality profile XML |
| `solution_path` | `SonarTestBadCode.sln` | Path to `.sln` or `.csproj` to build |

**Output:** PR `sonar/batch-{N}` → `agent-queue` with `output/issues.json` + `output/batches.json` committed.

---

### `sonar-fix-only.yml` — AI Fix Only (manual, from `agent-queue`)

The recommended fix workflow. Reads `output/batches.json` already committed on `agent-queue`, skips the scan entirely, and runs the AI fix loop directly. Startup time ~1 minute vs ~15 minutes for the full pipeline.

**Prerequisite:** merge a `sonar/batch-{N}` PR into `agent-queue` first.

| Input | Default | Description |
|-------|---------|-------------|
| `max_issues_per_call` | `15` | Issues per AI call — lower reduces hallucination risk |
| `base_branch` | `agent-queue` | PR target branch for the fix PR |
| `run_copilot_agent` | `false` | Fire Copilot Agent Tasks for skipped files |

**Output:** PR `ai/sonarqube-fixes-{N}` → `agent-queue` with one commit per fixed file.

---

### `sonar-fix-v3.yml` — Full Pipeline (manual, from `agent/fix`)

All-in-one alternative: scan → batch → fix in a single dispatch, split across three dependent jobs. Use this when you need a fresh scan without a prior batch PR.

| Input | Default | Description |
|-------|---------|-------------|
| `project_keys` | `sonar-test-bad-code` | SonarQube project key(s) |
| `severity_*` | `true` | Severity filters |
| `language_filter` | `cs` | Language key |
| `max_files` | `0` | Batch cap |
| `max_issues_per_call` | `15` | Sub-batching cap |
| `run_copilot_agent` | `false` | Fire Copilot Agent Tasks for skipped files |

---

### `sonar-verify.yml` — Build Verification (auto on fix PRs)

Triggers on every PR targeting `agent/fix`. Runs `dotnet msbuild` to confirm AI fixes compile. Read-only — no commits.

---

## Required secrets

| Secret | Required | Description |
|--------|----------|-------------|
| `GITHUB_TOKEN` | Auto | Built-in Actions token — GitHub Models fallback + PR/git operations |
| `COPILOT_TOKEN` | Recommended | GitHub PAT with `copilot` scope — primary AI API (Copilot Enterprise) |
| `COPILOT_PAT` | Optional | Fine-grained PAT with `actions:write`, `contents:write`, `issues:write`, `pull-requests:write` — required for Copilot Agent Tasks |

`SONARQUBE_TOKEN` is **not** a pre-configured secret — it is generated at runtime from the ephemeral container's default `admin:admin` credentials.

---

## AI provider chain

```
GitHub Copilot Enterprise   →   GitHub Models (GPT-4o)
     (COPILOT_TOKEN)               (GITHUB_TOKEN)
  on 401/403: fall back          on 429: retry with backoff
```

Temperature: `0.2`. Max retries: `3`. Delay between files: `3s`.

---

## Branch strategy

| Branch | Purpose |
|--------|---------|
| `dev` | Active development — pushes trigger `sonar-batch.yml` |
| `agent-queue` | Staging — receives batch PRs; fix workflow runs from here |
| `sonar/batch-{N}` | Ephemeral — created per batch run, carries code + batch files |
| `ai/sonarqube-fixes-{N}` | Ephemeral — created per fix run, carries AI-fixed files |
| `agent/fix` | Legacy base for `sonar-fix-v3.yml` and `sonar-verify.yml` |

> `agent` cannot be used as a branch name — `agent/fix` and `agent/batching` already exist in the `agent/*` namespace, which is a Git ref path conflict.

Full branch flow: [`docs/11-branch-strategy.md`](docs/11-branch-strategy.md)

---

## SonarQube rules violated (27 rules)

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
| S2583 | Boolean expression always `false` |
| S2589 | Boolean expression always `true` |
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

Rules S107, S134, S138, S1541, S3358, S3776, S4144 are always skipped by the AI fix loop — they require architectural reasoning beyond what a single-file context allows.

Full rule reference with fix strategies: [`docs/10-sonar-rules.md`](docs/10-sonar-rules.md)

---

## SonarQube version

Targets **SonarQube 9.9.x LTS**.

| Do | Don't |
|----|-------|
| `/d:sonar.login="TOKEN"` | `/d:sonar.token="TOKEN"` — 10.x syntax, fails on 9.9.x |
| `projectKeys=KEY` in REST API | `componentKeys=KEY` — returns incomplete results on 9.9.x |
| `auth=(token, "")` in Python | — |

---

## Prerequisites

- .NET Framework 4.8 SDK / MSBuild
- Python 3.12+ with `requests`
- Docker Desktop (for local SonarQube)
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

```powershell
# 1. Start SonarQube
cd C:\repos\sonarqube && docker compose up -d
# Open http://localhost:9000  login: admin / Admin1234!

# 2. Generate a token
curl.exe -u admin:Admin1234! -X POST "http://localhost:9000/api/user_tokens/generate" -d "name=test-token"

# 3. Create the project (first time only)
curl.exe -u admin:Admin1234! -X POST "http://localhost:9000/api/projects/create" `
  -d "project=sonar-test-bad-code&name=SonarTestBadCode"

# 4. Scan
cd C:\repos\SonarTestBadCode
dotnet sonarscanner begin /k:"sonar-test-bad-code" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="YOUR-TOKEN"
dotnet build
dotnet sonarscanner end /d:sonar.login="YOUR-TOKEN"
```

Full command reference: [`docs/02-sonarqube-local-setup.md`](docs/02-sonarqube-local-setup.md)

---

## Running scripts locally

```powershell
# Fetch issues
$env:SONARQUBE_URL      = "http://localhost:9000"
$env:SONARQUBE_TOKEN    = "YOUR-TOKEN"
$env:SONAR_PROJECT_KEYS = "sonar-test-bad-code"
$env:SONAR_SEVERITIES   = "BLOCKER,CRITICAL,MAJOR,MINOR,INFO"
$env:SONAR_LANGUAGE     = "cs"
python scripts/sonar/fetch_issues.py        # → output/issues.json

# Batch issues
$env:WORKSPACE_ROOT = "C:\repos\SonarTestBadCode"
$env:MAX_FILES      = "0"
python scripts/sonar/batch_issues.py        # → output/batches.json

# AI fix loop
$env:GITHUB_TOKEN        = "YOUR-GITHUB-TOKEN"
$env:COPILOT_TOKEN       = "YOUR-COPILOT-PAT"
$env:MAX_ISSUES_PER_CALL = "15"
$env:WORKSPACE_ROOT      = "C:\repos\SonarTestBadCode"
$env:GITHUB_RUN_ID       = "local"
python scripts/ai/fix_batch.py
```

---

## Documentation

Full RAG documentation is in [`docs/`](docs/) — structured for Copilot and Obsidian.

| Doc | Contents |
|-----|----------|
| [`docs/01-architecture.md`](docs/01-architecture.md) | End-to-end data flow, design decisions |
| [`docs/04-workflow-batch.md`](docs/04-workflow-batch.md) | `sonar-batch.yml` step-by-step |
| [`docs/05-workflow-fix.md`](docs/05-workflow-fix.md) | `sonar-fix-v3.yml` three-job structure |
| [`docs/07-scripts.md`](docs/07-scripts.md) | All scripts — APIs, env vars, output schemas |
| [`docs/09-secrets-and-config.md`](docs/09-secrets-and-config.md) | Every secret and input explained |
| [`docs/12-hallucination-mitigation.md`](docs/12-hallucination-mitigation.md) | AI validation chain |
| [`docs/13-troubleshooting.md`](docs/13-troubleshooting.md) | Known errors and fixes |
