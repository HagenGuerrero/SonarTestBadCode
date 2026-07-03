# GitHub Copilot API Integration Plan

## Context

Phase 1 of the SonarQube auto-remediation pipeline is complete on both repos
(SonarTestBadCode and visor-nomina-poc-ia): scan → fetch issues → batch by file.
The workflow produces `output/batches.json` with one entry per source file.

Phase 2 — calling an AI API to fix each batch and opening a PR — has not been
implemented. The two required scripts are empty stubs:
- `scripts/ai/fix_batch.py`
- `scripts/git/create_pr.py`

`scripts/shared/constants.py` already pre-declares both API endpoints, retry
config, and validation thresholds. The plan is to build exactly those two
scripts, wire them into the workflow, and make the system scale from 7 batches
(SonarTestBadCode, ~255 issues) to 227+ batches (visor-nomina, 2,706 issues)
and beyond.

---

## API Strategy: Two-tier Fallback Chain

Already specified in `plan.md` and pre-wired in `constants.py`.

**Primary: GitHub Copilot Enterprise**
- URL: `COPILOT_URL = "https://api.githubcopilot.com/chat/completions"` (already in constants)
- Auth: `Authorization: Bearer {COPILOT_TOKEN}` + `Copilot-Integration-Id: vscode-chat`
- Requires: `COPILOT_TOKEN` stored as a GitHub Actions repo secret (PAT from
  an account with a Copilot Enterprise subscription)
- No hard daily request cap; per-minute rate limit (generous)

**Fallback: GitHub Models**
- URL: `MODELS_URL = "https://models.inference.ai.azure.com/chat/completions"` (already in constants)
- Auth: `Authorization: Bearer {GITHUB_TOKEN}` (the built-in Actions token)
- Requires: `models: read` permission added to the workflow (one-line change)
- Free tier: ~15 RPM for GPT-4o — enough for dev/testing and small repos

**Trigger logic:**
```
try Copilot Enterprise
  on 401/403 → switch to GitHub Models (log once, no further retries on Copilot)
  on 429     → respect retry-after header, sleep, retry same provider
  on 3x fail → skip batch (exit code 2), continue to next batch
on GitHub Models 429 → same retry logic
```

The fallback chain is already the design from `plan.md`. The only new config
requirement is the optional `COPILOT_TOKEN` secret and one added workflow
permission for GitHub Models.

---

## Scaling Design: Adaptive Sub-Batching

The current `batches.json` schema (one file = one batch) is kept intact.
`fix_batch.py` reads it and decides at runtime how many API calls each batch needs.

### The cap: `MAX_ISSUES_PER_CALL`

Configurable via env var, default 15. This is the maximum number of issues sent
to the AI in a single chat completions request.

```
avg issues/file in visor-nomina: 2705 / 227 = ~12
→ most batches fit in one call with default cap of 15
→ only files with >15 issues trigger sub-batching
```

### Sub-batch execution for large files

When `issue_count > MAX_ISSUES_PER_CALL`:

1. Sort issues by line number ascending (already done in `batch_issues.py`)
2. Split into chunks of `MAX_ISSUES_PER_CALL`
3. **Chunk 1**: read original file → send to AI → validate → write back
4. **Chunk 2**: read the ALREADY-MODIFIED file → send to AI with next chunk → validate → write back
5. Repeat until all chunks processed or one fails validation (skip remainder, non-fatal)

Each subsequent chunk reads the current file state, not the original. This keeps
line numbers accurate for the model and builds fixes incrementally.

### API call count estimates

| Repo | Issues | Batches | Calls at cap=15 | Time at 3s/call |
|------|--------|---------|-----------------|-----------------|
| SonarTestBadCode | 255 | 7 | ~10 | ~30 seconds |
| visor-nomina | 2,706 | 227 | ~230 | ~12 minutes |
| Hypothetical 10k | 10,000 | 800+ | ~800 | ~40 minutes |

All within the GitHub Actions 6-hour job timeout.

---

## Hallucination Mitigation

### Why hallucination happens here

Two independent risk factors compound:

1. **Issue count per call.** Asking the model to hold 40 simultaneous fix
   instructions in context causes it to confuse line numbers, skip fixes, or
   over-correct unrelated lines. The `MAX_ISSUES_PER_CALL` cap directly addresses this.

