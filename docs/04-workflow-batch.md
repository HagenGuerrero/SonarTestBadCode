---
title: Workflow — sonar-batch.yml (Scan & Batch)
tags: [workflow, github-actions, sonar-batch, scan, batch, automation]
aliases: [batch-workflow, sonar-batch, scan-workflow]
---

# Workflow — `sonar-batch.yml` (Scan & Batch)

**File:** `.github/workflows/sonar-batch.yml`
**Name:** SonarQube — Scan & Batch Issues

---

## Triggers

| Event | Branch | Notes |
|-------|--------|-------|
| `push` | `dev` | Automatic — fires on every commit to `dev` |
| `workflow_dispatch` | any | Manual — allows overriding all inputs |

---

## Concurrency

```yaml
concurrency:
  group: sonar-batch-${{ github.ref }}
  cancel-in-progress: true
```

Only one run per branch is allowed at a time. A new push cancels any in-flight run for the same branch, preventing race conditions on the output files.

---

## Permissions

```yaml
permissions:
  contents: write      # push the PR branch
  pull-requests: write # open the PR to agent-queue
```

---

## Inputs (workflow_dispatch only)

When triggered by `push`, all inputs fall back to their defaults automatically.

| Input | Default | Description |
|-------|---------|-------------|
| `project_keys` | `sonar-test-bad-code` | Comma-separated SonarQube project keys to scan and fetch |
| `severity_blocker` | `true` | Include BLOCKER issues |
| `severity_critical` | `true` | Include CRITICAL issues |
| `severity_major` | `true` | Include MAJOR issues |
| `severity_minor` | `true` | Include MINOR issues |
| `severity_info` | `true` | Include INFO issues |
| `language_filter` | `cs` | SonarQube language key |
| `max_files` | `0` (unlimited) | Cap on files to batch; 0 = no cap |
| `quality_profile_path` | `sonar/quality-profile-cs.xml` | Repo-relative path to the quality profile XML |
| `solution_path` | `SonarTestBadCode.sln` | Repo-relative path to the `.sln` or `.csproj` to build |

> [!TIP]
> When triggered by `push`, boolean severity inputs default to `true` via shell parameter expansion (`${VAR:-true}`), meaning all severities are included automatically.

---

## Infrastructure

The job runs on `ubuntu-latest` with a SonarQube service container:

```yaml
services:
  sonarqube:
    image: sonarqube:lts-community
    ports:
      - 9000:9000
```

This is an ephemeral instance — H2 embedded database, no persistence between runs.

---

## Steps

### 1. Checkout
Full history (`fetch-depth: 0`) to ensure git can create and push branches.

### 2. Set up Python 3.12
Required for `fetch_issues.py` and `batch_issues.py`.

### 3. Set up Java 17
Required by the SonarScanner backend.

### 4. Install dotnet-sonarscanner
```bash
dotnet tool install --global dotnet-sonarscanner
```

### 5. Install Python dependencies
```bash
pip install requests
```

### 6. Install Mono MSBuild
Required to build `.NET Framework 4.8` projects on Linux. Installed from the official Mono Project repo because Ubuntu's apt package does not include `mono-msbuild`.

### 7. Wait for SonarQube to be fully UP
Polls `GET /api/system/status` every 10 seconds, up to 30 attempts. Fails the workflow if SonarQube never reaches `"status": "UP"`.

### 8. Bootstrap — generate token and create project
- Creates a SonarQube project for each key in `project_keys`
- Generates a CI token via `POST /api/user_tokens/generate`
- Writes `SONARQUBE_TOKEN` to `$GITHUB_ENV`

Uses default `admin:admin` credentials on the fresh ephemeral instance.

### 9. Import quality profile
- Renames "Sonar way" to `CI-Profile-{language}` in the XML (the built-in profile is read-only)
- Restores it via `POST /api/qualityprofiles/restore`
- Sets it as default for the language

Profile file: `sonar/quality-profile-cs.xml` (or the value of `quality_profile_path` input).

### 10. SonarScanner begin
Uses the **first** project key only (the scanner registers one project per analysis). Excludes `scripts/**` and `output/**` from analysis.

### 11. Build
```bash
dotnet msbuild <solution_path> /p:Configuration=Release
```
`FrameworkPathOverride` is set to `/usr/lib/mono/4.8-api/` for .NET Framework 4.8 compatibility on Linux.

### 12. SonarScanner end
Submits the analysis to the SonarQube service container.

### 13. Wait for SonarQube CE task to complete
Polls the CE task URL from `.sonarqube/out/.sonar/report-task.txt` every 5 seconds, up to 36 attempts. Fails on `FAILED` status.

### 14. Build severity filter
Constructs the `SONAR_SEVERITIES` comma-separated string from the boolean inputs.
When triggered by `push`, empty inputs default to `true` via `${VAR:-true}`.

### 15. Fetch issues
```bash
python scripts/sonar/fetch_issues.py
```
Reads env vars: `SONARQUBE_URL`, `SONAR_PROJECT_KEYS`, `SONAR_SEVERITIES`, `SONAR_LANGUAGE`.
Writes: `output/issues.json`

### 16. Batch issues by file
```bash
python scripts/sonar/batch_issues.py
```
Reads env vars: `WORKSPACE_ROOT`, `MAX_FILES`, `SCAN_COMMIT_SHA`.
Writes: `output/batches.json` (includes the scanned commit SHA for downstream validation).

### 17. Configure git identity
```bash
git config user.name "github-actions[bot]"
git config user.email "github-actions[bot]@users.noreply.github.com"
```

### 18. Ensure agent-queue branch exists
Checks `git ls-remote` for `refs/heads/agent-queue`. Creates and pushes it if absent (first run only).

### 19. Commit output files and open PR to agent-queue
- Creates branch `sonar/batch-{run_number}` from the current scanned HEAD
- Force-adds `output/issues.json` and `output/batches.json` (gitignored, so `-f` is required)
- Commits with message: `ci: sonar batch — run #{N} @ {sha}`
- Pushes the branch
- Opens PR: `sonar/batch-{N}` → `agent-queue`

The PR branch carries **both the latest code from `dev` and the batch files**, so merging it into `agent-queue` gives the fix workflow everything it needs without a re-scan.

### 20. Write step summary
Appends a summary table to the GitHub Actions step summary including issue count, batch count, severity filter, and PR URL.

---

## Output

| Artifact | Description |
|----------|-------------|
| `output/issues.json` | Raw SonarQube issues from the REST API |
| `output/batches.json` | Issues grouped by file with `scan_commit` SHA |
| PR to `agent-queue` | Branch `sonar/batch-{N}` ready for review and merge |

---

## Environment variables set during the run

| Variable | Set by step | Consumed by step |
|----------|-------------|-----------------|
| `SONARQUBE_TOKEN` | Bootstrap | SonarScanner begin/end, fetch issues |
| `NOTIFY_TO` | (removed) | — |
| `PR_URL` | Commit & PR step | Write step summary |
| `ISSUES_COUNT` | Commit & PR step | Write step summary |
| `BATCHES_COUNT` | Commit & PR step | Write step summary |

---

## Required secrets

See [[09-secrets-and-config]] for the full list. This workflow only needs `GITHUB_TOKEN` (auto-injected).

---

## Related documents

- [[01-architecture]] — where this fits in the full pipeline
- [[07-scripts]] — `fetch_issues.py` and `batch_issues.py` internals
- [[11-branch-strategy]] — `agent-queue` branch and PR flow
- [[09-secrets-and-config]] — secrets reference
