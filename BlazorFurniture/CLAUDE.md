# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build BlazorFurniture.sln

# Run (with full orchestration: Keycloak, PostgreSQL, Maildev)
dotnet run --project Aspire/BlazorFurniture.AppHost

# Run standalone (no orchestration)
dotnet run --project src/Presentation/BlazorFurniture

# Tests
dotnet test tests/BlazorFurniture.UnitTests
dotnet test tests/BlazorFurniture.IntegrationTests   # requires Docker for Testcontainers

# Run a single test
dotnet test tests/BlazorFurniture.UnitTests --filter "FullyQualifiedName~TestClassName"
```

## Keycloak Realm Config

Realm configuration is versioned as YAML in `extensions/keycloak/`. To apply to a running local Keycloak:

```bash
./apply-realm.sh
# Override defaults:
KEYCLOAK_URL=http://localhost:8080 KEYCLOAK_USER=admin KEYCLOAK_PASSWORD=admin ./apply-realm.sh
```

When making realm changes: update the relevant YAML file, run `./apply-realm.sh` to verify, then commit.

## Architecture

Clean Architecture / DDD with CQRS. .NET 10 / Blazor Hybrid (Server + WebAssembly).

```
src/
  Core/
    BlazorFurniture.Domain          # Entities, domain behaviors (ISoftDeletable, ITrackable)
    BlazorFurniture.Application     # CQRS commands/queries, FastEndpoints, FluentValidation
    BlazorFurniture.Shared          # Cross-cutting concerns
    BlazorFurniture.Templates       # Razor email templates
  Infrastructure/
    BlazorFurniture.Infrastructure  # Auth/JWT, MailKit, Keycloak integration
    BlazorFurniture.Persistence     # EF Core DbContext, repositories (PostgreSQL)
  Presentation/
    BlazorFurniture                 # Blazor Server, FastEndpoints, auth flows, service extensions
    BlazorFurniture.Client          # Blazor WASM components, UserState, PermissionsService
    BlazorFurniture.Shared          # Shared DTOs, Refit API clients, UI models
Aspire/
  BlazorFurniture.AppHost           # .NET Aspire orchestration (Keycloak, PostgreSQL, Maildev)
  BlazorFurniture.ServiceDefaults   # Shared Aspire service config
tests/
  BlazorFurniture.UnitTests         # xUnit, AutoFixture, NSubstitute, FastEndpoints.Testing
  BlazorFurniture.IntegrationTests  # Testcontainers (Keycloak, PostgreSQL, Maildev)
extensions/                         # Custom Keycloak Java SPIs and Keycloakify theme JARs
docs/plans/                         # Authorization design docs (read these before modifying auth)
```

## Key Tech Stack

- **UI**: MudBlazor v8 (Material Design components)
- **API**: FastEndpoints v7 (endpoint-based, not MVC controllers)
- **Auth**: Keycloak 26 via OpenID Connect / OAuth2 PKCE; JWT Bearer for API
- **Database**: PostgreSQL 16 via EF Core
- **HTTP clients**: Refit v8 with Polly resilience
- **API docs**: Scalar + Swashbuckle (accessible in dev at `/scalar/v1`)
- **Email**: MailKit + Maildev sandbox in dev
- **NuGet versions**: Centrally managed in `Directory.Packages.props`

## Authorization Architecture

Role hierarchy: **Platform Admin → Admin → User** (realm roles); **Group Admin / Group Member** (per-group roles, independent).

Authorization uses Keycloak Authorization Services with:
- Resources per area (`group-management`, `user-management`, `settings`)
- Scopes: `view`, `list`, `create`, `update`, `delete`
- Custom Java GroupTargetPolicy SPI (in `extensions/`) for group-specific decisions
- `PermissionAuthorizationHandler` + `PermissionPolicyProvider` on the .NET side

See `docs/plans/authorizationPlanning.md` and `docs/plans/roleMapping.md` for full design details before modifying any auth-related code.

## Service Registration Pattern

Service registration is extension-method based. Look in `src/Presentation/BlazorFurniture/Extensions/` for examples — new services should follow this pattern (one extension class per concern).

## Localization

Localization is set up via `Microsoft.Extensions.Localization`. Supported languages: English, Romanian.
