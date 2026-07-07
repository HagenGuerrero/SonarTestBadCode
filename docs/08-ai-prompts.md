---
title: AI Prompts Reference
tags: [ai, prompts, system-prompt, rules, csharp, remediation]
aliases: [prompts, ai-prompts, system-prompt, agent-prompt]
---

# AI Prompts Reference

The AI fix loop (`fix_batch.py`) loads two prompt files from `scripts/ai/prompts/` and concatenates them into a single system prompt. The user prompt is built dynamically per file.

---

## System prompt — `scripts/ai/prompts/system_prompt.md`

The behavioral contract. Tells the AI what to do, what not to do, and the exact output format required.

### Behavioral rules

1. Fix ONLY the issues explicitly listed. Do not refactor, rename, reformat, reorder, or improve any code not directly required by a listed issue.
2. Preserve all existing comments, blank lines, whitespace, and indentation that are not part of a fix.
3. If a safe, unambiguous fix cannot be determined — leave that specific code exactly as-is, mark it `SKIPPED, COULDN'T FIND A VIABLE FIX` in the FIXES section, and move on.
4. Do not add `using` directives unless a fix strictly requires a new type that is not already imported.
5. Do not change public method signatures, class names, interface contracts, or namespace declarations.
6. Do not merge or modify issues that are not in the list.

### Required output format

```
<CODE>
(complete corrected file content — first line to last, no omissions)
</CODE>
<FIXES>
(one line per issue in the same order they were listed)
</FIXES>
```

No text before, between, or after these two sections.

### CODE section rules

- Full file from first to last line — no truncation, no summaries
- No markdown fences inside (no triple backticks)
- Must be syntactically valid, compilable C#
- Do not add comments explaining what changed

### FIXES section format

```
S1481:42: Removed unused variable 'result'.
S125:67: Deleted commented-out code block lines 67–71.
S1192:89: Extracted repeated literal "active" to const StatusActive.
S2221:133: SKIPPED — catch type ambiguous without caller context.
```

- One line per issue, ordered as listed
- Strip the `csharpsquid:` prefix (`S1481`, not `csharpsquid:S1481`)
- Keep each line under 12 words
- Applied: `RULE:LINE: brief description`
- Skipped: `RULE:LINE: SKIPPED — one-phrase reason`

---

## Agent/rule reference — `scripts/ai/prompts/agent.md`

Rule-by-rule remediation instructions. Appended to the system prompt to give the AI precise guidance for each SonarQube rule.

### Target environment

- C# 7.x–8.x, .NET Framework 4.x, MSBuild
- BCL types only (`System.*`, `Microsoft.*`)
- One class per file; namespaces follow `CompanyName.Module.Subfolder`
- Line numbers refer to the original file as provided

### Rule-by-rule instructions

