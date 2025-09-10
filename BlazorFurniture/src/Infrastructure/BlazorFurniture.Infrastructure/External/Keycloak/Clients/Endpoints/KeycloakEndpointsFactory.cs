using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using Microsoft.Extensions.Options;

namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients.Endpoints;

internal class KeycloakEndpointsFactory(IOptions<KeycloakConfiguration> keycloakConfig)
{
    private readonly KeycloakConfiguration keycloakOptions = keycloakConfig.Value;

    public Endpoint CreateEndpoints()
    {
        return new Endpoint(keycloakOptions.BaseUrl, keycloakOptions.ServiceClient.Realm);
    }
}
