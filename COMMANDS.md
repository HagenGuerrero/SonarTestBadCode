# SonarQube Local Instance — Command Reference

> **Version: SonarQube 9.9.x LTS**
> Use `sonar.login` (not `sonar.token`) for scanner auth. Use `projectKeys` (not `componentKeys`) in REST API calls.

## Start the instance

```powershell
cd C:\repos\sonarqube
docker compose up -d
```

Open http://localhost:9000 — login: `admin` / `Admin1234!`

---

## Create a project

```powershell
curl.exe -u admin:Admin1234! -X POST "http://localhost:9000/api/projects/create" -d "project=YOUR-PROJECT-KEY&name=Your Project Name"
```

---

## Generate a token

```powershell
curl.exe -u admin:Admin1234! -X POST "http://localhost:9000/api/user_tokens/generate" -d "name=your-token-name"
```

Copy the `token` value from the response — SonarQube will not show it again.

---

## Run a scan

Run from the root of the project folder (where the `.sln` is).
**9.9.x uses `sonar.login` — `sonar.token` is 10.x syntax and will fail here.**

```powershell
dotnet sonarscanner begin /k:"YOUR-PROJECT-KEY" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="YOUR-TOKEN"
dotnet build
dotnet sonarscanner end /d:sonar.login="YOUR-TOKEN"
```

To exclude the `scripts/` directory from analysis (avoids Python findings polluting the C# report):

```powershell
dotnet sonarscanner begin /k:"YOUR-PROJECT-KEY" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="YOUR-TOKEN" /d:sonar.exclusions="scripts/**"
```

---

## Get report as JSON

### All issues with type counts summary
> Use `projectKeys` (not `componentKeys`) — `componentKeys` returns an incomplete subset on 9.9.x.

```powershell
curl.exe -u "YOUR-TOKEN:" "http://localhost:9000/api/issues/search?projectKeys=YOUR-PROJECT-KEY&facets=types&ps=500" -o report_issues.json
```

### Only bugs
```powershell
curl.exe -u "YOUR-TOKEN:" "http://localhost:9000/api/issues/search?projectKeys=YOUR-PROJECT-KEY&types=BUG&ps=500" -o report_bugs.json
```

### Only code smells
```powershell
curl.exe -u "YOUR-TOKEN:" "http://localhost:9000/api/issues/search?projectKeys=YOUR-PROJECT-KEY&types=CODE_SMELL&ps=500" -o report_code_smells.json
```

### Only vulnerabilities
```powershell
curl.exe -u "YOUR-TOKEN:" "http://localhost:9000/api/issues/search?projectKeys=YOUR-PROJECT-KEY&types=VULNERABILITY&ps=500" -o report_vulnerabilities.json
```

### Quality gate status
```powershell
curl.exe -u "YOUR-TOKEN:" "http://localhost:9000/api/qualitygates/project_status?projectKey=YOUR-PROJECT-KEY" -o report_quality_gate.json
```

### Metrics summary (bugs, vulnerabilities, code smells, duplications)
```powershell
curl.exe -u "YOUR-TOKEN:" "http://localhost:9000/api/measures/component?component=YOUR-PROJECT-KEY&metricKeys=bugs,vulnerabilities,code_smells,duplicated_lines_density,ncloc,alert_status" -o report_metrics.json
```

---

## Stop the instance

```powershell
cd C:\repos\sonarqube
docker compose stop
```
