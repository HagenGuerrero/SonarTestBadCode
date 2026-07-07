---
title: Troubleshooting
tags: [troubleshooting, errors, debugging, known-issues]
aliases: [troubleshooting, errors, debugging, faq]
---

# Troubleshooting

---

## Git / branch errors

### `fatal: cannot lock ref 'refs/heads/agent': 'refs/heads/agent/batching' exists`

**Cause:** Git cannot create a branch named `agent` when branches named `agent/fix` or `agent/batching` already exist. Ref names are stored as filesystem paths — `agent` would need to be both a file and a directory.

**Fix:** Use `agent-queue` (or any name that doesn't conflict with the existing `agent/*` namespace). The batch workflow uses `agent-queue` as the PR target for exactly this reason.

---

### `error: src refspec agent-queue does not match any`

**Cause:** The local `agent-queue` branch was not created before pushing.

**Fix:** The `sonar-batch.yml` "Ensure agent-queue branch exists" step handles this automatically. If running locally:
```bash
git checkout -b agent-queue
git push origin agent-queue
```

---

### `nothing to commit` on the batch output step

**Cause:** `output/issues.json` and `output/batches.json` are gitignored. `git add` without `-f` silently ignores them.

**Fix:** The workflow uses `git add -f output/issues.json output/batches.json`. If replicating locally, always use the `-f` flag.

---

## SonarQube errors

### `sonar.token is not supported on SonarQube 9.9.x`

**Cause:** `sonar.token` is the 10.x scanner parameter. The 9.9.x LTS scanner expects `sonar.login`.

**Fix:** Use `sonar.login` in all scanner commands:
```bash
dotnet sonarscanner begin /d:sonar.login="YOUR-TOKEN"
dotnet sonarscanner end   /d:sonar.login="YOUR-TOKEN"
```

---

### REST API returns incomplete issue list (`componentKeys`)

**Cause:** On SonarQube 9.9.x, using `componentKeys` in `/api/issues/search` with a project key returns only a subset of issues.

**Fix:** Use `projectKeys` instead:
```
/api/issues/search?projectKeys=YOUR-PROJECT-KEY
```

---

### `SonarQube never became UP after 30 attempts`

**Cause:** The SonarQube service container didn't start in time. Common when GitHub Actions runners are under load.

**Fix:** Re-run the workflow. The 30×10s = 5-minute window is usually enough. If it consistently fails, increase the attempt count in the wait loop.

---

### CE task `FAILED`

**Cause:** The SonarQube Compute Engine task (background analysis) failed. This usually means the build output wasn't produced or the scanner didn't report cleanly.

**Fix:**
1. Check the Mono MSBuild step — confirm `SonarTestBadCode.sln` compiled with exit code 0
2. Confirm `dotnet sonarscanner end` did not print errors
3. Check SonarQube logs via: `GET http://localhost:9000/api/ce/task?id=<taskId>`

---

### Quality profile import fails: `Profile 'Sonar way' is built-in and cannot be replaced`

**Cause:** The XML contains `<name>Sonar way</name>` which is the read-only built-in profile name.

**Fix:** The workflow's import step renames it before uploading:
```bash
sed 's|<name>Sonar way</name>|<name>CI-Profile-cs</name>|' sonar/quality-profile-cs.xml > /tmp/ci-profile.xml
```
If running locally, apply the same sed substitution before importing.

---

## AI / fix errors

### All files skipped — exit code 2

**Cause:** `fix_batch.py` processed all batches but every file failed validation or the API was unavailable.

**Fix:**
1. Check the workflow log for API error codes (401, 403, 429, 5xx)
2. If 401/403: verify `COPILOT_TOKEN` is valid and has `copilot` scope; GitHub Models should have been the fallback — check if `GITHUB_TOKEN` has `models: read` permission
3. If all validation failures: lower `MAX_ISSUES_PER_CALL` — prompts may be too large for the model to return complete files

The fix PR is still created (with a warning banner) even when exit code is 2. This is intentional.

---

### `size ratio suspicious` validation failures

**Cause:** The AI returned a truncated or padded response — either it summarized the file instead of returning the full content, or it added extensive commentary.

**Fix:**
- Lower `MAX_ISSUES_PER_CALL` (fewer issues per call = shorter prompt = model returns complete files more reliably)
- The system automatically retries once with a "return the COMPLETE file" reminder injected into the prompt

---

### `namespace declaration missing` validation failure

**Cause:** The AI replaced the entire file with only the changed section, dropping the namespace declaration.

**Fix:** Same as size ratio — lower `MAX_ISSUES_PER_CALL`. This is the most common catastrophic hallucination mode.

---

### `unbalanced braces` validation failure

**Cause:** The AI produced syntactically broken C# with unclosed classes or methods.

**Fix:**
- Check if the file contains Tier 3 rules (S134, S138, S1541, S3776) — these rules are more likely to cause the model to attempt structural changes that break brace balance
- The rules should be in the always-skip list in `agent.md` — verify they are listed there

---

### `COPILOT_TOKEN` exchange fails with 401

**Cause:** The PAT used as `COPILOT_TOKEN` doesn't have the `copilot` scope, or the account doesn't have a Copilot Enterprise subscription.

**Fix:** The system automatically falls back to GitHub Models. Verify the fallback is working by checking for `"Copilot auth failed, switching to GitHub Models"` in the log. If GitHub Models also fails, check that the workflow has `models: read` permission.

---

### `gh pr create` fails: `GraphQL: No commits between agent-queue and sonar/batch-{N}`

**Cause:** The batch PR branch has the same commit as `agent-queue`. This happens if the code on `dev` is identical to `agent-queue`.

**Fix:** This is expected when `dev` and `agent-queue` are in sync and only the batch files changed. Force-add the batch files before the commit so there is always at least one commit:
```bash
git add -f output/issues.json output/batches.json
git commit -m "ci: sonar batch — run #N @ sha"
```
The `output/` files contain timestamps so they are always different from the previous run.

---

## Build verification errors

### `sonar-verify.yml` fails: `Mono MSBuild not found`

**Cause:** The Mono apt package wasn't installed correctly.

**Fix:** The workflow installs from the official Mono Project repo. If the keyserver is unreachable, the apt-key step fails silently. Re-run the workflow. If consistently failing, check if `hkp://keyserver.ubuntu.com:80` is accessible from GitHub Actions.

---

### Build fails on a fix PR

**Cause:** The AI produced syntactically invalid C# that passed heuristic validation but fails compilation.

**Fix:**
1. Look at which file's commit caused the failure (each commit = one file)
2. Use `git revert <commit-sha>` to remove that specific file's fix
3. The remaining fixes in the PR can still be merged
4. The problematic file will be retried in the next batch run

---

## Workflow trigger issues

### Batch workflow doesn't trigger on `dev` push

**Cause:** The workflow file has a syntax error, or the branch name in the trigger doesn't exactly match.

**Fix:**
1. Verify `.github/workflows/sonar-batch.yml` parses correctly (GitHub shows YAML errors in the Actions tab)
2. Confirm the trigger: `on: push: branches: [dev]` — the branch name must be exactly `dev`
3. Check the Actions tab for the workflow — it may be disabled

---

### Two batch runs created conflicting PRs

**Cause:** The concurrency group cancelled the wrong run, or two pushes happened simultaneously before the concurrency check applied.

**Fix:** The concurrency group `sonar-batch-${{ github.ref }}` with `cancel-in-progress: true` prevents this in normal operation. If it happens, close the older PR (the one with the earlier run number) and work from the newer one.

---

## Related documents

- [[04-workflow-batch]] — batch workflow steps
- [[05-workflow-fix]] — fix workflow steps
- [[07-scripts]] — script internals and exit codes
- [[09-secrets-and-config]] — secrets setup
- [[02-sonarqube-local-setup]] — local SonarQube commands
