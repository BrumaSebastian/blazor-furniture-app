# Keycloak Realm Versioning Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Replace `extensions/realm-export.json` with versioned YAML files applied via keycloak-config-cli, so realm changes are reviewable in git.

**Architecture:** Split the `main` realm into 5 YAML files by concern under `extensions/keycloak/`. A shell script `apply-realm.sh` runs `adorsys/keycloak-config-cli` as a short-lived Docker container, mounting the YAML directory and connecting to the running local Keycloak instance. Files are processed in alphabetical (numbered) order.

**Tech Stack:** `adorsys/keycloak-config-cli:latest-26` Docker image, Keycloak 26.4.0, YAML, bash

---

## Background: keycloak-config-cli YAML format

Each YAML file must start with `realm: main`. Include only the keys that file owns — the tool merges files applied in order against the same realm. **Remove all UUID `id` fields** — the tool resolves by name. Use `$(env:VAR_NAME)` for environment-specific values.

The YAML structure mirrors the Keycloak Admin REST API / realm export JSON, but in YAML syntax. Most keys are identical.

Reference: https://github.com/adorsys/keycloak-config-cli

---

## Task 1: Create directory and `00-realm.yaml`

**Files:**
- Create: `extensions/keycloak/00-realm.yaml`

**Step 1: Create the directory**
```bash
mkdir -p extensions/keycloak
```

**Step 2: Write `00-realm.yaml`**

This file owns all scalar realm settings. Copy values from `extensions/realm-export.json`. Do NOT include `roles`, `clients`, `authenticationFlows`, `components`, `identityProviders`, `users`, `groups`, `scopeMappings`, `clientScopeMappings`.

```yaml
realm: main
displayName: "Blazor Management"
displayNameHtml: "Blazor Management"
enabled: true
sslRequired: external

# Login settings
registrationAllowed: true
registrationEmailAsUsername: false
rememberMe: true
verifyEmail: false
loginWithEmailAllowed: true
duplicateEmailsAllowed: false
resetPasswordAllowed: true
editUsernameAllowed: false

# Token lifespans (seconds)
accessTokenLifespan: 300
accessTokenLifespanForImplicitFlow: 900
ssoSessionIdleTimeout: 1800
ssoSessionMaxLifespan: 36000
offlineSessionIdleTimeout: 2592000
offlineSessionMaxLifespanEnabled: false
offlineSessionMaxLifespan: 5184000
accessCodeLifespan: 60
accessCodeLifespanUserAction: 86400
accessCodeLifespanLogin: 1800
actionTokenGeneratedByAdminLifespan: 43200
actionTokenGeneratedByUserLifespan: 300

# Security
defaultSignatureAlgorithm: RS256
bruteForceProtected: false
permanentLockout: false
failureFactor: 30
maxFailureWaitSeconds: 900
minimumQuickLoginWaitSeconds: 60
waitIncrementSeconds: 60
maxDeltaTimeSeconds: 43200

# SMTP — use env vars so this works across environments
smtpServer:
  host: $(env:SMTP_HOST,maildev)
  port: $(env:SMTP_PORT,1025)
  from: $(env:SMTP_FROM,something@no-reply.com)
  ssl: false
  starttls: false
  auth: false

# Themes
loginTheme: ""
accountTheme: ""
adminTheme: ""
emailTheme: ""

# Localization
internationalizationEnabled: true
supportedLocales:
  - en
  - ro
defaultLocale: en
```

**Step 3: Verify the file is valid YAML**
```bash
docker run --rm -v "$(pwd)/extensions/keycloak:/config" \
  adorsys/keycloak-config-cli:latest-26 \
  --keycloak.url=http://host.docker.internal:8080 \
  --keycloak.user=admin \
  --keycloak.password=admin \
  --import.path=/config/00-realm.yaml \
  --import.validate-only=true 2>&1 | tail -20
```
Expected: no fatal errors (validation-only mode).

**Step 4: Commit**
```bash
git add extensions/keycloak/00-realm.yaml
git commit -m "feat: add realm base settings YAML"
```

---

## Task 2: `01-roles.yaml`

**Files:**
- Create: `extensions/keycloak/01-roles.yaml`

**Step 1: Write `01-roles.yaml`**

Source: `roles.realm` and `roles.client` in `realm-export.json`. Omit built-in roles (`offline_access`, `uma_authorization`, `default-roles-main`) — Keycloak creates those automatically. Remove all `id`, `containerId` fields.

