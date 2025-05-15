using BlazorFurniture.Modules.Keycloak.Clients;
using BlazorFurniture.Modules.Keycloak.Extensions;
using BlazorFurniture.Modules.Keycloak.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace BlazorFurniture.Modules.Keycloak.Services;

internal class KeycloakService(KeycloakHttpClient keycloakHttpClient, IMemoryCache cache, 
    IOptions<KeycloakConfiguration> keycloakConfiguration) : IKeycloakService
{
    private const string SERVICE_TOKEN_CACHE_KEY = "keycloak_service_token";

    private readonly KeycloakHttpClient _keycloakHttpClient = keycloakHttpClient;
    private readonly IMemoryCache _cache = cache;
    private readonly KeycloakConfiguration _keycloakConfiguration = keycloakConfiguration.Value;

    public Task<User> GetUserAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<User>> GetUsersAsync()
    {
        var token = await GetCachedServiceTokenAsync();
        var endpoint = $"{_keycloakConfiguration.BaseUrl}/admin/realms/{_keycloakConfiguration.ServiceClient.Realm}/users";
        var users = await _keycloakHttpClient.GetAsync<List<User>>(endpoint, token);

        return users;
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
