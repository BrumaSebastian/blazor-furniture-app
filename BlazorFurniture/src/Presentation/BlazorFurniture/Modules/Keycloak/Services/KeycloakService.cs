using BlazorFurniture.Modules.Keycloak.Clients;
using BlazorFurniture.Modules.Keycloak.Extensions;
using BlazorFurniture.Modules.Keycloak.Models;
using BlazorFurniture.Modules.Keycloak.Models.DTOs;
using BlazorFurniture.Modules.Keycloak.Models.Representations;
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

    public async Task<User> GetUserByPhoneNumberAsync(string phoneNumber)
    {
        var token = await GetCachedServiceTokenAsync();
        var endpoint = $"{_keycloakConfiguration.BaseUrl}/admin/realms/{_keycloakConfiguration.ServiceClient.Realm}/users?q=phone:${phoneNumber}&exact=true";
        var users = await _keycloakHttpClient.GetAsync<List<User>>(endpoint, token);

        return users.First();
    }

    public async Task<List<User>> GetUsersAsync()
    {
        var token = await GetCachedServiceTokenAsync();
        var endpoint = $"{_keycloakConfiguration.BaseUrl}/admin/realms/{_keycloakConfiguration.ServiceClient.Realm}/users";
        var users = await _keycloakHttpClient.GetAsync<List<User>>(endpoint, token);

        return users;
    }

    public async Task<bool> UpdatePassword(string userId, UpdatePassword updatePasswordDto)
    {
        // verify if current password is correct
        var result = await _keycloakHttpClient.VerifyUserCredentialsAsync(updatePasswordDto.Email, updatePasswordDto.CurrentPassword);

        if (result is null) return false;

        var token = await GetCachedServiceTokenAsync();
        var endpoint = $"{_keycloakConfiguration.BaseUrl}/admin/realms/{_keycloakConfiguration.ServiceClient.Realm}/users/{userId}/reset-password";
        CredentialRepresentation body = new()
        {
            Temporary = false,
            Type = "password",
            Value = updatePasswordDto.NewPassword
        };

        var res = await _keycloakHttpClient.SendAsync<object>(HttpMethod.Put, endpoint, token, body);

        return true;
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
