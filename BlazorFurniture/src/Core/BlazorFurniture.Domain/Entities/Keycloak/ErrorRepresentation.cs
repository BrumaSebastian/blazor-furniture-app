using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed class ErrorRepresentation
{
    public string? Error { get; set; }
    [JsonPropertyName("error_description")]
    public string? Description { get; set; }
    public List<ErrorRepresentation>? Errors { get; set; } = [];
}
