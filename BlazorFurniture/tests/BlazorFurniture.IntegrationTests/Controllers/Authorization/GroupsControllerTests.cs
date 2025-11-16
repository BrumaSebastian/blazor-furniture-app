using BlazorFurniture.IntegrationTests.Controllers.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;

namespace BlazorFurniture.IntegrationTests.Controllers.Authorization;

[Trait("Category", "Integration")]
public class GroupsControllerTests : IClassFixture<KeycloakFixture>, IAsyncLifetime
{
    private readonly KeycloakFixture keycloakFixture;
    private readonly WebApplicationFactory<Program> factory;
    private HttpClient httpClient = null!;
    private readonly string AUTHORIZATION_RESOURCE_CLIENT = "server-client";

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
        httpClient = factory.CreateClient();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        httpClient?.Dispose();
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
        var response = await httpClient.GetAsync("/api/groups");

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
            Email = "testuser@test.com",
            Password = "Test123!"
        };

        await keycloakFixture.CreateUser(user);
        var accessToken = await GetUserAccessToken(user.Username, user.Password);

        SetAuthorizationHeader(accessToken);

        // Act
        var response = await httpClient.GetAsync("/api/groups", CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetGroups_WithNormalUserWithGroupsCreateRole_ReturnsOk()
    {
        // Arrange
        var roleName = "groups-list";
        var user = new KeycloakUser
        {
            Username = "testuser-groups-list",
            Email = "testuser-groups-list@test.com",
            Password = "Test123!"
        };

        var userId = await keycloakFixture.CreateUser(user);
        var authResourceClient = await keycloakFixture.GetClient(AUTHORIZATION_RESOURCE_CLIENT);
        var role = await keycloakFixture.GetClientRole(roleName, authResourceClient.Id);
        await keycloakFixture.AssignClientRole(userId, authResourceClient.Id, role);
        var accessToken = await GetUserAccessToken(user.Username, user.Password);

        SetAuthorizationHeader(accessToken);

        // Act
        var response = await httpClient.GetAsync("/api/groups", CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<string> GetUserAccessToken( string username, string password )
    {
        using var keycloakClient = new HttpClient { BaseAddress = new Uri(keycloakFixture.BaseUrl) };
        return await keycloakFixture.GetUserAccessToken(keycloakClient, username, password);
    }

    private void SetAuthorizationHeader( string accessToken )
    {
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
    }
}
