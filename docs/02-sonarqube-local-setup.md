---
title: SonarQube Local Instance — Setup & Commands
tags: [sonarqube, setup, docker, local, commands, rest-api]
aliases: [sonar-local, sonarqube-setup, local-sonar]
---

# SonarQube Local Instance — Setup & Commands

> [!IMPORTANT]
> This repo targets **SonarQube 9.9.x LTS**. Use `sonar.login` (not `sonar.token`) for scanner auth. Use `projectKeys` (not `componentKeys`) in REST API calls. These are breaking differences from 10.x.

---

## Start the local instance

```powershell
cd C:\repos\sonarqube
docker compose up -d
```

Open `http://localhost:9000`
Login: `admin` / `Admin1234!`

---

## Stop the instance

```powershell
cd C:\repos\sonarqube
docker compose stop
```

---

## Create a project

```powershell
curl.exe -u admin:Admin1234! -X POST "http://localhost:9000/api/projects/create" `
  -d "project=YOUR-PROJECT-KEY&name=Your Project Name"
```

---

## Generate a token

```powershell
curl.exe -u admin:Admin1234! -X POST "http://localhost:9000/api/user_tokens/generate" `
  -d "name=your-token-name"
```

> [!WARNING]
> Copy the `token` value immediately — SonarQube will not show it again.

---

## Run a scan

Run from the root of the project folder (where the `.sln` file is).

```powershell
dotnet sonarscanner begin /k:"YOUR-PROJECT-KEY" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="YOUR-TOKEN"
dotnet build
dotnet sonarscanner end /d:sonar.login="YOUR-TOKEN"
```

Exclude scripts and output folders from analysis:

```powershell
dotnet sonarscanner begin /k:"YOUR-PROJECT-KEY" `
  /d:sonar.host.url="http://localhost:9000" `
  /d:sonar.login="YOUR-TOKEN" `
  /d:sonar.exclusions="scripts/**,output/**"
```

---

## REST API — Query issues

Auth method: pass the token as the HTTP Basic **username** with an empty password.

```
Authorization: token:   (colon means empty password)
```

### All issues for a project (9.9.x syntax)

```powershell
curl.exe -u "YOUR-TOKEN:" `
  "http://localhost:9000/api/issues/search?projectKeys=YOUR-PROJECT-KEY&ps=500" `
  -o report_issues.json
```

> [!WARNING]
> Use `projectKeys` — not `componentKeys`. On 9.9.x, `componentKeys` with a project key returns an incomplete subset of issues.

### Filter by severity

```powershell
curl.exe -u "YOUR-TOKEN:" `
  "http://localhost:9000/api/issues/search?projectKeys=YOUR-PROJECT-KEY&severities=BLOCKER,CRITICAL&ps=500" `
  -o report_blocker_critical.json
```

### Filter by type

```powershell
# Only bugs
curl.exe -u "YOUR-TOKEN:" "http://localhost:9000/api/issues/search?projectKeys=YOUR-PROJECT-KEY&types=BUG&ps=500" -o bugs.json

# Only code smells
curl.exe -u "YOUR-TOKEN:" "http://localhost:9000/api/issues/search?projectKeys=YOUR-PROJECT-KEY&types=CODE_SMELL&ps=500" -o smells.json

# Only vulnerabilities
curl.exe -u "YOUR-TOKEN:" "http://localhost:9000/api/issues/search?projectKeys=YOUR-PROJECT-KEY&types=VULNERABILITY&ps=500" -o vulns.json
```

### Quality gate status

```powershell
curl.exe -u "YOUR-TOKEN:" `
  "http://localhost:9000/api/qualitygates/project_status?projectKey=YOUR-PROJECT-KEY" `
  -o quality_gate.json
```

### Metrics summary

```powershell
curl.exe -u "YOUR-TOKEN:" `
  "http://localhost:9000/api/measures/component?component=YOUR-PROJECT-KEY&metricKeys=bugs,vulnerabilities,code_smells,duplicated_lines_density,ncloc,alert_status" `
  -o metrics.json
```

---

## Pagination

SonarQube's REST API paginates at 500 issues per page (`ps=500`), with a hard cap of 10,000 total results. Use `p=1`, `p=2`, … to paginate.

When `paging.total > 10000`, decompose by issue type (BUG / VULNERABILITY / CODE_SMELL) as separate queries. See [[07-scripts#fetch_issues.py]] for the automated decomposition strategy.

---

## Import a custom quality profile

```powershell
curl.exe -u admin:Admin1234! -X POST `
  "http://localhost:9000/api/qualityprofiles/restore" `
  -F "backup=@sonar/quality-profile-cs.xml"
```

Set it as default for C#:

```powershell
curl.exe -u admin:Admin1234! -X POST `
  "http://localhost:9000/api/qualityprofiles/set_default" `
  --data-urlencode "language=cs" `
  --data-urlencode "qualityProfile=CI-Profile-cs"
```

---

## 9.9.x vs 10.x differences

| Feature | 9.9.x (this repo) | 10.x |
|---------|-------------------|------|
| Scanner auth param | `sonar.login` | `sonar.token` |
| REST API project filter | `projectKeys` | `componentKeys` |
| Default admin password | `admin` (must change on first login) | same |

---

## Related documents

- [[04-workflow-batch]] — automated scan in GitHub Actions
- [[09-secrets-and-config]] — token management in CI
