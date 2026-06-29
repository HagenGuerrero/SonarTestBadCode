# SonarTestBadCode

A .NET Framework 4.8 class library intentionally written with code quality violations. It serves as the SonarQube scan target for an AI auto-fix GitHub Actions workflow — the violations are input data, not bugs.

---

## Purpose

The project produces a predictable set of ~270 SonarQube findings across 7 files. A GitHub Actions workflow (described in `plan.md`) fetches those findings, batches them by file, sends each batch to an AI API for remediation, and opens a PR with the fixes.

**Do not fix the violations manually.** They are the fixture data the workflow operates on.

---

## Project structure

```
SonarTestBadCode.sln
SonarTestBadCode/
  Controllers/HomeController.cs      (~39 findings)
  Services/DataService.cs            (~44 findings)
  Models/UserModel.cs                (~39 findings)
  Utilities/StringHelper.cs          (~35 findings)
  Repositories/UserRepository.cs     (~48 findings)
  Helpers/ValidationHelper.cs        (~40 findings)
  ProcessorClass.cs                  (~27 findings)
scripts/
  shared/constants.py                shared config (no magic strings)
  sonar/fetch_issues.py              SonarQube REST API paginator
  sonar/batch_issues.py              groups issues into batches
  ai/fix_batch.py                    calls AI API, writes fixes, commits
  git/create_pr.py                   pushes branch, opens PR
```

---

## SonarQube rules violated

| Rule | Description |
|------|-------------|
| S112 | `System.Exception` should not be thrown |
| S125 | Commented-out code |
| S1066 | Collapsible `if` statements |
| S1116 | Empty statements (`;;`) |
| S1118 | Utility class with public constructor |
| S1144 | Unused private members |
| S1172 | Unused method parameters |
| S1186 | Empty method bodies |
| S1192 | Duplicate string literals |
| S1481 | Unused local variables |
| S1643 | String concatenation in a loop |
| S1764 | Identical expressions on both sides of an operator |
| S1871 | Two branches with identical implementation |
| S2221 | Catching `Exception` too broadly |
| S2386 | Mutable `public static` fields |
| S2583 | Boolean expression is always `false` |
| S2589 | Boolean expression is always `true` |
| S2696 | Instance method writes to a `static` field |
| S3400 | Method returns only a constant |
| S3717 | `NotImplementedException` thrown |
| S3963 | Static field initialized to its default value |

---

## Prerequisites

- .NET Framework 4.8 SDK / MSBuild
- Python 3.12+
- Docker Desktop (for local SonarQube)
- `dotnet-sonarscanner` global tool

```powershell
dotnet tool install --global dotnet-sonarscanner
```

---

## Build

```powershell
msbuild SonarTestBadCode.sln /p:Configuration=Release
```

---

## Running a local SonarQube scan

### 1. Start SonarQube

```powershell
cd C:\repos\sonarqube
docker compose up -d
```

Open http://localhost:9000 — login: `admin` / `Admin1234!`

### 2. Generate a token

```powershell
curl.exe -u admin:Admin1234! -X POST "http://localhost:9000/api/user_tokens/generate" -d "name=test-token"
```

Copy the `token` value from the response.

### 3. Create the project (first time only)

```powershell
curl.exe -u admin:Admin1234! -X POST "http://localhost:9000/api/projects/create" `
  -d "project=sonar-test-bad-code&name=SonarTestBadCode"
```

### 4. Scan

```powershell
cd C:\repos\SonarTestBadCode

dotnet sonarscanner begin /k:"sonar-test-bad-code" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="YOUR-TOKEN"
dotnet build
dotnet sonarscanner end /d:sonar.login="YOUR-TOKEN"
```

---

## Running the batching scripts

### Fetch issues from SonarQube

```powershell
$env:SONARQUBE_URL      = "http://localhost:9000"
$env:SONARQUBE_TOKEN    = "YOUR-TOKEN"
$env:SONAR_PROJECT_KEYS = "sonar-test-bad-code"
$env:SONAR_SEVERITIES   = "BLOCKER,CRITICAL,MAJOR,MINOR,INFO"
$env:SONAR_LANGUAGE     = "cs"

python scripts/sonar/fetch_issues.py
```

Output: `output/issues.json`

### Batch issues by file

```powershell
$env:WORKSPACE_ROOT = "C:\repos\SonarTestBadCode"
$env:MAX_FILES      = "0"   # 0 = no cap

python scripts/sonar/batch_issues.py
```

Output: `output/batches.json`

### Batching strategy

Issues are batched in three tiers:

1. **Per-class** — one file per batch; all its issues go together so the AI has full context
2. **Per-type** — within each batch, issues are sorted by rule ID so violations of the same rule are adjacent
3. **Numeric** — `batch_id` 0…N assigned in descending severity order (most critical files first)

### Quick smoke test (no SonarQube needed)

```powershell
python -c "
import json, os
os.makedirs('output', exist_ok=True)
issues = [
    {'key':'k1','rule':'csharpsquid:S112','severity':'MAJOR','component':'proj:Controllers/HomeController.cs','line':10,'message':'msg','type':'CODE_SMELL'},
    {'key':'k2','rule':'csharpsquid:S1481','severity':'MINOR','component':'proj:Controllers/HomeController.cs','line':20,'message':'msg','type':'CODE_SMELL'},
    {'key':'k3','rule':'csharpsquid:S112','severity':'CRITICAL','component':'proj:Services/DataService.cs','line':5,'message':'msg','type':'CODE_SMELL'},
]
json.dump({'fetched_at':'2026-06-29','total_fetched':3,'issues':issues}, open('output/issues.json','w'))
print('Mock issues.json written')
"

$env:WORKSPACE_ROOT = "C:\repos\SonarTestBadCode"
python scripts/sonar/batch_issues.py
```

---

## Reference

- `plan.md` — full architecture of the AI auto-fix GitHub Actions workflow
- `COMMANDS.md` — quick reference for SonarQube local instance commands
