using BlazorFurniture.Domain.Enums;
using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public class GroupRoleRepresentation
{
    public Guid Id { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public GroupRoles Role { get; set; }
}
