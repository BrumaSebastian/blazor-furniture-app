using BlazorFurniture.IntegrationTests.Controllers.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
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
                builder.UseEnvironment("IntegrationTests");
            });
    }

    [Fact]
    public async Task GetGroupUsers_WithAuthentication_ReturnsOk()
    {
        // Arrange
        var user = await keycloakFixture.CreateUser("testuser", "testuser@a.com", "password");

        using var keycloakClient = new HttpClient { BaseAddress = new Uri(keycloakFixture.BaseUrl) };
        var token = await keycloakFixture.GetAndSetUserToken(keycloakClient, "testuser", "password");
        var client = factory.CreateClient();
        var groupId = Guid.NewGuid(); // Replace with actual group from your realm export

        // Act
        var response = await client.GetAsync($"/api/groups/{groupId}/users");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetGroups_WithPlatformAdminRole_ReturnsOk()
    {
        // Arrange
        using var keycloakClient = new HttpClient { BaseAddress = new Uri(keycloakFixture.BaseUrl) };
        var accessToken = await keycloakFixture.GetAndSetUserToken(keycloakClient, keycloakFixture.PlatformAdmin.Username, keycloakFixture.PlatformAdmin.Password);
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

        // Act
        var response = await client.GetAsync($"/api/groups", CancellationToken.None);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

}
