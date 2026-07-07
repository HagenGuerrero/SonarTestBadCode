---
title: AI Hallucination Mitigation
tags: [ai, hallucination, validation, quality, fix-accuracy]
aliases: [hallucination, ai-quality, mitigation]
---

# AI Hallucination Mitigation

Hallucination in this context means: the AI returns code that looks plausible but is wrong — incorrect fixes, dropped lines, invented identifiers, or structurally broken C#.

---

## Why hallucination is a specific risk here

Two independent factors compound:

### Factor 1 — Issue count per call

When a single prompt contains too many simultaneous fix instructions, the model:
- Confuses line numbers across instructions
- Applies a fix to the wrong location
- Silently skips some issues
- Over-corrects unrelated lines to "clean up" the file

The `MAX_ISSUES_PER_CALL` parameter (default 15) directly caps this risk. Files with more issues are split into sequential sub-batches.

### Factor 2 — Rule complexity

Not all rules are equal in difficulty. Mixing mechanical rules (one-line changes) with logic-dependent rules (requiring knowledge of call graphs or invariants) in a single prompt confuses the model.

| Tier | Rules | Risk | Why |
|------|-------|------|-----|
| Simple | S125, S1116, S1186, S1192, S1481, S1643, S1764, S3400, S3963, S3717 | Low | Clear, mechanical one-line changes |
| Moderate | S1066, S1118, S1144, S1172, S2386, S112, S1871, S2696 | Medium | Need class-level understanding |
| Complex | S107, S134, S138, S1541, S3358, S3776, S4144, S2221, S2583, S2589 | High | Need call graphs, business logic, caller intent |

The issue cap indirectly helps even with complexity: a file with 3 complex violations and 12 simple ones gets at most 15 issues per call — the model isn't simultaneously overwhelmed by volume and complexity.

---

## Defense layer 1 — System prompt constraints

The system prompt explicitly forbids hallucination-adjacent behaviors:

- Fix ONLY the listed issues — do not refactor beyond the issue
- Do not add `using` directives unless required by a fix
- Do not change public method signatures, class names, or namespace declarations
- If a fix is ambiguous, skip it and mark it `SKIPPED` — do not guess

Temperature is set to `0.2` (near-deterministic). Code generation benefits from determinism over creativity.

---

## Defense layer 2 — Structured output format

The AI is required to use `<CODE>...</CODE>` and `<FIXES>...</FIXES>` tags. This prevents the model from mixing explanation prose with code, which is a common hallucination pattern.

The parser extracts only the content inside `<CODE>`. If tags are absent (model ignored the format), fallback parsing strips markdown fences.

---

## Defense layer 3 — Five-check heuristic validation

Applied to every AI response before writing any file:

| Check | What it catches |
|-------|-----------------|
| **Minimum length** (≥ 50 chars) | Completely empty or trivially short responses |
| **Size ratio** (0.3× to 2.5× original) | Truncated files (AI summarized instead of returning full content), or exploded responses (AI added extensive commentary) |
| **Namespace declaration present** | Complete file deletion or namespace rename — the most common catastrophic hallucination |
| **Brace balance** (diff ≤ 2) | Syntactically broken C# — unclosed classes or methods |
| **No markdown fences** | AI returned fenced code block instead of raw C# |

On failure: the original file is **not** overwritten. The batch is marked `skipped` in the fix results.

---

## Defense layer 4 — Sub-batch sequential reading

Each chunk in a sub-batched file reads the **already-modified** file state, not the original. This keeps the line numbers in subsequent prompts accurate. If the AI fixes lines 1–30 and shifts line numbers, the next chunk reads the shifted file and gets correct line references.

---

## Defense layer 5 — Retry with complete-file reminder

On size ratio failure (first attempt only), the system retries with an additional note appended to the user prompt:

```
⚠️ You MUST return the COMPLETE file from the very first line to the very last.
Do NOT return partial content, summaries, or only the changed sections.
Every single line of the original file must appear in your response.
```

If the second attempt also fails validation, the file is skipped.

---

## Defense layer 6 — Always-skip rules

Rules whose correct fix depends on information not available in a single file (caller context, business logic, architectural intent) are listed in `agent.md` as always-skip:

- S107, S134, S138, S1541, S3358, S3776, S4144

For these rules, the AI is instructed to mark them `SKIPPED` in the FIXES section without attempting a fix. This is safer than a plausible-but-wrong structural change.

---

## Defense layer 7 — Build verification (`sonar-verify.yml`)

After the fix PR is created, `sonar-verify.yml` runs `dotnet msbuild` on the fixed codebase. This catches any C# that passed heuristic validation but is still syntactically invalid (e.g., mismatched generic brackets that don't affect brace count).

---

## Defense layer 8 — Post-merge SonarQube re-scan

The authoritative quality gate. After merging a fix PR:
1. Trigger a new scan
2. Compare issue counts before and after
3. Issues that persist indicate the AI fix was ineffective
4. New issues that appear indicate the AI introduced a regression
5. Both cases surface as fresh issues in the next batch run

---

## What validation does NOT catch

- Logical errors in complex rules (wrong exception type narrowed, incorrect condition simplified)
- Subtle behavior regressions (variable renamed correctly but a serialized field name changes)
- Fixes that compile and pass SonarQube but are functionally wrong

These require human code review of the fix PR. The PR body includes a "review before merging" reminder and links each commit to a single file for easy per-file review.

---

## Tuning guidance

| Symptom | Adjustment |
|---------|------------|
| Many `size_ratio_suspicious` skips | Lower `MAX_ISSUES_PER_CALL` — prompts are too large, model truncates |
| Many `namespace_removed` failures | Same — model is losing the start of the file |
| Many `unbalanced_braces` failures | Lower temperature, check if rule mix includes S134/S138 |
| All files skipped (`exit_code=2`) | API unavailable, or ALL files failing validation — check logs for API error codes |
| Fixes compile but are wrong | Rules in Tier 3 — consider adding them to the always-skip list in `agent.md` |

---

## Related documents

- [[08-ai-prompts]] — the full system prompt and always-skip rule list
- [[07-scripts#fix_batch.py]] — validation implementation
- [[10-sonar-rules]] — complexity tier breakdown per rule
- [[09-secrets-and-config]] — `MAX_ISSUES_PER_CALL` tuning
