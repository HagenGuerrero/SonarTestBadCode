---
title: SonarQube Rules — Reference & Fix Strategies
tags: [sonarqube, rules, csharp, code-quality, remediation]
aliases: [rules, sonar-rules, rule-reference]
---

# SonarQube Rules — Reference & Fix Strategies

The 27 rules active in this project's quality profile, organized by complexity tier.

---

## Complexity tiers

Issues are grouped into three tiers based on how much reasoning an AI needs to fix them correctly. This tiers directly inform the hallucination mitigation strategy.

| Tier | Rules | Hallucination risk | Fix approach |
|------|-------|--------------------|--------------|
| **Simple** | S125, S1116, S1186, S1192, S1481, S1643, S1764, S3400, S3963, S3717 | Low — one-line changes with clear intent | AI fixes reliably |
| **Moderate** | S1066, S1118, S1144, S1172, S2386, S112, S1871, S2696 | Medium — require class-level context | AI fixes most instances |
| **Complex / Skip** | S107, S134, S138, S1541, S3358, S3776, S4144, S2221, S2583, S2589 | High — require call graph or business logic knowledge | Most are skipped by design |

---

## Tier 1 — Simple (mechanical, one-line)

### S125 — Commented-out code
**Pattern:** Multi-line `//` blocks of old code left in files.
**Fix:** Delete the entire dead-code comment block. Keep XML docs (`///`), workaround notes, and reasoning comments.

### S1116 — Empty statements
**Pattern:** `someCall();;` — double semicolons producing an empty statement.
**Fix:** Remove the extra semicolon.

### S1186 — Empty method bodies
**Pattern:** `public void Initialize() { }` — method with no body.
**Fix:** Add `/* intentionally empty */` inside the body. Do NOT throw — that is S3717.

### S1192 — Duplicate string literals
**Pattern:** Same string literal appears 3+ times in one file.
**Fix:** Extract to `private const string ConstName = "value";` near the top of the class and replace ALL occurrences in the file.

### S1481 — Unused local variables
**Pattern:** Variable declared and assigned but never read.
**Fix:** Remove the declaration and assignment. If the right-hand side has a side effect (method call), keep the call, discard the return value.

### S1643 — String concatenation in a loop
**Pattern:** `result += item` inside `for`/`foreach`/`while`.
**Fix:** Replace with `StringBuilder`: declare before loop, `.Append()` inside, `.ToString()` after. Add `using System.Text;` if missing.

### S1764 — Identical expressions on both sides of an operator
**Pattern:** `a == a`, `b < b` — tautologies or contradictions.
**Fix:** Correct the likely typo. Skip if the intended variable is not obvious from context.

### S3400 — Method always returns a constant
**Pattern:** `public int GetTimeout() { return 30; }`
**Fix:** For `private`/`internal` methods: inline the literal at all call sites in the file and remove the method. For `public` methods: skip (callers outside the file depend on the method).

### S3717 — `NotImplementedException` thrown
**Pattern:** `throw new NotImplementedException(...)`
**Fix:** Implement the method body if intent is clear from the name, parameters, and class context. Skip if the correct implementation requires knowledge outside this file.

### S3963 — Static field initialized to default value
**Pattern:** `private static string _x = null;`, `private static int _count = 0;`
**Fix:** Remove the explicit default initializer. CLR guarantees reference types default to null, value types to their zero value.

---

## Tier 2 — Moderate (class context required)

### S112 — `System.Exception` should not be thrown
**Pattern:** `throw new Exception("something went wrong")`
**Fix:** Replace with the most accurate specific type:
- `ArgumentNullException(nameof(param))` — null argument
- `ArgumentException("msg", nameof(param))` — invalid argument value
- `InvalidOperationException("msg")` — wrong state for the operation
- `NotSupportedException("msg")` — operation not supported
- `IOException("msg")` — I/O related

### S1066 — Collapsible `if` statements
**Pattern:**
```csharp
if (x != null) {
    if (x.Length > 0) {
        // ...
    }
}
```
**Fix:** Merge into one: `if (x != null && x.Length > 0)`. Preserve brace style.