| Rule | Instruction |
|------|-------------|
| **S112** | Replace `throw new Exception(...)` with the most accurate type: `ArgumentNullException(nameof(param))`, `ArgumentException("msg", nameof(param))`, `InvalidOperationException`, `NotSupportedException`, or `IOException` |
| **S125** | Delete the entire comment block containing old/dead code. Keep XML doc comments (`///`), reasoning notes, and workaround explanations |
| **S1066** | Merge nested `if` statements with no `else` into a single condition using `&&`. Preserve brace style |
| **S1104** | Change `public` field to `private`; expose via auto-property or backing-field property |
| **S1116** | Remove extra semicolons: `expr;;` → `expr;` |
| **S1118** | Change `public ClassName() { }` to `private ClassName() { }` for all-static utility classes |
| **S1125** | Remove boolean literal comparisons: `x == true` → `x`, `x == false` → `!x` |
| **S1144** | Remove the unused `private` field, method, or nested type. Confirm it is `private` before removing |
| **S1172** | Prefix unused parameter names with underscore (`count` → `_count`). Do not alter signatures of overrides or interface implementations |
| **S1186** | Add `/* intentionally empty */` inside the empty method body |
| **S1192** | Extract repeated string to a `private const string` near top of class and replace ALL occurrences |
| **S1481** | Remove the unused local variable and its assignment. If the right-hand side has a side effect, keep the call and discard the result |
| **S1643** | Replace `+=` loop concatenation with `StringBuilder`: declare before loop, `.Append()` inside, `.ToString()` after. Add `using System.Text;` if not present |
| **S1764** | Correct the likely variable typo (`a == a` → `a == b`). Skip if the intended variable is not obvious |
| **S1854** | Remove the dead store whose value is never read before being overwritten or going out of scope |
| **S1871** | Merge identical `if`/`switch` branches, or eliminate the conditional and keep the shared result |
| **S2221** | Narrow `catch (Exception)` to the specific type thrown inside the `try`. Skip if ambiguous |
| **S2223 / S2696** | If the instance method does not reference `this`, make it `static`. Otherwise change the `static` field it writes to an instance field |
| **S2386** | Add `readonly` to the mutable `public static` field |
| **S2583** | Remove or correct the dead branch whose condition is always false. Skip if ambiguous |
| **S2589** | Simplify the always-true condition by removing the redundant operand. Skip if ambiguous |
| **S3400** | For `private`/`internal`: inline the literal at call sites in this file and remove the method. For `public`: skip (callers outside this file depend on it) |
| **S3717** | Implement the method body if the intent is clear from name, parameters, and class context. Skip if the correct implementation requires knowledge outside this file |
| **S3963** | Remove explicit `= null`, `= 0`, `= false`, or `= default` initializers from static field declarations |

### Rules that are always skipped

| Rule | Why |
|------|-----|
| **S107** | Reducing parameter counts requires understanding all callers |
| **S134** | Restructuring deeply nested control flow requires understanding full business logic |
| **S138** | Splitting large methods requires understanding the full call contract |
| **S1541 / S3776** | Reducing cyclomatic/cognitive complexity requires architectural decisions |
| **S3358** | Unnesting ternary chains requires understanding all possible values and precedence |
| **S4144** | Merging identical methods requires understanding caller intent and potential future divergence |

### What must never change

- Namespace declarations
- Class, interface, struct, and enum names
- Public and protected method signatures (name, parameter types, return type)
- XML documentation comments (`/// <summary>`)
- Attribute annotations (`[Authorize]`, `[DataMember]`, `[HttpGet]`, etc.)
- `#region` / `#endregion` blocks
- Existing `using` directives not made redundant by a fix
- `#if` / `#endif` preprocessor directives

---

## User prompt structure

Built dynamically in `fix_batch.py` for each file × chunk:

```
File: SonarTestBadCode/Controllers/HomeController.cs

Issues to fix:
  Line 42: [csharpsquid:S1481] MAJOR — Remove the unused local variable 'result'.
  Line 67: [csharpsquid:S125]  MAJOR — Remove this commented-out code.
  Line 89: [csharpsquid:S1192] MINOR — Define a constant instead of duplicating this literal.

File content:
using System;
namespace SonarTestBadCode.Controllers
{
    public class HomeController
    {
        ...
    }
}
```

If a retry is triggered due to size ratio failure, an additional note is appended:

```
⚠️ You MUST return the COMPLETE file from the very first line to the very last.
Do NOT return partial content, summaries, or only the changed sections.
Every single line of the original file must appear in your response.
```

---

## AI model configuration

| Setting | Value |
|---------|-------|
| Model | `gpt-4o` |
| Temperature | `0.2` (low — determinism over creativity) |
| `max_tokens` | `min(max(4096, file_tokens + 1024), 16384)` — scales to file size |

---

## Related documents

- [[07-scripts#fix_batch.py]] — how prompts are loaded and used
- [[12-hallucination-mitigation]] — why temperature is low and issues are capped
- [[10-sonar-rules]] — rules in more depth with examples from the test fixture
