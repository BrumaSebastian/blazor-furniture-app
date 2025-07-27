using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed class AuthenticationRepresentation
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; init; }
    [JsonPropertyName("session_state")]
    public string? Session { get; init; }
}
