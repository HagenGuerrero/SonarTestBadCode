"""
Fetch SonarQube issues via REST API and write output/issues.json.

Usage:
    python scripts/sonar/fetch_issues.py

Environment variables (all required):
    SONARQUBE_URL       Base URL, e.g. http://localhost:9000
    SONARQUBE_TOKEN     User/project token (used as HTTP Basic username)
    SONAR_PROJECT_KEYS  Comma-separated project keys
    SONAR_SEVERITIES    Comma-separated severities, e.g. BLOCKER,CRITICAL,MAJOR
    SONAR_LANGUAGE      Language filter, e.g. cs
"""

import json
import os
import sys
import time
from datetime import datetime, timezone

import requests

sys.path.insert(0, os.path.join(os.path.dirname(__file__), ".."))
from shared.constants import (
    EXIT_CODE_FATAL,
    EXIT_CODE_SUCCESS,
    ISSUES_FILE,
    OUTPUT_DIR,
    SONAR_MAX_OFFSET,
    SONAR_PAGE_SIZE,
)

# ---------------------------------------------------------------------------
# Configuration from environment
# ---------------------------------------------------------------------------

SONARQUBE_URL = os.environ.get("SONARQUBE_URL", "http://localhost:9000").rstrip("/")
SONARQUBE_TOKEN = os.environ["SONARQUBE_TOKEN"]
PROJECT_KEYS = [k.strip() for k in os.environ.get("SONAR_PROJECT_KEYS", "").split(",") if k.strip()]
SEVERITIES = [s.strip() for s in os.environ.get("SONAR_SEVERITIES", "BLOCKER,CRITICAL,MAJOR").split(",") if s.strip()]
LANGUAGE = os.environ.get("SONAR_LANGUAGE", "cs")


def _session() -> requests.Session:
    s = requests.Session()
    s.auth = (SONARQUBE_TOKEN, "")
    return s


# ---------------------------------------------------------------------------
# Pagination helpers
# ---------------------------------------------------------------------------

def _paginate(session: requests.Session, params: dict) -> list[dict]:
    """Paginate /api/issues/search for a single (params) combination."""
    issues: list[dict] = []
    page = 1

    while True:
        params["p"] = page
        params["ps"] = SONAR_PAGE_SIZE
        resp = session.get(f"{SONARQUBE_URL}/api/issues/search", params=params, timeout=30)
        resp.raise_for_status()
        data = resp.json()

        issues.extend(data.get("issues", []))

        paging = data.get("paging", {})
        total = paging.get("total", 0)
        offset = paging.get("pageIndex", page) * SONAR_PAGE_SIZE

        print(f"    page {page}: +{len(data.get('issues', []))} issues (total declared: {total})")

        if offset >= total or offset >= SONAR_MAX_OFFSET:
            if offset >= SONAR_MAX_OFFSET and total > SONAR_MAX_OFFSET:
                print(f"    WARNING: hit 10 000-issue cap — decomposing by type")
                return None  # signal caller to decompose
            break
        page += 1

    return issues


def _fetch_by_type(session: requests.Session, base_params: dict) -> list[dict]:
    """Decompose a >10 000 query by issue type."""
    issues: list[dict] = []
    for itype in ("BUG", "VULNERABILITY", "CODE_SMELL"):
        params = {**base_params, "types": itype}
        print(f"  Decomposing by type={itype}")
        result = _paginate(session, params)
        if result is None:
            print(f"  WARNING: {itype} still > 10 000 — skipping decomposition-by-directory (not implemented)")
            continue
        issues.extend(result)
    return issues


def _fetch_project_severity(session: requests.Session, project_key: str, severity: str) -> list[dict]:
    base_params = {
        "projectKeys": project_key,
        "severities": severity,
        "languages": LANGUAGE,
        "resolved": "false",
    }

    print(f"  Fetching {project_key} / {severity}")
    result = _paginate(session, dict(base_params))

    if result is None:
        return _fetch_by_type(session, base_params)

    return result


# ---------------------------------------------------------------------------
# Main
# ---------------------------------------------------------------------------

def main() -> int:
    if not PROJECT_KEYS:
        print("ERROR: SONAR_PROJECT_KEYS is empty", file=sys.stderr)
        return EXIT_CODE_FATAL

    session = _session()
    all_issues: list[dict] = []
    seen_keys: set[str] = set()

    for project_key in PROJECT_KEYS:
        for severity in SEVERITIES:
            try:
                issues = _fetch_project_severity(session, project_key, severity)
            except requests.HTTPError as exc:
                print(f"ERROR: HTTP {exc.response.status_code} for {project_key}/{severity}: {exc}", file=sys.stderr)
                return EXIT_CODE_FATAL

            deduped = [i for i in issues if i["key"] not in seen_keys]
            seen_keys.update(i["key"] for i in deduped)
            all_issues.extend(deduped)
            print(f"  → {len(deduped)} unique issues added (running total: {len(all_issues)})")
            time.sleep(0.2)  # be polite to the server

    os.makedirs(OUTPUT_DIR, exist_ok=True)

    payload = {
        "fetched_at": datetime.now(timezone.utc).isoformat(),
        "total_fetched": len(all_issues),
        "project_keys": PROJECT_KEYS,
        "severities": SEVERITIES,
        "language": LANGUAGE,
        "issues": all_issues,
    }

    with open(ISSUES_FILE, "w", encoding="utf-8") as f:
        json.dump(payload, f, indent=2)

    print(f"\nWrote {len(all_issues)} issues to {ISSUES_FILE}")
    return EXIT_CODE_SUCCESS


if __name__ == "__main__":
    sys.exit(main())
