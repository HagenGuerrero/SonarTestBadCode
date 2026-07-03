# Agent Context: SonarQube C# Auto-Remediation

## Background

This agent is part of an automated pipeline that fetches SonarQube findings from
a .NET Framework C# codebase and applies AI-generated fixes file by file. The
fixes are committed and submitted as a pull request for human review. A follow-up
SonarQube scan is the final quality gate — the goal here is to produce correct,
compilable code that eliminates the flagged violations without introducing new ones.

## Target environment

- Language: C# 7.x–8.x, compiled against .NET Framework 4.x
- Build tool: MSBuild (no `dotnet build`; no SDK-style csproj)
- Only BCL types available: `System.*`, `Microsoft.*` — no third-party NuGet packages
- Files are typically one class or interface per file
- Namespaces follow the pattern `CompanyName.Module.Subfolder`

## Input format

The user message will have this structure:

```
File: path/to/ClassName.cs

Issues to fix:
  Line 42: [csharpsquid:S1481] MINOR — Remove this unused local variable 'result'.
  Line 67: [csharpsquid:S125]  MAJOR — Remove this commented-out code.
  ...

File content:
<full source of the file>
```

Rule IDs always carry the `csharpsquid:` prefix. The line numbers refer to the
original file as provided — they do not update between issues in the same call.

---

## Rule reference

### S112 — Do not throw `System.Exception`

Replace `throw new Exception(...)` with the most semantically accurate type.

Common replacements:
- Parameter is null or invalid → `ArgumentNullException(nameof(param))` / `ArgumentException("msg", nameof(param))`
- Object is in wrong state → `InvalidOperationException("msg")`
- Feature not supported in this context → `NotSupportedException("msg")`
- I/O problem → `IOException("msg")`

```csharp
// Before
throw new Exception("Value cannot be null");
// After
throw new ArgumentNullException(nameof(value));
```

---

### S125 — Commented-out code

Remove the entire comment block that contains old/dead code. Do not remove
comments that explain reasoning, document a workaround, or are XML doc comments.

```csharp
// Before
// var result = OldMethod();
// result = result.Trim();
// After: (those lines are deleted entirely)
```

---

### S1066 — Collapsible `if` statements

Merge consecutive `if` statements that share no `else` into a single condition
with `&&`. Preserve existing indentation and brace style.

```csharp
// Before
if (x != null) {
    if (x.Length > 0) {
        Process(x);
    }
}
// After
if (x != null && x.Length > 0) {
    Process(x);
}
```

---

### S1104 — Field should be private with a public property

Change the field's access modifier from `public` to `private` and add an
auto-property (or a backing-field property) to expose it publicly.

```csharp
// Before
public string Name;
// After
private string _name;
public string Name { get { return _name; } set { _name = value; } }
```

If the file already has auto-property syntax elsewhere, prefer that style:
```csharp
public string Name { get; set; }
// (field declaration removed)
```

---

### S1116 — Empty statements

Remove the extra semicolons that produce empty statements.

```csharp
// Before
DoSomething();;
// After
DoSomething();
```

---

### S1118 — Utility class should not have a public constructor

Change `public ClassName() { }` to `private ClassName() { }`. The class has
only static members and must not be instantiated.

```csharp
// Before
public StringHelper() { }
// After
private StringHelper() { }
```

---

### S1125 — Boolean literal in comparison

Simplify boolean comparisons that compare to `true` or `false` literally.

```csharp
// Before
if (isValid == true) { ... }
if (flag == false) { ... }
// After
if (isValid) { ... }
if (!flag) { ... }
```

---

### S1144 — Unused private types or members

Remove the unused private field, method, or nested type entirely. Only remove
members that are explicitly listed in the issues. Verify the member is `private`
before deleting it — do not remove internal or protected members.

---

### S1172 — Unused method parameters

Prefix the unused parameter name with an underscore. Prefer underscore over
removal when the method overrides a base class method or implements an interface,
since changing the signature would break the contract.

```csharp
// Before
public void Handle(string input, int count) { Log(input); }
// After (count is listed as unused)
public void Handle(string input, int _count) { Log(input); }
```

---

### S1186 — Empty method body

Add a minimal comment inside the body to document the intentional emptiness.
Do not throw `NotImplementedException` for S1186 — that is a separate issue
(S3717). Use a comment instead.

```csharp
// Before
public void Initialize() { }
// After
public void Initialize() { /* intentionally empty */ }
```

---

### S1192 — Duplicate string literals

Extract the repeated string to a `private const string` field near the top of
the class. Replace ALL occurrences in the file (not just the flagged line) with
the constant name.

```csharp
// Before (string "active" appears 4 times)
if (status == "active") { ... }
// After
private const string StatusActive = "active";
...
if (status == StatusActive) { ... }
```

---

### S1481 — Unused local variable

Remove the variable declaration and its assignment. If the right-hand side
expression has a side effect worth preserving, keep the call but discard
the result.

```csharp
// Before
var unused = ComputeValue();
// After (no side effect worth keeping)
// (line removed)

// Before
var count = GetCount(); // GetCount updates internal state
// After (side effect matters)
GetCount();
```

---

### S1643 — String concatenation in a loop

Replace `+=` string concatenation inside any loop (`for`, `foreach`, `while`,
`do`) with a `System.Text.StringBuilder`. Declare the builder before the loop,
call `.Append()` inside, and call `.ToString()` after the loop.

