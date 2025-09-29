using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public class KeycloakError
{
    [JsonPropertyName("error")]
    public string Title { get; set; }
    [JsonPropertyName("error_description")]
    public string Description { get; set; }
    [JsonPropertyName("errors")]
    public List<ErrorRepresentation>? Errors { get; set; }
}
