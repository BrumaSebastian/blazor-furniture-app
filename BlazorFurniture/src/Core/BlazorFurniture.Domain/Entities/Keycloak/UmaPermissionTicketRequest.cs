using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed class UmaPermissionTicketRequest
{
    [JsonPropertyName("resource_id")]
    public string ResourceId { get; set; }
    [JsonPropertyName("resource_scopes")]
    public List<string> ResourceScopes { get; set; } = [];
    [JsonPropertyName("claims")]
    public IDictionary<string, List<string>> Claims { get; set; } = new Dictionary<string, List<string>>();
}
