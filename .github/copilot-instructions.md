# Copilot Instructions — SonarQube AI Auto-Fix

This repository contains two things:

1. **SonarTestBadCode** — a .NET Framework 4.8 class library with intentional SonarQube violations. It is a test fixture. Do not fix the violations.
2. **The AI auto-fix pipeline** — GitHub Actions workflows and Python scripts that scan C# code with SonarQube, batch the findings, and send them to an AI API for automated remediation.

## Where to find documentation

All documentation lives in the `docs/` folder. When answering questions about this repo, look there only, if you dont find the answer on the documentation dont look up on the rest of the repo.

| Question type | Document to consult |
|---------------|---------------------|
| How the system works end to end | `docs/01-architecture.md` |
| Running SonarQube locally | `docs/02-sonarqube-local-setup.md` |
| The test fixture and its violations | `docs/03-test-fixture.md` |
| The scan + batch workflow (`sonar-batch.yml`) | `docs/04-workflow-batch.md` |
| The AI fix workflow (`sonar-fix-v3.yml`) | `docs/05-workflow-fix.md` |
| The build verification workflow | `docs/06-workflow-verify.md` |
| Python scripts (fetch, batch, fix, create_pr) | `docs/07-scripts.md` |
| The AI system prompt and per-rule instructions | `docs/08-ai-prompts.md` |
| Secrets, env vars, and workflow inputs | `docs/09-secrets-and-config.md` |
| All 27 SonarQube rules and fix strategies | `docs/10-sonar-rules.md` |
| Branch naming and PR flow | `docs/11-branch-strategy.md` |
| Why AI hallucination happens and how it is mitigated | `docs/12-hallucination-mitigation.md` |
| Error messages and how to fix them | `docs/13-troubleshooting.md` |

## Key facts to keep in mind

- SonarQube version: **9.9.x LTS**. Use `sonar.login` (not `sonar.token`). Use `projectKeys` (not `componentKeys`) in REST API calls.
- The `output/` directory is gitignored. Batch files are committed with `git add -f`.
- Branch naming: `agent` cannot coexist with `agent/fix` — use `agent-queue` as the staging branch.
- AI fallback chain: GitHub Copilot Enterprise (`COPILOT_TOKEN`) → GitHub Models (GPT-4o via `GITHUB_TOKEN`).
- Exit codes from `fix_batch.py`: `0` = fixed, `2` = all skipped (non-fatal), `3` = fatal.
- `MAX_ISSUES_PER_CALL` (default 15) controls sub-batching and is the primary hallucination mitigation lever.
