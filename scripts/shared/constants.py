"""Shared constants for SonarQube AI auto-fix scripts."""

# Severity ranking (higher = more severe)
SEVERITY_SCORE = {
    "BLOCKER": 5,
    "CRITICAL": 4,
    "MAJOR": 3,
    "MINOR": 2,
    "INFO": 1,
}

SEVERITY_ORDER = ["BLOCKER", "CRITICAL", "MAJOR", "MINOR", "INFO"]

# SonarQube pagination
SONAR_PAGE_SIZE = 500
SONAR_MAX_OFFSET = 10_000  # SonarQube hard cap on paginated results

# Token estimation: ~4 UTF-8 bytes per token
TOKEN_CHARS_PER_TOKEN = 4
MAX_TOKENS_PER_FILE = 3_000  # files above this threshold are flagged large_file=true

# AI API endpoints
COPILOT_URL = "https://api.githubcopilot.com/chat/completions"
MODELS_URL = "https://models.inference.ai.azure.com/chat/completions"
AI_MODEL = "gpt-4o"

# Retry / rate-limit
AI_RETRY_MAX = 3
AI_INTER_FILE_DELAY_SEC = 3

# Response validation thresholds
VALIDATION_MIN_CHARS = 50
VALIDATION_MAX_SIZE_RATIO = 2.5
VALIDATION_MIN_SIZE_RATIO = 0.3

# Exit codes
EXIT_CODE_SUCCESS = 0
EXIT_CODE_ALL_SKIPPED = 2
EXIT_CODE_FATAL = 3

# Output paths (relative to repo root)
OUTPUT_DIR = "output"
ISSUES_FILE = "output/issues.json"
BATCHES_FILE = "output/batches.json"
FIX_RESULTS_DIR = "output/fix_results"
