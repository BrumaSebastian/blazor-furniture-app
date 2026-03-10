#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CONFIG_DIR="${SCRIPT_DIR}/extensions/keycloak"

KEYCLOAK_URL="${KEYCLOAK_URL:-http://localhost:8080}"
KEYCLOAK_USER="${KEYCLOAK_USER:-admin}"
KEYCLOAK_PASSWORD="${KEYCLOAK_PASSWORD:-admin}"

echo "Applying Keycloak realm config to ${KEYCLOAK_URL}..."
echo "Config directory: ${CONFIG_DIR}"

docker run --rm \
  --network host \
  -v "${CONFIG_DIR}:/config" \
  -e KEYCLOAK_URL="${KEYCLOAK_URL}" \
  -e KEYCLOAK_USER="${KEYCLOAK_USER}" \
  -e KEYCLOAK_PASSWORD="${KEYCLOAK_PASSWORD}" \
  -e IMPORT_PATH=/config \
  adorsys/keycloak-config-cli:latest-26 \
  --keycloak.url="${KEYCLOAK_URL}" \
  --keycloak.user="${KEYCLOAK_USER}" \
  --keycloak.password="${KEYCLOAK_PASSWORD}" \
  --import.path=/config \
  --import.var-substitution.enabled=true

echo "Done."
