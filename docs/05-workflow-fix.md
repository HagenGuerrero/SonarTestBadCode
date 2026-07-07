---
title: Workflow — sonar-fix-v3.yml (AI Fix)
tags: [workflow, github-actions, ai-fix, sonar-fix, automation]
aliases: [fix-workflow, sonar-fix, ai-workflow]
---

# Workflow — `sonar-fix-v3.yml` (AI Fix)

**File:** `.github/workflows/sonar-fix-v3.yml`
**Name:** SonarQube — Scan, Batch & AI Auto-Fix (v3 — three-job)

---

## Trigger

`workflow_dispatch` only — manually triggered from the `agent/fix` branch.

```yaml
if: github.ref == 'refs/heads/agent/fix'
```

> [!NOTE]
> This workflow is older architecture. The batch workflow (`sonar-batch.yml`) now handles the scan+batch phase and creates a PR to `agent-queue`. `sonar-fix-v3.yml` is the standalone all-in-one alternative that scans, batches, and fixes in a single dispatch.

---

## Permissions

```yaml
permissions:
  contents: write
  pull-requests: write
  models: read          # enables GITHUB_TOKEN for GitHub Models API
  issues: write
```

---

## Inputs

| Input | Default | Description |
|-------|---------|-------------|
| `project_keys` | `sonar-test-bad-code` | SonarQube project keys |
| `severity_blocker/critical/major/minor/info` | `true` | Severity filters |
| `language_filter` | `cs` | Language key |
| `max_files` | `0` | Cap on batches (0 = unlimited) |
| `max_issues_per_call` | `15` | Issues per AI call — lower = less hallucination risk |
| `run_copilot_agent` | `false` | Fire Copilot Agent Tasks for skipped files |

---

## Three-job structure

The workflow splits into three dependent jobs to isolate the expensive SonarQube infrastructure from the cheaper AI fix step.

```
scan ──► batch ──► fix
```

### Job 1 — `scan`

Runs on `ubuntu-latest` with the SonarQube service container.

Steps:
1. Checkout, Python, Java, dotnet-sonarscanner, Mono MSBuild setup
2. Wait for SonarQube UP, bootstrap (token + project)
3. Import quality profile (`sonar/quality-profile-cs.xml`)
4. SonarScanner begin → build → SonarScanner end → wait for CE task
5. Build severity filter
6. `fetch_issues.py` → `output/issues.json`
7. Upload `output/issues.json` as artifact `sonar-issues-{run_number}` (7-day retention)

### Job 2 — `batch`

Runs on `ubuntu-latest`, no SonarQube container (lightweight).

Steps:
1. Checkout, Python setup
2. Download `sonar-issues-{run_number}` artifact → `output/`
3. `batch_issues.py` → `output/batches.json`
4. Upload `output/batches.json` as artifact `sonar-batches-{run_number}` (7-day retention)

### Job 3 — `fix`

Runs on `ubuntu-latest`, no SonarQube container, no Java, no Mono.

Steps:
1. Checkout (fetch-depth: 0), Python setup
2. Configure git identity
3. Download `sonar-batches-{run_number}` artifact → `output/`
4. **AI fix loop** — `fix_batch.py`
5. Upload `output/` as artifact `sonar-fix-run-{run_number}` (7-day retention)
6. **Create fix PR** — `create_pr.py`
7. (Optional) Create Copilot Agent Tasks for skipped files

---

## AI fix loop (Job 3, step 4)

```yaml
- name: AI fix batches
  id: fix
  env:
    COPILOT_TOKEN: ${{ secrets.COPILOT_TOKEN }}
    GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
    MAX_ISSUES_PER_CALL: ${{ inputs.max_issues_per_call }}
    GITHUB_RUN_ID: ${{ github.run_id }}
    WORKSPACE_ROOT: ${{ github.workspace }}
  run: |
    python scripts/ai/fix_batch.py
    EXIT=$?
    echo "exit_code=$EXIT" >> "$GITHUB_OUTPUT"
    [ $EXIT -eq 3 ] && exit 1 || exit 0
```

Exit code semantics:
- `0` — at least one file fixed
- `2` — all batches skipped (non-fatal, PR is still created)
- `3` — fatal error (fails the step)

---

## PR creation (Job 3, step 6)

```yaml
- name: Create fix PR
  if: steps.fix.outputs.exit_code != '3'
  env:
    GH_TOKEN: ${{ secrets.GITHUB_TOKEN }}
    GITHUB_BASE_BRANCH: agent/fix
    WORKFLOW_NAME: ${{ github.workflow }}
```

`create_pr.py` creates branch `ai/sonarqube-fixes-{run_number}`, pushes it, and opens a PR to `agent/fix` with a detailed body including fixed/skipped file tables and token usage.

---

## Copilot Agent Tasks (optional, Job 3, step 7)

When `run_copilot_agent: true` and the `COPILOT_PAT` secret is present, `create_copilot_tasks.py` fires Copilot Agent Tasks for every file that `fix_batch.py` could not fix automatically.

> [!IMPORTANT]
> This requires `COPILOT_PAT` — a fine-grained PAT with `actions:write`, `contents:write`, `issues:write`, `pull-requests:write`. The built-in `GITHUB_TOKEN` is rejected by the Agent Tasks API (billing is user-level).

---

## Artifacts produced

| Artifact name | Contents | Retention |
|---------------|----------|-----------|
| `sonar-issues-{run_number}` | `output/issues.json` | 7 days |
| `sonar-batches-{run_number}` | `output/batches.json` | 7 days |
| `sonar-fix-run-{run_number}` | full `output/` directory | 7 days |

---

## Related documents

- [[04-workflow-batch]] — newer batch-only workflow
- [[06-workflow-verify]] — build verification on fix PRs
- [[07-scripts]] — `fix_batch.py` and `create_pr.py` internals
- [[08-ai-prompts]] — the AI system prompt
- [[09-secrets-and-config]] — `COPILOT_TOKEN`, `COPILOT_PAT`
- [[12-hallucination-mitigation]] — how `max_issues_per_call` is tuned
