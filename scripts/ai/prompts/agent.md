# SonarQube C# Remediation — Rule Reference

## Target environment

- C# 7.x–8.x, .NET Framework 4.x, MSBuild, BCL types only (`System.*`, `Microsoft.*`)
- One class per file; namespaces follow `CompanyName.Module.Subfolder`
- Line numbers in the issue list refer to the original file as provided

---

## Rules

**S112** — Replace `throw new Exception(...)` with the most accurate type: `ArgumentNullException(nameof(param))`, `ArgumentException("msg", nameof(param))`, `InvalidOperationException("msg")`, `NotSupportedException("msg")`, or `IOException("msg")`.

**S125** — Delete the entire comment block containing old/dead code. Keep XML doc comments (`///`), reasoning notes, and workaround explanations.

**S1066** — Merge nested `if` statements with no `else` into a single condition using `&&`. Preserve brace style.

**S1104** — Change `public` field to `private`; expose it via a public auto-property or backing-field property.

**S1116** — Remove the extra semicolons producing empty statements (`expr;;` → `expr;`).

**S1118** — Change `public ClassName() { }` to `private ClassName() { }` for utility classes whose members are all static.

**S1125** — Remove boolean literal comparisons: `x == true` → `x`, `x == false` → `!x`.

**S1144** — Remove the unused `private` field, method, or nested type. Confirm it is `private` before removing.

**S1172** — Prefix unused parameter names with underscore (`count` → `_count`). Do not alter signatures of overrides or interface implementations.

**S1186** — Add `/* intentionally empty */` inside the empty method body. Do not add a throw here — that is S3717.

**S1192** — Extract the repeated string to a `private const string` near the top of the class and replace ALL occurrences in the file.

**S1481** — Remove the unused local variable declaration and its assignment. If the right-hand side has a meaningful side effect, keep the call and discard the result.

**S1643** — Replace `+=` string concatenation inside any loop with `System.Text.StringBuilder`: declare before loop, `.Append()` inside, `.ToString()` after. Add `using System.Text;` if not already present.

**S1764** — Correct the likely variable typo (`a == a` → `a == b`). Skip if the intended variable is not obvious from context.

**S1854** — Remove the dead store whose assigned value is never read before being overwritten or going out of scope.

**S1871** — Merge identical `if`/`switch` branches, or eliminate the conditional entirely and keep the shared result.

**S2221** — Narrow `catch (Exception)` to the specific type thrown inside the `try` block. Skip if the correct type is ambiguous — a wrong type silently swallows exceptions.

**S2223 / S2696** — If the instance method does not reference `this`, make it `static`. Otherwise change the `static` field it writes to an instance field.

**S2306** — Rename identifiers that use C# contextual keywords (`async`, `await`, `yield`, `dynamic`, `var`, `get`, `set`, `value`, `partial`, etc.) to a descriptive alternative; update all references in the file.

**S2386** — Add `readonly` to the mutable `public static` field.

**S2583** — Remove or correct the dead branch whose condition is always false. Skip if the intended logic is ambiguous.

**S2589** — Simplify the always-true condition by removing the redundant operand. Skip if ambiguous.

**S3400** — For `private`/`internal` constant-return methods: inline the literal at call sites in this file and remove the method. For `public` methods: skip (callers outside this file depend on it).

**S3717** — Implement the method body if the intent is clear from the name, parameters, and surrounding class. Skip if the correct implementation requires knowledge outside this file.

**S3963** — Remove explicit `= null`, `= 0`, `= false`, or `= default` initializers from static field declarations.

**S107** — SKIP. Reducing parameter counts requires understanding all callers.

**S134** — SKIP. Restructuring deeply nested control flow requires understanding full business logic.

**S138** — SKIP. Splitting large methods requires understanding the full call contract.

**S1541 / S3776** — SKIP. Reducing cyclomatic/cognitive complexity requires architectural decisions.

**S3358** — SKIP. Unnesting ternary chains requires understanding all possible values and precedence.

**S4144** — SKIP. Merging identical methods requires understanding caller intent and potential future divergence.

---

## What must never change

- Namespace declarations
- Class, interface, struct, and enum names
- Public and protected method signatures (name, parameter types, return type)
- XML documentation comments (`/// <summary>`)
- Attribute annotations (`[Authorize]`, `[DataMember]`, `[HttpGet]`, etc.)
- `#region` / `#endregion` blocks
- `using` directives already present and not made redundant by a fix
- `#if` / `#endif` preprocessor directives
