---
title: Branch Strategy & PR Flow
tags: [branches, git, pr, workflow, github]
aliases: [branches, branch-strategy, pr-flow, git-flow]
---

# Branch Strategy & PR Flow

---

## Branch naming constraint

> [!IMPORTANT]
> Git stores branch names as filesystem paths under `refs/heads/`. A branch named `agent` cannot coexist with branches named `agent/fix` or `agent/batching` — `agent` would need to be both a file and a directory simultaneously. This is why the new aggregation branch is named `agent-queue` instead of `agent`.

---

## Branch inventory

| Branch | Purpose | Created by |
|--------|---------|------------|
| `dev` | Active development; pushes here trigger the batch workflow | Developers |
| `agent-queue` | Staging area: receives batch PRs containing code + issue files | `sonar-batch.yml` (auto-creates on first run) |
| `agent/fix` | Base for the fix workflow; fix PRs target this branch | Manual / legacy |
| `agent/batching` | Legacy — was the base for the old batch-only workflow | Legacy |
| `sonar/batch-{N}` | Ephemeral PR branch: `dev` HEAD + `batches.json` | `sonar-batch.yml` per run |
| `ai/sonarqube-fixes-{N}` | Ephemeral PR branch: AI-fixed files | `sonar-fix-v3.yml` per run |
| `main` | Production / merge target | Standard |

---

## Full PR flow

```
dev (push)
   │
   ▼ sonar-batch.yml
   │
   ├─► creates: sonar/batch-{run_number}
   │     contains: latest dev code + output/issues.json + output/batches.json
   │
   └─► opens PR: sonar/batch-{N} → agent-queue
         │
         │ (merge manually or via automation)
         ▼
       agent-queue
         │
         │ (trigger sonar-fix-v3.yml manually on agent/fix, or adapt to read from agent-queue)
         ▼ sonar-fix-v3.yml
         │
         ├─► creates: ai/sonarqube-fixes-{run_number}
         │     contains: AI-fixed source files
         │
         └─► opens PR: ai/sonarqube-fixes-{N} → agent/fix
               │
               │ sonar-verify.yml triggers automatically
               ▼
             Build verification (dotnet msbuild)
               │
               │ (human review + merge)
               ▼
             agent/fix (fixed code lives here)
```

---

## `sonar/batch-{N}` branch

Created by `sonar-batch.yml` after every successful scan.

- **Base:** current `dev` HEAD (the commit that was just pushed)
- **Extra commits:** `output/issues.json` + `output/batches.json` force-added (gitignored files)
- **Commit message:** `ci: sonar batch — run #{N} @ {sha}`
- **PR title:** `ci: sonar batch — run #{N} @ {branch}`
- **PR target:** `agent-queue`

The key design property: this branch carries **both** the latest source code and the fresh batch files. Merging it into `agent-queue` gives the fix workflow everything it needs without a re-scan.

---

## `ai/sonarqube-fixes-{N}` branch

Created by `create_pr.py` after the AI fix loop completes.

- **Base:** the `agent/fix` branch at the time of the fix run
- **Commits:** one commit per successfully fixed file (`fix(sonar): N issues in File.cs [batch-X, run-Y]`)
- **PR title:** `fix(sonar): AI auto-remediation — {fixed}/{total} files fixed [run #{N}]`
- **PR target:** `agent/fix`

The `sonar-verify.yml` workflow triggers automatically on this PR to confirm the fixes compile.

---

## Concurrency protection

`sonar-batch.yml` uses a concurrency group:

```yaml
concurrency:
  group: sonar-batch-${{ github.ref }}
  cancel-in-progress: true
```

If two commits are pushed to `dev` quickly, the first run is cancelled and only the second run produces the batch PR. This prevents two batch branches racing to push conflicting batch files.

---

## `agent-queue` first-run bootstrap

On the very first execution of `sonar-batch.yml`, if `agent-queue` does not exist:

```bash
if git ls-remote --exit-code origin refs/heads/agent-queue > /dev/null 2>&1; then
  echo "agent-queue branch already exists"
else
  git checkout -b agent-queue
  git push origin agent-queue
fi
```

The branch is created from the current `dev` HEAD. Subsequent runs skip this step.

---

## `scan_commit` SHA validation

`batches.json` includes the commit SHA that was scanned:

```json
{ "scan_commit": "7f08ff5a3b2c..." }
```

The fix workflow can read this and validate that the current HEAD matches before applying patches. If the code has been updated since the scan, line numbers in the batch files may be stale and the fix should be aborted or re-triggered.

For `SonarTestBadCode` this check always passes — the violation files never change. For a production repo this is an important safety gate.

---

## Related documents

- [[04-workflow-batch]] — how `sonar/batch-{N}` branches are created
- [[05-workflow-fix]] — how `ai/sonarqube-fixes-{N}` branches are created
- [[06-workflow-verify]] — build verification on fix PRs
- [[01-architecture]] — full pipeline data flow
