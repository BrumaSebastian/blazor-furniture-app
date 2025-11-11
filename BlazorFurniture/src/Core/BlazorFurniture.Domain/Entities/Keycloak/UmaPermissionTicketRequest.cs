using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed class UmaPermissionTicketRequest
{
    [JsonPropertyName("resource_id")]
    public required string ResourceId { get; set; }
    [JsonPropertyName("resource_scopes")]
    public string[] ResourceScopes { get; set; } = [];
    [JsonPropertyName("claims")]
    public Dictionary<string, List<string>> Claims { get; set; } = [];
}
