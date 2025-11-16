using BlazorFurniture.IntegrationTests.Controllers.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;

namespace BlazorFurniture.IntegrationTests.Controllers.Authorization;

public class GroupsControllerTests : IClassFixture<KeycloakFixture>, IAsyncLifetime
{
    private readonly KeycloakFixture keycloakFixture;
    private readonly WebApplicationFactory<Program> factory;
    private HttpClient client = null!;

    public GroupsControllerTests( KeycloakFixture keycloakFixture )
    {
        this.keycloakFixture = keycloakFixture;
        factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("IntegrationTests");
            });
    }

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        client = factory.CreateClient();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        client?.Dispose();
    }

    [Fact]
    public async Task GetGroups_WithPlatformAdminRole_ReturnsOk()
    {
        // Arrange
        var accessToken = await GetUserAccessToken(
            keycloakFixture.PlatformAdmin.Username,
            keycloakFixture.PlatformAdmin.Password);

        SetAuthorizationHeader(accessToken);

        // Act
        var response = await client.GetAsync("/api/groups");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetGroups_WithNormalUser_ReturnsForbidden()
    {
        // Arrange
        var user = new KeycloakUser
        {
            Username = "testuser",
            Email = "testuser@a.com",
            Password = "Test123!"
        };

        await keycloakFixture.CreateUser(user);
        var accessToken = await GetUserAccessToken(user.Username, user.Password);

        SetAuthorizationHeader(accessToken);

        // Act
        var response = await client.GetAsync("/api/groups");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    
    private async Task<string> GetUserAccessToken( string username, string password )
    {
        using var keycloakClient = new HttpClient { BaseAddress = new Uri(keycloakFixture.BaseUrl) };
        return await keycloakFixture.GetUserToken(keycloakClient, username, password);
    }

    private void SetAuthorizationHeader( string accessToken )
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
    }
}
