using BlazorFurniture.Domain.Enums;
using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed record GroupUserRepresentation
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public bool Enabled { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GroupRoles Role { get; set; }
    public Dictionary<string, List<string>> Attributes { get; set; } = [];
}
