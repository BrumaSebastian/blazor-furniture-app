# Role and Permission Mapping (Planning)

This document defines the roles, their scopes, and how they map to Keycloak, claims, and authorization policies used by the Blazor Furniture application.

Roles and scopes

| Level        | Role               | Description                                                                                             | Scope            |
| ------------ | ------------------ | ------------------------------------------------------------------------------------------------------- | ---------------- |
| Platform     | Platform Admin     | Superuser — full access across the system. Manage users, groups, settings, audit logs.                  | Global           |
|              | Admin              | Manage all groups and users.                                                                            | Global           |
|              | User               | Standard user — access to own data and public content.                                                  | Global           |
| Group        | Group Admin        | Administrates a specific group. Add/remove users, assign group roles, edit group data.                  | Per group        |
|              | Group Member       | Regular member with access to group-specific features.                                                  | Per group        |

Defaults and hierarchy
- New users receive the "User" role by default.
- Platform Admin > Admin > User (privilege superset). Platform roles do not imply any group role.
- Group roles are assigned per group and are independent across groups. A user can be Group Admin in one group and Group Member in another.

What each role can do
- Platform Admin
  - All Admin permissions
  - Manage platform settings, audit logs, security boundaries
- Admin
  - Manage all users: view, edit, delete, assign platform roles, assign groups
  - Manage all groups: create, edit, delete, assign group admins and members
- User
  - View and edit own profile
- Group Admin
  - Manage users within their group: add/remove users, assign group roles
  - Edit group data and settings for that group
- Group Member
  - Access group-specific features and content

Keycloak mapping (recommended)
- Realm roles (global):
  - platform-admin, admin, user
- Group modeling:
  - Use Keycloak groups for tenant/group identity. Each group contains users.
  - Represent group-roles either as:
    - Client roles on a dedicated client (e.g., app-backend: group-admin, group-member) with group attribute scoping, or
    - A custom claim that encodes group role membership (simpler for API checks; see Claims model).