2. **Rule complexity.** Mixing mechanical rules with logic-dependent rules in
   one prompt confuses the model:

   | Tier | Rules | Hallucination risk |
   |------|-------|--------------------|
   | Simple (mechanical) | S125, S1116, S1186, S1192, S1481, S1643, S1764, S3400, S3963, S3717 | Low — one-line changes |
   | Moderate (class context) | S1066, S1118, S1144, S1172, S2386, S112, S1871 | Medium |
   | Complex (logic-dependent) | S2221, S2583, S2589, S2696 | High — require understanding call stacks and invariants |

   The issue cap indirectly helps here: a file with 3 complex S2221 violations
   and 12 simple violations gets at most 15 issues per call, so the model isn't
   overwhelmed by mixing.

### System prompt design

```
You are a C# code quality expert. You will be given a C# source file and a list
of SonarQube issues to fix. Fix ONLY the listed issues. Do not refactor, rename,
reformat, or improve anything not in the list. Return the complete file content
with no markdown code fences, no explanations, and no comments about what changed.
If you are unsure how to safely fix an issue, leave that line exactly as-is.
```

Temperature: `0.2` (low — code generation needs determinism, not creativity).

### Validation chain

Uses thresholds already declared in `constants.py`:

```
1. min length:     len(response) >= VALIDATION_MIN_CHARS (50)
2. size ratio:     VALIDATION_MIN_SIZE_RATIO (0.3) <= len(response)/len(original) <= VALIDATION_MAX_SIZE_RATIO (2.5)
3. namespace:      original namespace declaration still present in response
4. brace balance:  abs(response.count('{') - response.count('}')) <= 2
5. no fences:      response does not start with ``` or contain ```csharp
```

On any check failing: skip this sub-batch chunk, log it, do NOT write the file,
continue with next batch. The remaining chunks of the same file are also skipped.

**What validation does NOT catch:** Logical errors in complex rules (S2221, S2583,
S2589, S2696). These require understanding invariants. The mitigation is the issue
cap and the focused system prompt. A post-fix SonarQube re-scan is the authoritative
quality gate — bad fixes re-surface as issues in the next run.

No MSBuild compile check per file (too slow: ~30-60s × 227 files = 2+ hours extra).

---

## Implementation: `scripts/ai/fix_batch.py`

### Inputs

- `output/batches.json` (Phase 1 output)
- Env vars: `WORKSPACE_ROOT`, `COPILOT_TOKEN` (optional), `GITHUB_TOKEN`,
  `MAX_ISSUES_PER_CALL` (default 15), `GITHUB_RUN_ID`

### Core loop

```python
load batches.json
for batch in batches:
    if not batch["file_exists"]: skip

    chunks = split_issues(batch["issues"], MAX_ISSUES_PER_CALL)
    current_content = read(batch["abs_path"])

    for chunk_idx, chunk in enumerate(chunks):
        response = call_ai(current_content, batch["local_path"], chunk)
        if not validate(response, current_content):
            log_skip(batch, chunk_idx); break
        write(batch["abs_path"], response)
        current_content = response
        git_add(batch["abs_path"])

    git_commit(f"fix(sonar): batch {batch['batch_id']} - {batch['local_path']} [{run_id}]")
    write_fix_result(batch)
    sleep(AI_INTER_FILE_DELAY_SEC)   # constant already in constants.py

exit(EXIT_CODE_SUCCESS)       # if any fixes committed
exit(EXIT_CODE_ALL_SKIPPED)   # if all skipped (non-fatal)
exit(EXIT_CODE_FATAL)         # on fatal error
```

### AI call function

```python
def call_ai(file_content, file_path, issues, retries=0):
    payload = {
        "model": AI_MODEL,                    # "gpt-4o" from constants
        "messages": [
            {"role": "system", "content": SYSTEM_PROMPT},
            {"role": "user",   "content": build_user_prompt(file_path, file_content, issues)}
        ],
        "temperature": 0.2,
        "max_tokens": 4096
    }

    # Try Copilot Enterprise first
    if copilot_available and COPILOT_TOKEN:
        resp = post(COPILOT_URL, payload, headers={
            "Authorization": f"Bearer {COPILOT_TOKEN}",
            "Copilot-Integration-Id": "vscode-chat"
        })
        if resp.status_code in (401, 403):
            copilot_available = False   # disable for this run, fall through
        elif resp.status_code == 200:
            return extract_content(resp)
        elif resp.status_code == 429:
            sleep(parse_retry_after(resp))
            return call_ai(file_content, file_path, issues, retries + 1)

    # Fallback: GitHub Models
    resp = post(MODELS_URL, payload, headers={
        "Authorization": f"Bearer {GITHUB_TOKEN}"
    })
    if resp.status_code == 200:
        return extract_content(resp)
    elif resp.status_code == 429:
        sleep(parse_retry_after(resp))
        if retries < AI_RETRY_MAX:
            return call_ai(file_content, file_path, issues, retries + 1)
    return None   # triggers skip in caller
```