```yaml
realm: main

roles:
  realm:
    - name: user
      description: "Represents a simple user of the application"
      composite: false

    - name: admin
      description: "Represent an admin of the application"
      composite: true
      composites:
        client:
          server-client:
            - dashboard-management
            - groups-create
            - groups-list

    - name: group-admin
      description: "Represents an admin of a group"
      composite: true
      composites:
        client:
          server-client:
            - group-users-remove
            - dashboard-management
            - group-users-add
            - groups-update
            - group-users-list
            - groups-read
            - group-users-update

    - name: group-member
      description: "Represents a member of a group"
      composite: true
      composites:
        client:
          server-client:
            - dashboard-management
            - group-users-list
            - groups-read
```

> Note: `server-client` client roles are defined in `02-clients.yaml`. The tool applies files in order so client roles will exist before composite role mappings are resolved.

**Step 2: Commit**
```bash
git add extensions/keycloak/01-roles.yaml
git commit -m "feat: add realm roles YAML"
```

---

## Task 3: `02-clients.yaml`

**Files:**
- Create: `extensions/keycloak/02-clients.yaml`

**Step 1: Write `02-clients.yaml`**

Source: `clients` array in `realm-export.json`. Include only the custom/application clients. Skip internal Keycloak clients: `account`, `account-console`, `admin-cli`, `broker`, `realm-management`, `security-admin-console`.

Include: `blazor-app-client`, `server-client` (has authorization services), `scalar-client`, `integration-test-client`.

For the `server-client` authorization config: copy the full `authorizationSettings` block from `realm-export.json` — this contains resources, scopes, policies (the GroupTargetPolicy). Remove all `id` fields recursively.

```yaml
realm: main

clients:
  - clientId: blazor-app-client
    name: blazor-app-client
    protocol: openid-connect
    publicClient: true
    standardFlowEnabled: true
    directAccessGrantsEnabled: false
    redirectUris:
      - $(env:APP_REDIRECT_URI,https://localhost:7126/*)
      - $(env:APP_REDIRECT_URI_HTTP,http://localhost:5126/*)
    webOrigins:
      - $(env:APP_ORIGIN,https://localhost:7126)
    attributes:
      pkce.code.challenge.method: S256

  - clientId: scalar-client
    name: scalar-client
    protocol: openid-connect
    publicClient: true
    standardFlowEnabled: true
    directAccessGrantsEnabled: false
    redirectUris:
      - $(env:SCALAR_REDIRECT_URI,https://localhost:7126/scalar/oauth2-redirect)
    webOrigins:
      - $(env:APP_ORIGIN,https://localhost:7126)
    attributes:
      pkce.code.challenge.method: S256

  - clientId: integration-test-client
    name: integration-test-client
    protocol: openid-connect
    publicClient: true
    standardFlowEnabled: false
    directAccessGrantsEnabled: true

  - clientId: server-client
    name: server-client
    protocol: openid-connect
    publicClient: false
    serviceAccountsEnabled: true
    authorizationServicesEnabled: true
    # Copy full authorizationSettings from realm-export.json here,
    # removing all "id" fields. This includes:
    #   - resources (group-management, user-management, settings)
    #   - scopes (view, list, create, update, delete)
    #   - policies (GroupTargetPolicy, role policies)
    #   - permissions
    # See realm-export.json clients[].authorizationSettings for the full content.
    roles:
      - name: dashboard-management
        description: "Permission to view Management Dashboard"
      - name: groups-create
        description: "Permission to create a group"
      - name: groups-list
        description: "Permission to retrieve groups"
      - name: groups-read
        description: "Permission to retrieve group details"
      - name: groups-update
        description: "Permission to update a group"
      - name: group-users-add
        description: "Permission to add a user to a group"
      - name: group-users-remove
        description: "Permission to remove a user from group"
      - name: group-users-list
        description: "Permission to retrieve all users of a group"
      - name: group-users-update
        description: "Permission to update a user within a group"
      - name: uma_protection
```

> **Important for `authorizationSettings`:** Open `realm-export.json`, find the `server-client` entry, copy the entire `authorizationSettings` object, convert to YAML (any online JSON→YAML converter works), and paste under `server-client`. Remove every `"id"` key.

**Step 2: Commit**
```bash
git add extensions/keycloak/02-clients.yaml
git commit -m "feat: add clients YAML including server-client authorization config"
```

---

## Task 4: `03-flows.yaml`

**Files:**
- Create: `extensions/keycloak/03-flows.yaml`

**Step 1: Write `03-flows.yaml`**

Source: `authenticationFlows` and `authenticatorConfig` in `realm-export.json`. The realm uses standard Keycloak flows without customization (21 flows, all built-in). For this reason, the file only needs to declare the active flow bindings — Keycloak's defaults cover the rest.

```yaml
realm: main

browserFlow: browser
registrationFlow: registration
directGrantFlow: direct grant
resetCredentialsFlow: reset credentials
clientAuthenticationFlow: clients
dockerAuthenticationFlow: docker auth
firstBrokerLoginFlow: first broker login
```

