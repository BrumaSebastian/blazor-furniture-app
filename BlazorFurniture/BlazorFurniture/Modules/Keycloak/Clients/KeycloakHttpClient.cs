using BlazorFurniture.Modules.Keycloak.Extensions;
using BlazorFurniture.Modules.Keycloak.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;

namespace BlazorFurniture.Modules.Keycloak.Clients;

internal class KeycloakHttpClient(HttpClient httpClient, 
    IOptions<KeycloakConfiguration> keycloakConfiguration, 
    KeycloakEndpoints keycloakEndpoints)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly KeycloakEndpoints _keycloakEndpoints = keycloakEndpoints;
    private readonly KeycloakConfiguration _keycloakConfiguration = keycloakConfiguration.Value;

    public async Task<KeycloakTokenResponse> GetServiceTokenAsync()
    {
        var requestBody = new FormUrlEncodedContent(
            new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _keycloakConfiguration.ServiceClient.ClientId },
                { "client_secret", _keycloakConfiguration.ServiceClient.ClientSecret }
            });

        var response = await _httpClient.PostAsync(_keycloakEndpoints.Token, requestBody);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();

        return content!;
    }

    public async Task<T?> GetAsync<T>(string endpoint, string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public async Task<T?> PostAsync<T>(string endpoint, string token, object body)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
        request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(body), System.Text.Encoding.UTF8, "application/json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>();
    }
}
