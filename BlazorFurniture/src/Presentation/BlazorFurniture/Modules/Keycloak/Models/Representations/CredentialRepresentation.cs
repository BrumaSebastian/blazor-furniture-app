using System.Text.Json.Serialization;

namespace BlazorFurniture.Modules.Keycloak.Models.Representations;

public class CredentialRepresentation
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    [JsonPropertyName("temporary")]
    public bool Temporary { get; set; }
    [JsonPropertyName("value")]
    public string Value { get; set; }
}
