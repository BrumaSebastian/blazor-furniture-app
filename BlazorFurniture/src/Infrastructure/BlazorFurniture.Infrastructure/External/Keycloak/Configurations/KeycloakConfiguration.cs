namespace BlazorFurniture.Infrastructure.External.Keycloak.Configurations;

public class KeycloakConfiguration
{
    public const string Name = "Keycloak";
    public required string Url { get; set; }
    public required ServiceClient ServiceClient { get; set; }
}

public class ServiceClient
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string Realm { get; set; }
}
