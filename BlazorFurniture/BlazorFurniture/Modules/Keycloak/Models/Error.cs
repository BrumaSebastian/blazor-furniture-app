using System.Text.Json.Serialization;

namespace BlazorFurniture.Modules.Keycloak.Models;

public class Error
{
    [JsonPropertyName("error")]
    public string Type { get; set; }

    [JsonPropertyName("error_description")]
    public string Description { get; set; }
}
