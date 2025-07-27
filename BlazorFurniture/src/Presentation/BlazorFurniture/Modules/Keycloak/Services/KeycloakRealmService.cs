using BlazorFurniture.Modules.Keycloak.Clients;
using BlazorFurniture.Modules.Keycloak.Extensions;
using BlazorFurniture.Modules.Keycloak.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BlazorFurniture.Modules.Keycloak.Services;

internal class KeycloakRealmService(KeycloakHttpClient keycloakHttpClient, IMemoryCache cache,
    IOptions<KeycloakConfiguration> keycloakConfiguration) : IKeycloakRealmService
{
    private const string SERVICE_TOKEN_CACHE_KEY = "keycloak_service_token";

    private readonly KeycloakHttpClient _keycloakHttpClient = keycloakHttpClient;
    private readonly IMemoryCache _cache = cache;
    private readonly KeycloakConfiguration _keycloakConfiguration = keycloakConfiguration.Value;

    public async Task<RealmRepresentation> CreateRealmAsync(RealmRepresentation realm)
    {
        var token = await GetCachedServiceTokenAsync();
        var endpoint = $"{_keycloakConfiguration.BaseUrl}/admin/realms";
        var realmRes = await _keycloakHttpClient.SendAsync<RealmRepresentation>(HttpMethod.Post, endpoint, token, realm);

        return realmRes;
    }

    private async Task<string> GetCachedServiceTokenAsync()
    {
        return await _cache.GetOrCreateAsync(SERVICE_TOKEN_CACHE_KEY, async entry =>
        {
            var serviceToken = await _keycloakHttpClient.GetServiceTokenAsync();
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(serviceToken.ExpiresIn - 10);
            return serviceToken.AccessToken;
        });
    }
}
