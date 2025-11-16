using BlazorFurniture.IntegrationTests.Controllers.Setup;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorFurniture.IntegrationTests.Controllers.Authorization;

public class GroupsControllerTests : IClassFixture<KeycloakFixture>
{
    private readonly KeycloakFixture keycloakFixture;
    private readonly WebApplicationFactory<Program> factory;

    public GroupsControllerTests( KeycloakFixture keycloakFixture )
    {
        this.keycloakFixture = keycloakFixture;
        factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration(( context, config ) =>
                {
                    // Override Keycloak settings to point to test container
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["OpenIdConnect:Authority"] = keycloakFixture.Authority,
                        ["Keycloak:Url"] = keycloakFixture.Authority.Replace($"/realms/{keycloakFixture.RealmName}", ""),
                        ["Keycloak:Realm"] = keycloakFixture.RealmName,
                        ["Keycloak:ServiceClient:ClientId"] = "test-api-client",
                        ["Keycloak:ServiceClient:ClientSecret"] = "test-secret"
                    }!);
                });
            });
    }

    [Fact]
    public async Task GetGroupUsers_WithAuthentication_ReturnsOk()
    {
        // Arrange
        var client = factory.CreateClient();
        var token = await GetUserToken("testuser", "password");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var groupId = Guid.NewGuid(); // Replace with actual group from your realm export

        // Act
        var response = await client.GetAsync($"/api/groups/{groupId}/users");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<string> GetUserToken( string username, string password )
    {
        using var client = new HttpClient();
        var tokenResponse = await client.PostAsync(
            $"{keycloakFixture.Authority}/protocol/openid-connect/token",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["client_id"] = "integration-test-client",
                ["username"] = username,
                ["password"] = password
            }));

        tokenResponse.EnsureSuccessStatusCode();
        var token = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();
        return token.GetProperty("access_token").GetString()!;
    }
}
