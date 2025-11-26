using BlazorFurniture.Domain.Enums;
using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed record ExtendedUserRepresentation : UserRepresentation
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PlatformRoles Role { get; set; }
}
