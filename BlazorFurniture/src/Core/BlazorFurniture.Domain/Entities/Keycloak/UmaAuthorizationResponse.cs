using System.Text.Json.Serialization;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed class UmaAuthorizationResponse
{
    [JsonPropertyName("result")]
    public bool IsAuthorized { get; set; }
}
