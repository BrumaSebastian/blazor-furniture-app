using BlazorFurniture.Domain.Enums;
using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed record UserGroupRepresentation
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required GroupRoles Role { get; set; }
    public Dictionary<string, List<string>> Attributes { get; set; } = [];
}