```csharp
// Before
string result = "";
foreach (var item in items) { result += item + ", "; }
// After
var sb = new System.Text.StringBuilder();
foreach (var item in items) { sb.Append(item).Append(", "); }
string result = sb.ToString();
```

Add `using System.Text;` at the top if the `StringBuilder` type is not already
referenced in the file.

---

### S1764 — Identical expressions on both sides of an operator

The expression is suspicious: `a == a` is always true, `b < b` is always false,
`x ^ x` is always zero. Fix by correcting the likely variable typo.

```csharp
// Before
if (minValue == minValue) { ... }  // probably meant (minValue == maxValue)
// After — only if the intended variable is obvious from surrounding context
if (minValue == maxValue) { ... }
```

If the correct variable is not obvious, leave the issue unfixed.

---

### S1854 — Dead store to a local variable

An assigned value is never read before being overwritten or going out of scope.
Remove the useless assignment or the entire variable if it is never read.

```csharp
// Before
int count = 0;        // never read; overwritten on next line
count = GetCount();
return count;
// After
int count = GetCount();
return count;
```

---

### S1871 — Two branches in an `if`/`switch` with identical implementation

Merge branches that do the same thing, or eliminate the conditional entirely.

```csharp
// Before
if (condition) { return "active"; }
else { return "active"; }
// After
return "active";
```

---

### S2221 — Catching `System.Exception` too broadly

Replace `catch (Exception)` or `catch (Exception ex)` with the most specific
exception type appropriate to the `try` block's content.

**Caution:** Only change this when the correct specific type is unambiguous from
the `try` block. If uncertain, leave the issue unfixed — a wrong type can
swallow exceptions silently.

```csharp
// Before
try { File.ReadAllText(path); } catch (Exception ex) { Log(ex); }
// After
try { File.ReadAllText(path); } catch (IOException ex) { Log(ex); }
```

---

### S2223 / S2696 — Instance method writes to a static field

Either make the method `static` (preferred when the method does not use `this`),
or change the affected field to an instance field.

```csharp
// Before (instance method assigns to static field)
public void SetName(string name) { _staticName = name; }
// After option A — make method static
public static void SetName(string name) { _staticName = name; }
// After option B — make field instance
private string _instanceName;
public void SetName(string name) { _instanceName = name; }
```

Choose option A if the method does not reference `this` anywhere. Choose
option B if it does.

---

### S2306 — C# contextual keyword used as an identifier

Rename the identifier (variable, parameter, method, or class) that uses a
contextual keyword (`async`, `await`, `yield`, `dynamic`, `var`, `get`, `set`,
`add`, `remove`, `value`, `partial`, `where`, `from`, `select`, etc.) as its
name. Choose a descriptive replacement that conveys the same meaning.

```csharp
// Before
private string async;               // 'async' is a contextual keyword
public void Process(int yield) { }  // 'yield' is a contextual keyword
// After
private string asyncOperation;
public void Process(int yieldValue) { }
```

Update all references to the renamed identifier within the file.

---

### S2386 — Mutable `public static` field

Add `readonly` to make the field immutable after initialization. If the field
is only used within the same file, also consider making it `private`.

```csharp
// Before
public static List<string> Items = new List<string>();
// After
public static readonly List<string> Items = new List<string>();
```

Note: `readonly` on a collection prevents reassignment but not mutation of the
collection's contents. This is the correct fix for the rule.

---

### S2583 — Boolean expression is always `false`

The condition can never be true (e.g., `x < 0 && x >= 0`). Remove the dead
branch entirely or correct the logic error. Only fix when the correct logic is
unambiguous.

---

### S2589 — Boolean expression is always `true`

The condition is always satisfied (e.g., `x != null || true`, `count == count`).
Simplify by removing the redundant operand. Only fix when unambiguous.

---

### S3400 — Method always returns the same constant

If the method is `private` or `internal`, inline the constant at call sites
visible within this file. If it is `public`, leave it unfixed — callers outside
this file may depend on it.

```csharp
// Before (private)
private int GetTimeout() { return 30; }
// After (if all callers are in this file)
// (method removed; call sites replaced with literal 30)
```

---

### S3717 — `NotImplementedException` thrown

Implement the method body with real logic if the intent is clear from the method
name, parameters, and surrounding class context. If the correct implementation
requires knowledge that is not present in this file, leave the issue unfixed.

---

### S3963 — Static field initialized to its default value

Remove the explicit `= null`, `= 0`, `= false`, or `= default` initialization.
The CLR guarantees fields are zero-initialized without it.

```csharp
// Before
private static string _name = null;
private static int _count = 0;
private static bool _ready = false;
// After
private static string _name;
private static int _count;
private static bool _ready;
```

---

## Uncertainty protocol

When a fix requires information that is not present in the current file — the
correct replacement variable, the right exception type, the intended logic — do
not guess. Leave that specific issue untouched and continue with the remaining
issues in the list. It is better to skip a fix than to introduce a regression.

## What must never change

- Namespace declarations
- Class, interface, struct, and enum names
- Public and protected method signatures (name, parameter types, return type)
- XML documentation comments (`/// <summary>`)
- Attribute annotations (`[Authorize]`, `[DataMember]`, etc.)
- `#region` / `#endregion` blocks
- `using` directives that are already present and not made redundant by a fix
- `#if` / `#endif` preprocessor directives
