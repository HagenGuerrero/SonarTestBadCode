---
title: SonarQube AI Auto-Fix — Documentation Index
tags: [index, sonarqube, ai-fix, automation]
aliases: [home, index, docs-home]
---

# SonarQube AI Auto-Fix — Documentation Index

This folder documents the end-to-end SonarQube AI auto-remediation system built on GitHub Actions. The system scans C# source code with SonarQube, batches findings by file, sends each batch to an AI API for remediation, and opens a pull request with the fixes — fully automated from a push to the `dev` branch.

---

## Documents

| File | What it covers |
|------|----------------|
| [[01-architecture]] | Full system design, data flow diagram, key design decisions |
| [[02-sonarqube-local-setup]] | Running SonarQube locally with Docker, scanning, querying the REST API |
| [[03-test-fixture]] | The `SonarTestBadCode` project — its purpose, structure, and the 27 rules it violates |
| [[04-workflow-batch]] | `sonar-batch.yml` — scan + batch workflow, inputs, steps, branch strategy |
| [[05-workflow-fix]] | `sonar-fix-v3.yml` — AI fix workflow, three-job structure |
| [[06-workflow-verify]] | `sonar-verify.yml` — build verification on fix PRs |
| [[07-scripts]] | All Python scripts: fetch, batch, fix, create_pr, constants |
| [[08-ai-prompts]] | The AI system prompt and rule-by-rule remediation guide |
| [[09-secrets-and-config]] | Every secret, environment variable, and workflow input |
| [[10-sonar-rules]] | All 27 SonarQube rules, complexity tiers, and fix strategies |
| [[11-branch-strategy]] | Branch naming convention, PR flow, `dev` → `agent-queue` → fix |
| [[12-hallucination-mitigation]] | Why AI hallucination happens here and how the system defends against it |
| [[13-troubleshooting]] | Known errors, failure modes, and fixes |

---

## Quick orientation

```
push to dev
   └─► sonar-batch.yml        ← scan + batch, opens PR to agent-queue
         └─► agent-queue branch (latest code + batches.json)
               └─► sonar-fix-v3.yml   ← AI loop, opens PR with fixes
                     └─► ai/sonarqube-fixes-{N}  ← fix PR for human review
```

## Copilot: how to use this knowledge base

Ask questions like:
- "How does `batch_issues.py` group SonarQube issues?"
- "What secrets do I need to configure for the fix workflow?"
- "Why is the `agent` branch naming scheme different from `agent/fix`?"
- "What does exit code 2 mean in `fix_batch.py`?"
- "How does the AI fallback chain work?"
