namespace BlazorFurniture.Infrastructure.External.Keycloak.Configurations;

internal class EndpointsFactory( KeycloakConfiguration configuration )
{
    public Endpoints Create()
    {
        return new Endpoints(configuration.ServiceClient.Realm);
    }
}
