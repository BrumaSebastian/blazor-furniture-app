namespace BlazorFurniture.Infrastructure.External.Keycloak.Configurations;

public class KeycloakConfiguration
{
    public required string BaseUrl { get; set; }
    public required string LocalUrl { get; set; }
    //public required Endpoints Endpoints { get; set; }
    public required ServiceClient ServiceClient { get; set; }
    public required Authentication Authentication { get; set; }
}

public class Authentication
{
    public required string Audience { get; set; }
    public required string ValidIssuer { get; set; }
    public required string MetadataAddress { get; set; }
}

public class ServiceClient
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string Realm { get; set; }
}

//internal class Endpoints
//{
//    public required string Authorization { get; set; }
//    public required string Token { get; set; }
//}