### User prompt structure

```
File: {local_path}

Issues to fix:
  Line {line}: [{rule}] {severity} — {message}
  Line {line}: [{rule}] {severity} — {message}
  ...

File content:
{full file content}
```

---

## Implementation: `scripts/git/create_pr.py`

### Inputs

- `output/fix_results/batch_*.json` (written by fix_batch.py)
- Env vars: `GITHUB_TOKEN`, `GITHUB_REPOSITORY`, `GITHUB_RUN_ID`

### Steps

1. Aggregate all `fix_results/batch_*.json`
2. `git push -u origin ai/sonarqube-fixes-{run_id}`
3. Build PR body:
   - Summary table: files fixed, files skipped, total issues addressed
   - Per-batch detail table: file, issues fixed, issues skipped, validation failures
   - Link to workflow run
4. `gh pr create --title "fix(sonar): AI auto-remediation run {run_id}" --body "..."`
5. Write PR URL to `$GITHUB_STEP_SUMMARY`

---

## Workflow Changes (`sonar-batch.yml`)

### 1. Add `models: read` permission

```yaml
permissions:
  contents: write
  pull-requests: write
  models: read        # enables GITHUB_TOKEN to call GitHub Models
```

### 2. Add `COPILOT_TOKEN` repo secret (optional)

Store a GitHub PAT from a Copilot Enterprise account as `COPILOT_TOKEN`.
If absent, the workflow falls through to GitHub Models automatically.

### 3. Add workflow input

```yaml
inputs:
  max_issues_per_call:
    description: 'Max issues per AI call (lower = less hallucination risk)'
    default: '15'
```

### 4. Two new steps after the batch step

```yaml
- name: AI fix batches
  id: fix
  env:
    COPILOT_TOKEN: ${{ secrets.COPILOT_TOKEN }}
    GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
    MAX_ISSUES_PER_CALL: ${{ inputs.max_issues_per_call }}
    GITHUB_RUN_ID: ${{ github.run_id }}
    WORKSPACE_ROOT: ${{ github.workspace }}
  run: python scripts/ai/fix_batch.py

- name: Create PR
  if: steps.fix.outcome == 'success'
  env:
    GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
    GITHUB_REPOSITORY: ${{ github.repository }}
    GITHUB_RUN_ID: ${{ github.run_id }}
  run: python scripts/git/create_pr.py
```

---

## Files to Create / Modify

| File | Action |
|------|--------|
| `scripts/ai/fix_batch.py` | Create |
| `scripts/git/create_pr.py` | Create |
| `.github/workflows/sonar-batch.yml` | Modify (add permission + input + 2 steps) |
| `scripts/shared/constants.py` | Possibly extend (`MAX_ISSUES_PER_CALL` default, `SYSTEM_PROMPT`) |

Output directory already declared in constants: `FIX_RESULTS_DIR = "output/fix_results"`.

---

## Verification

1. **Small scale (SonarTestBadCode, 7 batches):** Run workflow on `agent/batching`
   with `max_issues_per_call=5` to force sub-batching. Expect 7 commits, a PR
   targeting `main`, at least one file changed.

2. **Large scale (visor-nomina, 227 batches):** Run with `max_issues_per_call=15`
   (default). Watch logs for 429 events and validation failures. Expect a PR with
   a summary table showing fixed vs skipped.

3. **Fallback test:** Set `COPILOT_TOKEN` to an invalid value. Confirm logs show
   "Copilot auth failed, switching to GitHub Models" and the run completes.

4. **Cap override test:** Set `max_issues_per_call=3` on a file with >3 issues.
   Confirm sequential sub-batches are applied and each chunk reads the already-
   modified file state.
