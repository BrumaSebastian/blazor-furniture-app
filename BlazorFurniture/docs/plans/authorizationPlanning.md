# Keycloak Authorization Server — Planning (Controller-Scoped Resources + Java Policy)

Goal
- Use Keycloak Authorization Services (KCAS) to make authorization decisions with resources modeled per controller (e.g., `group-management`) and scopes like `view`, `list`, `update`, `delete`.
- Avoid per-group resource creation. Enforce group-based access via a reusable custom Java policy that evaluates the target group dynamically.

High-level design
- Resource server: API backend is a Keycloak client with Authorization enabled (audience = api-client-id).
- Resources represent controller areas (not instances): e.g., `group-management`, `user-management`, `settings`.
- Scopes represent operations: `view`, `list`, `create`, `update`, `delete`.
- Policy: a Java Policy SPI that reads the requested target group (e.g., `groupId`) from KC evaluation context attributes and decides based on user’s group membership and/or realm roles.
- Decision mode: prefer on-demand decision checks from the API (PDP call) with request attributes, to avoid issuing a separate RPT for every groupId variation.

Resource model (examples)
- group-management: scopes `view`, `list`, `update`, `delete`
- user-management: scopes `view`, `list`, `update`, `delete`
- settings: scopes `view`, `update`

Policies
- Static role policies
  - PlatformAdminPolicy: realm role `platform-admin`
  - AdminPolicy: realm role `admin`
- Java custom policy (reusable)
  - Name: `GroupTargetPolicy`
  - Config:
    - targetAttribute: `groupId` (evaluation context attribute to read)
    - groupBasePath: `/app` (KC group tree root)
    - adminSubPath: `admins`
    - memberSubPath: `members`
    - scopeRules (mapping):
      - `update`, `delete` → require group-admin OR admin/platform-admin
      - `view`, `list` → require group-member OR group-admin OR admin/platform-admin

How the policy works (Java SPI)
- Implement a provider via Keycloak Policy SPI (`PolicyProviderFactory` + `PolicyProvider`).
- On `evaluate`:
  - Read requested scopes from `evaluation.getPermission().getScopes()`.
  - Read `groupId` from `evaluation.getContext().getAttributes().getValue(targetAttribute)`.
  - Resolve the user (`evaluation.getContext().getIdentity()`) → `userId` → `UserModel` via `KeycloakSession`.
  - Check realm roles (`platform-admin`, `admin`). If matched and scope allowed, `evaluation.grant()`.
  - Else check KC group membership:
    - Admin path: `${groupBasePath}/${groupId}/admins`
    - Member path: `${groupBasePath}/${groupId}/members`
    - If scope ∈ {`update`,`delete`} require admin path; if scope ∈ {`view`,`list`} require member OR admin path.
  - Grant if satisfied for all requested scopes.

Passing the target group to Keycloak (no per-group resources)
- Use Keycloak’s evaluation context attributes. Two viable approaches:
  1) API-as-PEP with decision mode (recommended):
     - For each incoming request on the API, extract `groupId` from route/data.
     - Call KC token endpoint with UMA grant (`grant_type=urn:ietf:params:oauth:grant-type:uma-ticket`) using the user’s Access Token as RPT input and include:
       - `audience=api-client-id`
       - `permission=group-management#<scope>` (one or more)
       - `response_mode=decision` (boolean decision)
       - `claims` (JSON) or `claim_token` representing `{ "groupId": ["<value>"] }`
     - Keycloak evaluates, your Java policy reads `groupId` from context attributes and returns allow/deny.
  2) Claim Information Point (CIP):
     - Configure a CIP on the API client that pulls attributes from an HTTP endpoint on your API (e.g., passes `groupId` from the current request), so the policy gets the attribute without the API building UMA claims. This is useful if you standardize attribute provisioning.

Why decision mode over RPT caching
- If RPTs encode permissions for a specific `groupId`, you’d need an RPT per (user, controller, scope, groupId) combination. That increases churn and client complexity.
- With decision mode, the API asks KC per request with the current `groupId`, keeping tokens simple and centralized decisions in KC.

API integration (server-side)
- Keep normal OIDC authentication (JwtBearer) to authenticate the user.
- Authorization: before executing the controller/endpoint logic, call KC PDP to check the required scope(s).
- Suggested helper in .NET:
  - Extract `groupId` from route/body.
  - Build a UMA decision request and POST to `/realms/{realm}/protocol/openid-connect/token` with:
    - `grant_type=urn:ietf:params:oauth:grant-type:uma-ticket`
    - `audience=api-client-id`
    - `permission=group-management#update` (etc.)
    - `response_mode=decision`
    - `claims={"groupId":["<value>"]}` (URL-encoded JSON)
  - If KC returns 200 and `result=true`, proceed; otherwise 403.
- Batch scopes: if an endpoint needs multiple scopes (rare), include multiple `permission` parameters.

Keycloak configuration checklist
- API client (confidential) with Authorization enabled.
- Resources: create one per controller area with the needed scopes.
- Policies:
  - Role-based policies for platform roles.
  - Register and configure the `GroupTargetPolicy` (Java provider) once.
- Permissions (scope-based): map each scope to the composite of policies:
  - `group-management#update/delete` → `GroupTargetPolicy` OR `AdminPolicy` OR `PlatformAdminPolicy`
  - `group-management#view/list` → `GroupTargetPolicy` OR `AdminPolicy` OR `PlatformAdminPolicy`
- KC groups structure for dynamic membership (no policy churn):
  - `/app/{groupId}/admins`
  - `/app/{groupId}/members`
  - Your app automation only creates/removes these KC groups and adds/removes users; policies remain unchanged.

Deployment of the Java policy
- Package as a Keycloak server provider JAR (SPI), include `META-INF/services` entries.
- Deploy to the Keycloak container (`/opt/keycloak/providers`) and trigger `kc.sh build` (or image rebuild).
- After restart, select the new policy type in Authorization → Policies and set its configuration (attribute name, base path, subpaths, scope rules).

Blazor app notes
- Blazor WASM and Server continue to authenticate with OIDC.
- The API performs the authorization decision with KC; the Blazor UI can still hide actions based on roles/groups, but the server is the source of truth.

Testing plan
- Create KC groups `/app/G1/admins` and `/app/G1/members`, assign users accordingly.
- Configure resource `group-management` with scopes and permissions using the new policy.
- Call API endpoints with different `groupId` and scopes; verify:
  - Platform Admin → always allowed
  - Admin → allowed
  - Group Admin of G1 → allowed for `update/delete` with `groupId=G1`
  - Group Member of G1 → allowed for `view/list` with `groupId=G1`, denied for `update/delete`
  - Non-member → denied
- Change membership; next request reflects new decision (no policy or resource changes).

Operational notes
- Cache PDP decisions briefly (e.g., 30–60s) keyed by (user, resource, scope, groupId) if needed; invalidate on group updates when possible.
- Ensure CORS and redirect URIs are correct for OIDC; PDP calls are server-to-server using the user’s token and the API’s client credentials if required.
- Audit membership and group lifecycle events in the app.
