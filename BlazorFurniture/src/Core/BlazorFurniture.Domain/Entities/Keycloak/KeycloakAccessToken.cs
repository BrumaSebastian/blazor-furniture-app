using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public class KeycloakAccessToken
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    [JsonPropertyName("session_state")]
    public string? Session { get; set; }
}
