using System.Text.Json.Serialization;

namespace BlazorFurniture.Modules.Keycloak.Models;

public class KeycloakTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}

