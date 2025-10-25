using BlazorFurniture.Domain.Enums;
using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed record UserGroupPermissionsRepresentation
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required GroupRoles Role { get; set; }
    public IReadOnlyList<string> Permissions { get; set; } = [];
}
