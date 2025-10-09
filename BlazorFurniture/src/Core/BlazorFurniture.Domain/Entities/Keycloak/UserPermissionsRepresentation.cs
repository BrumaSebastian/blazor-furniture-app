using BlazorFurniture.Domain.Enums;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed record UserPermissionsRepresentation
{
    public PlatformRoles Role { get; init; }
    public IReadOnlyList<string> Permissions { get; set; } = [];
    public IReadOnlyList<UserGroupPermissionsRepresentation> Groups { get; set; } = [];
}
