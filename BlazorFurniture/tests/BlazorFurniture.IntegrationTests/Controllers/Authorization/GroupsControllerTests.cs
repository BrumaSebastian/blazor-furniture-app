using BlazorFurniture.IntegrationTests.Controllers.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http.Headers;

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
        var user = await keycloakFixture.CreateUserAsync("testuser", "testuser@a.com", "password");

        using var keycloakClient = new HttpClient { BaseAddress = new Uri(keycloakFixture.BaseUrl) };
        var token = await keycloakFixture.GetAndSetUserToken(keycloakClient, "testuser", "password");
        var client = factory.CreateClient();
        var groupId = Guid.NewGuid(); // Replace with actual group from your realm export

        // Act
        var response = await client.GetAsync($"/api/groups/{groupId}/users");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

}
