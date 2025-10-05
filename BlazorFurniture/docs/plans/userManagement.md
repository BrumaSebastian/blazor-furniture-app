# User Management Planning — Introduction

This document defines the scope, goals, and constraints for implementing user management in the Blazor Furniture application. The solution targets .NET 10 and uses Blazor (Server + WebAssembly) with FastEndpoints on the server, MudBlazor for UI, Keycloak as the OpenID Connect provider, and OpenAPI/Scalar for API discoverability.

Objectives
- Provide secure authentication with Keycloak (OIDC) and reliable sign-in/sign-out.
- Enforce role/claim-based authorization and least-privilege access to features and APIs.
- Enable self-service profile management and admin capabilities for user and role management.
- Ensure a consistent UX across Server and WASM render modes.
- Maintain auditability, compliance, and privacy standards.

In Scope
- Auth flows: login, logout (RP-initiated), token handling, and session management.
- Profile: view/edit basic details; optional avatar and preferences.
- Authorization: roles, claims, and policy-based access for pages/endpoints.
- Admin: list users, view details, assign/revoke roles/claims.
- API surface with OpenAPI definitions and appropriate security schemes.

Out of Scope
- Credentials and password recovery flows (delegated to Keycloak).
- Cross-realm SSO, custom MFA providers, or email delivery infrastructure.

Constraints & Assumptions
- .NET 10, C# 14, FastEndpoints, MudBlazor, Keycloak (OIDC).
- Proper Keycloak client configuration (redirect and post-logout URIs).
- CORS and SameSite settings compatible with OIDC redirects and Blazor hosting model.

Success Criteria
- Protected routes/pages and endpoints enforce policies correctly.
- Logout clears app cookies and performs Keycloak RP-initiated sign-out.
- OpenAPI docs reflect secured endpoints; basic tests for auth and profile flows pass.
