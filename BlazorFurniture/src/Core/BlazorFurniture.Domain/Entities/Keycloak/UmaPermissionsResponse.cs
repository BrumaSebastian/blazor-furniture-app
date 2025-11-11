using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed class UmaPermissionsResponse
{
    [JsonPropertyName("rsid")]
    public required string ResourceId { get; set; }
    [JsonPropertyName("rsname")]
    public string? ResourceName { get; set; }
    public List<string> Scopes { get; set; } = [];
}
