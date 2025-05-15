namespace BlazorFurniture.Modules.Keycloak.Extensions;

internal class KeycloakConfiguration
{
    public required string BaseUrl { get; set; }
    public required Endpoints Endpoints { get; set; }
    public required ServiceClient ServiceClient { get; set; }
    public required Authentication Authentication { get; set; }
}

internal class Authentication
{
    public required string Audience { get; set; }
    public required string ValidIssuer { get; set; }
    public required string MetadataAddress { get; set; }
}

internal class ServiceClient
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string Realm { get; set; }
}

internal class Endpoints
{
    public required string Authorization { get; set; }
    public required string Token { get; set; }
}
