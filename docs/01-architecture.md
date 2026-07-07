---
title: System Architecture
tags: [architecture, design, overview, data-flow]
aliases: [architecture, system-design, overview]
---

# System Architecture

## Purpose

An automated pipeline that:
1. Scans a C# codebase with SonarQube
2. Batches the findings by source file
3. Sends each file + its issues to an AI API
4. Commits the AI-generated fixes to a branch
5. Opens a pull request for human review

No human intervention is needed between push and PR creation.

---

## End-to-end data flow

```
Developer pushes to dev
         │
         ▼
┌─────────────────────────────────────────────────────────┐
│  sonar-batch.yml  (GitHub Actions, ubuntu-latest)        │
│                                                          │
│  [SonarQube service container — ephemeral, H2 DB]        │
│   wait for UP → generate token → create project          │
│         │                                                │
│         ▼                                                │
│   dotnet sonarscanner begin                              │
│   dotnet msbuild                                         │
│   dotnet sonarscanner end                                │
│         │                                                │
│         ▼                                                │
│   fetch_issues.py  →  output/issues.json                 │
│         │                                                │
│         ▼                                                │
│   batch_issues.py  →  output/batches.json                │
│         │                                                │
│         ▼                                                │
│   git commit output files                                │
│   gh pr create  →  PR to agent-queue branch             │
└─────────────────────────────────────────────────────────┘
         │
         │  (human or automation merges to agent-queue)
         ▼
┌─────────────────────────────────────────────────────────┐
│  sonar-fix-v3.yml  (GitHub Actions, ubuntu-latest)       │
│  (triggered manually from agent/fix branch)              │
│                                                          │
│  Job 1 — scan:  full SonarQube scan → issues.json        │
│  Job 2 — batch: batch_issues.py → batches.json           │
│  Job 3 — fix:                                            │
│    for each batch:                                       │
│      call AI (Copilot Enterprise → GitHub Models)        │
│      validate response                                   │
│      write file, git commit                              │
│    create_pr.py → PR ai/sonarqube-fixes-{N}             │
└─────────────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────┐
│  sonar-verify.yml  (triggered on PR to agent/fix)        │
│   dotnet msbuild → confirms fixes compile                │
└─────────────────────────────────────────────────────────┘
```

---

## Component inventory

| Component | Location | Role |
|-----------|----------|------|
| `sonar-batch.yml` | `.github/workflows/` | Scan trigger + batching, PR to `agent-queue` |
| `sonar-fix-v3.yml` | `.github/workflows/` | AI fix loop, runs from `agent/fix` |
| `sonar-verify.yml` | `.github/workflows/` | Build check on fix PRs |
| `fetch_issues.py` | `scripts/sonar/` | Paginates SonarQube REST API |
| `batch_issues.py` | `scripts/sonar/` | Groups issues by file, assigns batch IDs |
| `fix_batch.py` | `scripts/ai/` | AI API call loop, validation, git commits |
| `create_pr.py` | `scripts/git/` | Aggregates results, opens GitHub PR |
| `constants.py` | `scripts/shared/` | All shared config — no magic strings |
| `system_prompt.md` | `scripts/ai/prompts/` | AI behavioral rules |
| `agent.md` | `scripts/ai/prompts/` | Per-rule remediation instructions |
| `quality-profile-cs.xml` | `sonar/` | Custom SonarQube quality profile for C# |
| `SonarTestBadCode/` | repo root | Intentionally broken .NET 4.8 library (test fixture) |

---

## Key design decisions

### Ephemeral SonarQube (Option A — chosen)
A fresh `sonarqube:lts-community` Docker container starts inside the GitHub Actions job on every run. It uses the embedded H2 database. The project and token are created at runtime using the default `admin:admin` credentials. No persistent infrastructure required.

*Trade-off:* ~8–12 minutes of startup overhead per run. No issue history accumulates.

**Option B (not chosen):** Persistent SonarQube instance on a VM or hosted service, queried via REST API. Saves startup time and retains history. Better for large repos (Visor Nomina) where the scan overhead matters. The `fetch_issues.py` script already supports this — only the scan steps in the workflow change.

### Full-file replacement (not diff)
The AI receives the entire file content and must return the entire corrected file. Diffs are not generated or applied. This simplifies the AI prompt and the validation logic at the cost of requiring heuristic size-ratio checks to detect truncated responses.

### No MSBuild compile check per file
Compiling after every AI fix would take 30–60 seconds × N files = hours. Instead, heuristic validation (size ratio, brace balance, namespace presence) catches obvious failures fast. The post-merge `sonar-verify.yml` build is the authoritative compile gate.

### One commit per file
Each fixed file gets its own git commit with a structured message. This makes the fix PR easy to review file-by-file and easy to revert a single bad fix with `git revert`.

### Sub-batching for large files
Files with more issues than `MAX_ISSUES_PER_CALL` (default 15) are split into sequential chunks. Each chunk reads the already-modified file so line numbers stay accurate for subsequent chunks.

### AI fallback chain
Primary: GitHub Copilot Enterprise (`COPILOT_TOKEN` secret). On HTTP 401/403: fall back to GitHub Models (GPT-4o via `GITHUB_TOKEN`). On 429: respect `retry-after` header, exponential backoff, max 3 retries.

---

## Output files

| File | Written by | Consumed by |
|------|-----------|-------------|
| `output/issues.json` | `fetch_issues.py` | `batch_issues.py` |
| `output/batches.json` | `batch_issues.py` | `fix_batch.py` |
| `output/fix_results/batch_N.json` | `fix_batch.py` | `create_pr.py` |

All output files are gitignored by default. They are force-added (`git add -f`) when committed to the PR branch.

---

## Related documents

- [[04-workflow-batch]] — batch workflow step-by-step
- [[05-workflow-fix]] — fix workflow step-by-step
- [[07-scripts]] — script internals
- [[11-branch-strategy]] — branch and PR naming
