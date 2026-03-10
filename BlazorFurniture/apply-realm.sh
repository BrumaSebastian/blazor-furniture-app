#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CONFIG_DIR="${SCRIPT_DIR}/extensions/keycloak"

# On Windows (Git Bash), Docker needs paths in //c/... format
if [[ "$OSTYPE" == "msys" || "$OSTYPE" == "cygwin" ]]; then
  CONFIG_DIR="$(echo "$CONFIG_DIR" | sed 's|^/\([a-zA-Z]\)/|//\1/|')"
fi

KEYCLOAK_URL="${KEYCLOAK_URL:-http://host.docker.internal:8080}"
KEYCLOAK_USER="${KEYCLOAK_USER:-admin}"
KEYCLOAK_PASSWORD="${KEYCLOAK_PASSWORD:-admin}"

# Realm config defaults (override via env vars)
SMTP_HOST="${SMTP_HOST:-maildev}"
SMTP_PORT="${SMTP_PORT:-1025}"
SMTP_FROM="${SMTP_FROM:-something@no-reply.com}"
APP_REDIRECT_URI="${APP_REDIRECT_URI:-https://localhost:7126/*}"
APP_REDIRECT_URI_HTTP="${APP_REDIRECT_URI_HTTP:-http://localhost:5126/*}"
APP_ORIGIN="${APP_ORIGIN:-https://localhost:7126}"
SCALAR_REDIRECT_URI="${SCALAR_REDIRECT_URI:-https://localhost:7126/scalar/oauth2-redirect}"

echo "Applying Keycloak realm config to ${KEYCLOAK_URL}..."
echo "Config directory: ${CONFIG_DIR}"

# MSYS_NO_PATHCONV=1 prevents Git Bash from converting /config to a Windows path
MSYS_NO_PATHCONV=1 docker run --rm \
  -v "${CONFIG_DIR}:/config" \
  -e KEYCLOAK_URL="${KEYCLOAK_URL}" \
  -e KEYCLOAK_USER="${KEYCLOAK_USER}" \
  -e KEYCLOAK_PASSWORD="${KEYCLOAK_PASSWORD}" \
  -e IMPORT_PATH=/config \
  -e IMPORT_VAR_SUBSTITUTION_ENABLED=true \
  -e SMTP_HOST="${SMTP_HOST}" \
  -e SMTP_PORT="${SMTP_PORT}" \
  -e SMTP_FROM="${SMTP_FROM}" \
  -e APP_REDIRECT_URI="${APP_REDIRECT_URI}" \
  -e APP_REDIRECT_URI_HTTP="${APP_REDIRECT_URI_HTTP}" \
  -e APP_ORIGIN="${APP_ORIGIN}" \
  -e SCALAR_REDIRECT_URI="${SCALAR_REDIRECT_URI}" \
  adorsys/keycloak-config-cli:latest-26

echo "Done."
