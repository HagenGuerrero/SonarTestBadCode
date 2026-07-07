---
title: SonarTestBadCode — Test Fixture
tags: [test-fixture, sonarqube, violations, csharp, dotnet]
aliases: [test-fixture, bad-code, sonar-fixture]
---

# SonarTestBadCode — Test Fixture

## Purpose

`SonarTestBadCode` is a .NET Framework 4.8 class library whose source files are **intentionally written with code quality violations**. It exists to give the AI auto-fix workflow a realistic, predictable set of SonarQube findings to operate on.

> [!CAUTION]
> **Never auto-fix violations in this project.** The violations are the input data. Fixing them removes the test fixture.

Expected findings: approximately **481 issues across 7 files**.

---

## Project structure

```
SonarTestBadCode.sln
SonarTestBadCode/
  SonarTestBadCode.csproj          ← .NET Framework 4.8, OutputType: Library
  Properties/AssemblyInfo.cs
  Controllers/HomeController.cs    ← ~68 findings
  Services/DataService.cs          ← ~74 findings
  Models/UserModel.cs              ← ~70 findings
  Utilities/StringHelper.cs        ← ~63 findings
  Repositories/UserRepository.cs   ← ~79 findings
  Helpers/ValidationHelper.cs      ← ~70 findings
  ProcessorClass.cs                ← ~57 findings
```

---

## Build

```powershell
msbuild SonarTestBadCode.sln /p:Configuration=Release
```

On Linux (GitHub Actions), via Mono MSBuild:

```bash
dotnet msbuild SonarTestBadCode.sln /p:Configuration=Release
```

The project compiles cleanly despite all violations — no syntax errors, only quality rule violations.

---

## Rules violated (27 distinct rules)

Rules are split into two groups: the original 20 mechanical/moderate-difficulty rules, and 7 rules added later to test the workflow's handling of structural/logical complexity.

### Group 1 — Original 20 (mechanical and moderate)

| Rule | Description | Pattern in code |
|------|-------------|-----------------|
| S112 | `System.Exception` should not be thrown | `throw new Exception(...)` in many methods |
| S125 | Commented-out code | Multi-line `//` blocks of old code |
| S1066 | Collapsible `if` statements | `if (x != null) { if (x.Length > 0) { ... } }` |
| S1116 | Empty statements | Double semicolons `;;` |
| S1118 | Utility class with public constructor | All-static class with `public ClassName() { }` |
| S1144 | Unused private members | Private fields/methods never referenced |
| S1172 | Unused method parameters | Declared parameters never read in body |
| S1186 | Empty method bodies | `public void Initialize() { }` |
| S1192 | Duplicate string literals | Same string literal 3+ times in one file |
| S1481 | Unused local variables | Declared and assigned but never read |
| S1643 | String concatenation in a loop | `result += item` inside `for`/`foreach` |
| S1764 | Identical expressions on both sides | `a == a`, `b < b` |
| S1871 | Two branches with identical implementation | `if (...) return "x"; else return "x";` |
| S2221 | Catching `Exception` too broadly | `catch (Exception)` with no rethrow |
| S2386 | Mutable `public static` fields | `public static List<T> Foo = new List<T>()` |
| S2583 | Boolean always false | `x < 0 && x >= 0` |
| S2589 | Boolean always true | `x != null \|\| true` |
| S2696 | Instance method writes to static field | `_staticField = value` in non-static method |
| S3400 | Method returns only a constant | `public int GetTimeout() { return 30; }` |
| S3717 | `NotImplementedException` thrown | `throw new NotImplementedException(...)` |
| S3963 | Static field initialized to default | `private static string _x = null;` |

### Group 2 — Added later (structural/logical complexity)

These rules specifically exercise the workflow's "hallucination mitigation" tiers because fixing them requires reasoning about call graphs, nesting depth, and duplicate logic.

| Rule | Description | Pattern in code |
|------|-------------|-----------------|
| S107 | Too many parameters | 8-parameter `Configure*` methods |
| S134 | Control flow nested too deeply | 5-level nested `if`/`for`/`switch` in `Evaluate*Strategy` |
| S138 | Method has too many lines | `FlushAll*Buffers` methods with 80+ sequential statements |
| S1541 | Cyclomatic complexity too high | Same `Evaluate*Strategy` methods |
| S3358 | Nested ternary operators | `Classify*Level` methods |
| S3776 | Cognitive complexity too high | Same `Evaluate*Strategy` methods |
| S4144 | Two methods with identical implementations | `Compute*ScoreA` / `Compute*ScoreB` pairs |

---

## SonarQube project key

`sonar-test-bad-code`

This is the key used in all workflow inputs and REST API calls when targeting this fixture.

---

## Adding more violations

Follow the patterns already in each file. Update the per-file estimates in this document and in `CLAUDE.md`.

Rules to target for new violations are the existing 27 rules — any rule not yet represented in a given file is fair game.

---

## Related documents

- [[10-sonar-rules]] — detailed fix strategies for each rule
- [[04-workflow-batch]] — how to scan this fixture in CI
- [[12-hallucination-mitigation]] — how Group 2 rules stress the AI
