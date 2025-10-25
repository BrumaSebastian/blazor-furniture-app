using BlazorFurniture.Domain.Enums;
using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed record UserPermissionsRepresentation
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PlatformRoles Role { get; init; }
    public IReadOnlyList<string> Permissions { get; set; } = [];
    public IReadOnlyList<UserGroupPermissionsRepresentation> Groups { get; set; } = [];
}