> If you later customize an authentication flow (e.g., add WebAuthn or passwordless), add the full flow definition here with `authenticationFlows`.

**Step 2: Commit**
```bash
git add extensions/keycloak/03-flows.yaml
git commit -m "feat: add authentication flows YAML"
```

---

## Task 5: `04-components.yaml`

**Files:**
- Create: `extensions/keycloak/04-components.yaml`

**Step 1: Write `04-components.yaml`**

Source: `components` in `realm-export.json`. Covers key providers and user profile config. Remove all `id` fields.

The `components` block in keycloak-config-cli YAML uses the provider type as the key:

```yaml
realm: main

components:
  org.keycloak.userprofile.UserProfileProvider:
    - providerId: declarative-user-profile
      config:
        kc.user.profile.config:
          - '{"attributes":[{"name":"username","displayName":"${username}","validations":{"length":{"min":3,"max":255},"username-prohibited-characters":{},"up-username-not-idn-homograph":{}},"permissions":{"view":["admin","user"],"edit":["admin","user"]},"multivalued":false},{"name":"email","displayName":"${email}","validations":{"email":{},"length":{"max":255}},"required":{"roles":["user"]},"permissions":{"view":["admin","user"],"edit":["admin","user"]},"multivalued":false},{"name":"firstName","displayName":"${firstName}","validations":{"length":{"max":255},"person-name-prohibited-characters":{}},"required":{"roles":["user"]},"permissions":{"view":["admin","user"],"edit":["admin","user"]},"multivalued":false},{"name":"lastName","displayName":"${lastName}","validations":{"length":{"max":255},"person-name-prohibited-characters":{}},"required":{"roles":["user"]},"permissions":{"view":["admin","user"],"edit":["admin","user"]},"multivalued":false}],"groups":[],"unmanagedAttributePolicy":"DISABLED"}'
```

> Copy the exact user profile JSON string from `realm-export.json` under `components["org.keycloak.userprofile.UserProfileProvider"][0].config["kc.user.profile.config"][0]`.

**Step 2: Commit**
```bash
git add extensions/keycloak/04-components.yaml
git commit -m "feat: add components YAML (user profile, key providers)"
```

---

## Task 6: Write `apply-realm.sh`

**Files:**
- Create: `apply-realm.sh`

**Step 1: Write the script**

```bash
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
```

**Step 2: Make it executable**
```bash
chmod +x apply-realm.sh
```

**Step 3: Commit**
```bash
git add apply-realm.sh
git commit -m "feat: add apply-realm.sh script for keycloak-config-cli"
```

---

## Task 7: End-to-end test

**Prerequisites:** Aspire running (`dotnet run --project Aspire/BlazorFurniture.AppHost`) and Keycloak accessible at `http://localhost:8080`.

**Step 1: Pull the Docker image first**
```bash
docker pull adorsys/keycloak-config-cli:latest-26
```

**Step 2: Run against a fresh realm**

To test idempotency properly, delete and recreate the `main` realm in Keycloak Admin UI (`http://localhost:8080/admin`), then apply:
```bash
./apply-realm.sh
```
Expected output: lines showing each file being imported, no `ERROR` lines, ends with `Done.`

**Step 3: Verify in Keycloak Admin UI**

Check these manually at `http://localhost:8080/admin/master/console/#/main`:
- Realm Settings → correct display name, token lifespans, SMTP config
- Roles → `user`, `admin`, `group-admin`, `group-member` present with correct composites
- Clients → `blazor-app-client`, `server-client`, `scalar-client`, `integration-test-client` present
- `server-client` → Authorization tab → Resources, Scopes, Policies present
- Authentication → flows bound correctly

**Step 4: Test idempotency — run again, nothing should change**
```bash
./apply-realm.sh
```
Expected: same output as first run, no errors. The tool is idempotent by design.

**Step 5: Commit final state**
```bash
git add -A
git commit -m "feat: keycloak realm versioning with keycloak-config-cli"
```

---

## Task 8: Update CLAUDE.md and docs

**Files:**
- Modify: `CLAUDE.md`

**Step 1: Add a section to CLAUDE.md**

Add under the existing commands section:

```markdown
## Keycloak Realm Config

Realm configuration is versioned as YAML in `extensions/keycloak/`. To apply to a running local Keycloak:

```bash
./apply-realm.sh
# Override defaults:
KEYCLOAK_URL=http://localhost:8080 KEYCLOAK_USER=admin KEYCLOAK_PASSWORD=admin ./apply-realm.sh
```

When making realm changes: update the relevant YAML file, run `./apply-realm.sh` to verify, then commit.
```

**Step 2: Commit**
```bash
git add CLAUDE.md
git commit -m "docs: document keycloak realm versioning workflow"
```
