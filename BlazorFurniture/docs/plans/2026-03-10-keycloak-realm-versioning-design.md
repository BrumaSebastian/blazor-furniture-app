# Keycloak Realm Versioning Design

**Date:** 2026-03-10
**Tool:** [keycloak-config-cli](https://github.com/adorsys/keycloak-config-cli) by adorsys
**Approach:** Split YAML files per concern, applied manually via shell script

---

## Goal

Version the `main` Keycloak realm configuration in git so that:
- Changes are reviewable as meaningful diffs
- Any developer can reproduce the exact realm state from a fresh Keycloak instance
- The `realm-export.json` (opaque single-file blob) is replaced by structured YAML files

---

## Tool Overview

`keycloak-config-cli` connects to the Keycloak Admin REST API and applies realm configuration idempotently. When pointed at a directory, it processes YAML files in sorted (alphabetical) order, each treated as a partial realm import for the same realm. Supports `$(env:VAR_NAME)` variable substitution for environment-specific values.

Docker image: `adorsys/keycloak-config-cli:latest-26` (matches Keycloak 26.4.0 in use).

---

## File Structure

```
extensions/
  keycloak/
    00-realm.yaml            # Base settings: tokens, login, email, brute-force, OTP, SSL
    01-roles.yaml            # Realm roles, composite role mappings, scope mappings
    02-clients.yaml          # Client definitions and client scopes
    03-flows.yaml            # Authentication flows and authenticator config
    04-components.yaml       # Components: SPIs, Keycloak providers, authorization services
    05-identity-providers.yaml  # Identity providers and mappers (if any)
apply-realm.sh               # Script to apply all files against a running Keycloak
```

**Excluded from versioning:**
- `users` — runtime data, not configuration
- `groups` — created at runtime by application logic
- Internal Keycloak-generated IDs (stripped or ignored by the tool)

---

## YAML File Format

Each file declares `realm: main` and includes only its owned sections. Example structure for `01-roles.yaml`:

```yaml
realm: main
roles:
  realm:
    - name: admin
      description: "Platform administrator"
      composite: false
    - name: user
      description: "Regular user"
      composite: false
```

Variable substitution for environment-specific values:
```yaml
smtpServer:
  host: $(env:SMTP_HOST)
  port: $(env:SMTP_PORT)
  from: $(env:SMTP_FROM)
```

---

## `apply-realm.sh`

Located at the repo root. Behavior:

1. Reads config from env vars with documented defaults:
   - `KEYCLOAK_URL` (default: `http://localhost:8080`)
   - `KEYCLOAK_USER` (default: `admin`)
   - `KEYCLOAK_PASSWORD` (default: `admin`)
2. Runs `docker run --rm` mounting `extensions/keycloak/` as the import path
3. Passes credentials and URL as container env vars
4. Exits with a non-zero code if the tool reports errors

---

## Conversion from `realm-export.json`

The existing `realm-export.json` is the source of truth for the initial conversion. Sections map to files as follows:

| realm-export.json keys | Target file |
|---|---|
| All scalar settings (tokens, login, brute-force, OTP, SSL, themes) | `00-realm.yaml` |
| `roles`, `scopeMappings`, `clientScopeMappings` | `01-roles.yaml` |
| `clients`, `clientScopes`, `defaultDefaultClientScopes`, `defaultOptionalClientScopes` | `02-clients.yaml` |
| `authenticationFlows`, `authenticatorConfig`, `requiredActions` | `03-flows.yaml` |
| `components`, `clientPolicies`, `clientProfiles` | `04-components.yaml` |
| `identityProviders`, `identityProviderMappers` | `05-identity-providers.yaml` |

`realm-export.json` remains in `extensions/` for reference but is superseded by the YAML files.

---

## Local Testing

Prerequisites: Docker running, Keycloak running via Aspire (`dotnet run --project Aspire/BlazorFurniture.AppHost`).

```bash
# Apply realm config to local Keycloak
./apply-realm.sh

# Override defaults
KEYCLOAK_URL=http://localhost:8080 KEYCLOAK_USER=admin KEYCLOAK_PASSWORD=admin ./apply-realm.sh
```

To verify: open `http://localhost:8080/admin` → `main` realm → inspect clients, roles, flows.

---

## Workflow for Realm Changes

1. Make changes in the Keycloak Admin UI
2. Export the realm from the UI (or use the Keycloak export endpoint)
3. Update the relevant YAML file(s) in `extensions/keycloak/`
4. Run `./apply-realm.sh` against a fresh Keycloak to verify the config applies cleanly
5. Commit the YAML changes with a meaningful message
