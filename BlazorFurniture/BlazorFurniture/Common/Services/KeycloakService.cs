using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace BlazorFurniture.Common.Services;

public class KeycloakService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public KeycloakService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    private async Task<string> GetAdminAccessTokenAsync()
    {
        var baseUrl = _config["Keycloak:BaseUrl"];
        var realm = _config["Keycloak:Realm"];
        var clientId = _config["Keycloak:ClientId"];
        var clientSecret = _config["Keycloak:ClientSecret"];

        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/realms/{realm}/protocol/openid-connect/token");
        var body = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
        });

        request.Content = body;

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);
        return doc.RootElement.GetProperty("access_token").GetString();
    }

    public async Task CreateUserAsync(string username, string email, string firstName, string lastName)
    {
        var token = await GetAdminAccessTokenAsync();
        var baseUrl = _config["Keycloak:BaseUrl"];
        var realm = _config["Keycloak:Realm"];

        var userPayload = new
        {
            username,
            email,
            firstName,
            lastName,
            enabled = true
        };

        var json = JsonSerializer.Serialize(userPayload);
        var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/admin/realms/{realm}/users")
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
