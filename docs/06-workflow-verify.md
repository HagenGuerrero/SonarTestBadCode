---
title: Workflow — sonar-verify.yml (Build Verification)
tags: [workflow, github-actions, build, verify, pr-check]
aliases: [verify-workflow, build-verify, sonar-verify]
---

# Workflow — `sonar-verify.yml` (Build Verification)

**File:** `.github/workflows/sonar-verify.yml`
**Name:** Build Verify

---

## Purpose

Verifies that the AI-generated fixes compile successfully. Triggered automatically on every pull request targeting `agent/fix`, so broken fixes are caught before any human review is needed.

---

## Trigger

```yaml
on:
  pull_request:
    branches:
      - agent/fix
    types: [opened, synchronize, reopened]
```

Fires on: PR opened, new commit pushed to the PR, PR reopened.

---

## Permissions

```yaml
permissions:
  contents: read
```

Read-only — this workflow only builds, it does not commit or create anything.

---

## Job — `build`

Runs on `ubuntu-latest`.

### Steps

**1. Checkout**
Checks out the PR head commit (the branch containing AI fixes).

**2. Install Mono MSBuild**
Installs from the official Mono Project apt repository (Ubuntu's default apt does not include `mono-msbuild`).

```bash
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 \
  --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb https://download.mono-project.com/repo/ubuntu stable-focal main" \
  | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
sudo apt-get update -qq
sudo apt-get install -y mono-devel
```

**3. Build**
```bash
dotnet msbuild SonarTestBadCode.sln /p:Configuration=Release
```

`FrameworkPathOverride=/usr/lib/mono/4.8-api/` is set so the .NET Framework 4.8 reference assemblies resolve correctly on Linux.

---

## What this catches

- Syntax errors introduced by the AI (mismatched braces, missing semicolons)
- Invalid C# that passed the heuristic validation in `fix_batch.py`
- Structural damage to a file (e.g., a complete class body accidentally removed)

## What this does NOT catch

- Logic errors (wrong variable used, wrong exception type)
- New SonarQube violations introduced by the fix
- Behavior regressions

A follow-up SonarQube scan after merge is the authoritative quality gate for those.

---

## Related documents

- [[05-workflow-fix]] — the fix workflow that produces the PRs this verifies
- [[07-scripts#fix_batch.py]] — heuristic validation that runs before this step
- [[12-hallucination-mitigation]] — full validation chain