### S1118 — Utility class with public constructor
**Pattern:** A class with only `static` members has a `public ClassName() { }` constructor.
**Fix:** Change the constructor to `private ClassName() { }`.

### S1144 — Unused private members
**Pattern:** A `private` field, method, or nested type that is never referenced in the file.
**Fix:** Remove it. Confirm it is `private` before removing.

### S1172 — Unused method parameters
**Pattern:** A parameter is declared but never read in the method body.
**Fix:** Prefix the parameter name with underscore: `count` → `_count`. Do NOT alter signatures of overrides or interface implementations.

### S1871 — Branches with identical implementations
**Pattern:**
```csharp
if (condition) return "same";
else return "same";
```
**Fix:** Eliminate the conditional and return the shared value directly.

### S2386 — Mutable `public static` fields
**Pattern:** `public static List<string> Items = new List<string>();`
**Fix:** Add `readonly`: `public static readonly List<string> Items = new List<string>();`

### S2696 — Instance method writes to a static field
**Pattern:** A non-static method assigns a value to a static field.
**Fix:** If the method does not reference `this`, make the method `static`. Otherwise, change the static field to an instance field.

---

## Tier 3 — Complex / Always skipped

### S2221 — Catching `Exception` too broadly
**Pattern:** `catch (Exception ex) { log(ex); }` — swallows all exception types.
**Risk:** Narrowing to the wrong type silently swallows unexpected exceptions in production.
**Decision:** Skipped unless the specific exception type is unambiguously determinable from the `try` block.

### S2583 — Boolean expression always false
**Pattern:** `x < 0 && x >= 0` — logically impossible.
**Risk:** The intended logic may be a guard that was miscoded. Removing it incorrectly can introduce bugs.
**Decision:** Skip if the intended correct condition is not obvious from context.

### S2589 — Boolean expression always true
**Pattern:** `x != null || true` — condition is always true.
**Decision:** Simplify only if the redundant operand is obviously dead. Skip if ambiguous.

### S107 — Too many parameters
**Pattern:** 8-parameter `Configure*` methods.
**Decision:** Always skipped. Reducing parameters requires understanding all callers.

### S134 — Control flow statements nested too deeply
**Pattern:** 5-level nested `if`/`for`/`switch` in `Evaluate*Strategy` methods.
**Decision:** Always skipped. Restructuring requires understanding full business logic.

### S138 — Method has too many lines
**Pattern:** `FlushAll*Buffers` methods with 80+ sequential statements.
**Decision:** Always skipped. Splitting requires understanding the full call contract.

### S1541 / S3776 — Cyclomatic / Cognitive complexity too high
**Pattern:** Same `Evaluate*Strategy` methods as S134.
**Decision:** Always skipped. Reducing complexity requires architectural decisions.

### S3358 — Nested ternary operators
**Pattern:** `x > 500 ? "critical" : x > 200 ? "high" : x > 50 ? "medium" : "low"`
**Decision:** Always skipped. Unnesting requires understanding all possible values.

### S4144 — Two methods with identical implementations
**Pattern:** `ComputeScoreA` and `ComputeScoreB` have identical bodies.
**Decision:** Always skipped. Merging requires understanding caller intent and whether future divergence is expected.

---

## Finding counts in `SonarTestBadCode`

| File | ~Issues |
|------|---------|
| `Controllers/HomeController.cs` | 68 |
| `Services/DataService.cs` | 74 |
| `Models/UserModel.cs` | 70 |
| `Utilities/StringHelper.cs` | 63 |
| `Repositories/UserRepository.cs` | 79 |
| `Helpers/ValidationHelper.cs` | 70 |
| `ProcessorClass.cs` | 57 |
| **Total** | **~481** |

---

## Related documents

- [[08-ai-prompts]] — per-rule AI instructions
- [[12-hallucination-mitigation]] — why Tier 3 rules increase hallucination risk
- [[03-test-fixture]] — how rules are instantiated in the test fixture
