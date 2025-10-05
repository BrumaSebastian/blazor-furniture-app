using BlazorFurniture.Domain.Enums;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed record UserGroupPermissionsRepresentation
{
    public required GroupRepresentation Group { get; set; }
    public required GroupRoles Role { get; set; }
    public IReadOnlyList<string> Permissions { get; set; } = [];
}
