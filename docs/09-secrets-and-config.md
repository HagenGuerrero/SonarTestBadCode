---
title: Secrets, Environment Variables & Configuration
tags: [secrets, config, environment-variables, github-actions, setup]
aliases: [secrets, config, env-vars, configuration]
---

# Secrets, Environment Variables & Configuration

---

## GitHub Actions secrets

Configure these in the repository settings under **Settings → Secrets and variables → Actions**.

### Required for the AI fix workflow

| Secret | Used by | Description |
|--------|---------|-------------|
| `COPILOT_TOKEN` | `fix_batch.py` | GitHub PAT with `copilot` scope. Primary AI API. If absent, the workflow falls back to GitHub Models automatically |
| `GITHUB_TOKEN` | All workflows | Auto-injected by GitHub Actions. No manual setup needed. Used for GitHub Models fallback, git operations, and PR creation via `gh` |

### Required for Copilot Agent Tasks (optional feature)

| Secret | Used by | Description |
|--------|---------|-------------|
| `COPILOT_PAT` | `create_copilot_tasks.py` | Fine-grained PAT with `actions:write`, `contents:write`, `issues:write`, `pull-requests:write`. Cannot be `GITHUB_TOKEN` — billing is user-level |

### Not required (tokens are generated at runtime)

The `SONARQUBE_TOKEN` does **not** need to be a pre-configured secret. It is generated dynamically during the bootstrap step using the default `admin:admin` credentials of the ephemeral SonarQube container, then passed between steps via `$GITHUB_ENV`.

---

## Workflow inputs — `sonar-batch.yml`

All inputs are optional. When triggered by `push` to `dev`, defaults apply automatically.

| Input | Default | Type | Description |
|-------|---------|------|-------------|
| `project_keys` | `sonar-test-bad-code` | string | Comma-separated SonarQube project keys |
| `severity_blocker` | `true` | boolean | Include BLOCKER issues |
| `severity_critical` | `true` | boolean | Include CRITICAL issues |
| `severity_major` | `true` | boolean | Include MAJOR issues |
| `severity_minor` | `true` | boolean | Include MINOR issues |
| `severity_info` | `true` | boolean | Include INFO issues |
| `language_filter` | `cs` | string | SonarQube language key |
| `max_files` | `0` | string | Max files to batch; `0` = unlimited |
| `quality_profile_path` | `sonar/quality-profile-cs.xml` | string | Repo-relative path to quality profile XML |
| `solution_path` | `SonarTestBadCode.sln` | string | Repo-relative path to `.sln` or `.csproj` |

---

## Workflow inputs — `sonar-fix-v3.yml`

| Input | Default | Description |
|-------|---------|-------------|
| `project_keys` | `sonar-test-bad-code` | Same as batch |
| `severity_*` | `true` | Same as batch |
| `language_filter` | `cs` | Same as batch |
| `max_files` | `0` | Same as batch |
| `max_issues_per_call` | `15` | Issues per AI request. Lower = less hallucination risk, more API calls |
| `run_copilot_agent` | `false` | Fire Copilot Agent Tasks for skipped files. Requires `COPILOT_PAT` |

---

## Environment variables — `fetch_issues.py`

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `SONARQUBE_URL` | Yes | `http://localhost:9000` | SonarQube base URL |
| `SONARQUBE_TOKEN` | Yes | — | Auth token (username in HTTP Basic) |
| `SONAR_PROJECT_KEYS` | Yes | — | Comma-separated project keys |
| `SONAR_SEVERITIES` | Yes | — | Comma-separated severities |
| `SONAR_LANGUAGE` | No | `cs` | Language filter |

---

## Environment variables — `batch_issues.py`

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `WORKSPACE_ROOT` | No | `cwd` | Absolute path to the repository root |
| `MAX_FILES` | No | `0` | Cap on output batches |
| `SCAN_COMMIT_SHA` | No | `""` | Git SHA of the scanned commit; written into `batches.json` for downstream validation |

---

## Environment variables — `fix_batch.py`

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `WORKSPACE_ROOT` | No | `cwd` | Absolute path to the repository root |
| `COPILOT_TOKEN` | No | `""` | GitHub PAT for Copilot Enterprise API |
| `GITHUB_TOKEN` | Yes | — | Built-in Actions token for GitHub Models |
| `MAX_ISSUES_PER_CALL` | No | `15` | Issues per AI call; controls sub-batching |
| `GITHUB_RUN_ID` | No | `local` | Written into git commit messages |

---

## Environment variables — `create_pr.py`

| Variable | Required | Default | Description |
|----------|----------|---------|-------------|
| `GITHUB_TOKEN` | Yes | — | Built-in Actions token; needed for `gh` CLI |
| `GITHUB_RUN_ID` | No | `local` | Written into PR body |
| `GITHUB_RUN_NUMBER` | No | `0` | Used in branch name (`ai/sonarqube-fixes-{N}`) |
| `GITHUB_BASE_BRANCH` | No | `agent/fix` | PR target branch |
| `FIX_EXIT_CODE` | No | `0` | From `fix_batch.py`; `2` triggers a warning banner in the PR body |
| `WORKFLOW_NAME` | No | `""` | Displayed in the PR body |

---

## `constants.py` — tuneable values

These can be changed in `scripts/shared/constants.py` to adjust runtime behavior without modifying any workflow YAML.

| Constant | Default | Effect |
|----------|---------|--------|
| `AI_RETRY_MAX` | `3` | Max retries per API call before skipping |
| `AI_INTER_FILE_DELAY_SEC` | `3` | Sleep between files (rate limit courtesy) |
| `MAX_ISSUES_PER_CALL` | `15` | Default sub-batch size (overridden by env var) |
| `VALIDATION_MIN_CHARS` | `50` | Minimum AI response length |
| `VALIDATION_MAX_SIZE_RATIO` | `2.5` | AI response can't be >2.5× the original |
| `VALIDATION_MIN_SIZE_RATIO` | `0.3` | AI response can't be <0.3× the original |
| `MAX_TOKENS_PER_FILE` | `3000` | Token estimate threshold for `large_file` flag |
| `SONAR_PAGE_SIZE` | `500` | Issues per REST API page |
| `SONAR_MAX_OFFSET` | `10000` | SonarQube pagination hard cap |

---

## Quality profile

The custom C# quality profile is stored at `sonar/quality-profile-cs.xml`. It defines exactly which rules are active for the scan. Imported at runtime into the ephemeral SonarQube instance during the bootstrap step.

To regenerate from a live SonarQube instance:
1. Navigate to **Quality Profiles** in the SonarQube UI
2. Select your C# profile → **Back up**
3. Replace `sonar/quality-profile-cs.xml` with the downloaded XML

---

## Related documents

- [[04-workflow-batch]] — how secrets flow through the batch workflow
- [[05-workflow-fix]] — how secrets flow through the fix workflow
- [[07-scripts]] — per-script environment variable details
