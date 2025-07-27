using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Keycloak.Clients.Endpoints;
using BlazorFurniture.Infrastructure.External.Keycloak.Clients.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients.Implementations;

internal abstract class KeycloakBaseHttpClient(HttpClient httpClient,
    IOptions<KeycloakConfiguration> keycloakConfiguration,
    Endpoint endpoint) : IClient
{
    private readonly KeycloakConfiguration configuration = keycloakConfiguration.Value;

    protected virtual async Task<AuthenticationRepresentation> GetServiceTokenAsync()
    {
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", configuration.ServiceClient.ClientId },
                { "client_secret", configuration.ServiceClient.ClientSecret }
            });

        var response = await httpClient.PostAsync(endpoint.Token, requestBody);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<AuthenticationRepresentation>();

        return content!;
    }

    protected virtual async Task<AuthenticationRepresentation> VerifyUserCredentialsAsync(string email, string password)
    {
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { "grant_type", "password" },
                { "client_id", configuration.ServiceClient.ClientId },
                { "client_secret", configuration.ServiceClient.ClientSecret },
                { "username",  email },
                { "password",  password },

            });

        var response = await httpClient.PostAsync(endpoint.Token, requestBody);
        response.EnsureSuccessStatusCode();

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AuthenticationRepresentation>();
        }

        var error = await response.Content.ReadFromJsonAsync<ErrorRepresentation>();
        throw new Exception(error?.Description);
    }

    public async Task<T?> GetAsync<T>(string endpoint, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> SendAsync<T>(HttpMethod httpMethod, string endpoint, string token, object body)
    {
        var request = new HttpRequestMessage(httpMethod, endpoint);
        request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<T>();
        }

        var error = await response.Content.ReadFromJsonAsync<ErrorRepresentation>();
        throw new Exception(error?.Description);
    }
}
