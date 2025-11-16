using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Infrastructure.External;
using BlazorFurniture.IntegrationTests.Controllers.Setup;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

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
        await factory.DisposeAsync();
    }

    [Fact]
    public async Task GetGroups_WithPlatformAdminRole_ReturnsOk()
    {
        // Arrange
        var accessToken = await GetUserAccessToken(
            keycloakFixture.PlatformAdmin.Username,
            keycloakFixture.PlatformAdmin.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Get)
            .WithPath("api/groups")
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGroups_WithNormalUser_ReturnsForbidden()
    {
        // Arrange
        var user = new KeycloakUser();
        await keycloakFixture.CreateUser(user);
        var accessToken = await GetUserAccessToken(user.Username, user.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Get)
            .WithPath("api/groups")
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetGroups_WithNormalUserWithGroupsCreateRole_ReturnsOk()
    {
        // Arrange
        var roleName = "groups-list";
        var user = new KeycloakUser();
        await CreateUserAndAssignClientRole(roleName, user);
        var accessToken = await GetUserAccessToken(user.Username, user.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Get)
            .WithPath("api/groups")
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGroups_WithoutAccessToken_ReturnsUnauthorized()
    {
        // Arrange
        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Get)
            .WithPath("api/groups")
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateGroup_WithPlatformAdminUser_ReturnsSuccessfulResponse()
    {
        // Arrange
        var accessToken = await GetUserAccessToken(
            keycloakFixture.PlatformAdmin.Username,
            keycloakFixture.PlatformAdmin.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Post)
            .WithPath("/api/groups")
            .WithContent(new CreateGroupRequest("Test Create Group", "This is a test group created during integration tests."))
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateGroup_WithNormalUserWithGroupsCreateRole_ReturnsSuccesfullResponse()
    {
        // Arrange
        var roleName = "groups-create";
        var user = new KeycloakUser();
        await CreateUserAndAssignClientRole(roleName, user);
        var accessToken = await GetUserAccessToken(user.Username, user.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Post)
            .WithPath("/api/groups")
            .WithContent(new CreateGroupRequest("Test Create Group 1", "This is a test group created during integration tests."))
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateGroup_WithNormalUser_ReturnsForbidden()
    {
        // Arrange
        var user = new KeycloakUser();
        await keycloakFixture.CreateUser(user);
        var accessToken = await GetUserAccessToken(user.Username, user.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Get)
            .WithPath("api/groups")
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetGroup_WithPlatformAdminUser_ReturnsSuccessfulResponse()
    {
        // Arrange
        var groupName = "Test Get Group";
        var groupId = await CreateGroup(groupName, "This is a test group created during integration tests.");

        var accessToken = await GetUserAccessToken(keycloakFixture.PlatformAdmin.Username, keycloakFixture.PlatformAdmin.Password);
        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Get)
            .WithPath($"/api/groups/{groupId}")
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetGroup_WithNormalUserWithGroupsReadRole_ReturnsForbiddenResponse()
    {
        // Arrange
        var groupName = "Test Get Group 2";
        var groupId = await CreateGroup(groupName, "This is a test group created during integration tests.");

        var roleName = "groups-read";
        var user = new KeycloakUser();
        await CreateUserAndAssignClientRole(roleName, user);
        var accessToken = await GetUserAccessToken(user.Username, user.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Get)
            .WithPath($"/api/groups/{groupId}")
            .WithContent(new CreateGroupRequest("Test Create Group 1", "This is a test group created during integration tests."))
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetGroup_WithNormalUser_ReturnsForbidden()
    {
        // Arrange
        var groupName = Guid.NewGuid().ToString();
        var groupId = await CreateGroup(groupName, "This is a test");

        var user = new KeycloakUser();
        await keycloakFixture.CreateUser(user);
        var accessToken = await GetUserAccessToken(user.Username, user.Password);
        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Get)
            .WithPath($"/api/groups/{groupId}")
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetGroup_WithNormalUser_WithinGroup_ReturnsSuccessResponse()
    {
        // Arrange
        var groupName = Guid.NewGuid().ToString();
        var groupId = await CreateGroup(groupName);

        var user = new KeycloakUser();
        var userId = await keycloakFixture.CreateUser(user);
        await AddUserToGroup(groupId, userId);
        var accessToken = await GetUserAccessToken(user.Username, user.Password);
        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Get)
            .WithPath($"/api/groups/{groupId}")
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task GetGroup_WithNormalUser_WithinAGroup_TryingToGetAnotherGroupInfo_ReturnsForbidden()
    {
        // Arrange
        var userGroupName = Guid.NewGuid().ToString();
        var userGroupId = await CreateGroup(userGroupName);

        var secondGroupName = Guid.NewGuid().ToString();
        var secondGroupId = await CreateGroup(secondGroupName);

        var user = new KeycloakUser();
        var userId = await keycloakFixture.CreateUser(user);
        await AddUserToGroup(userGroupId, userId);
        var accessToken = await GetUserAccessToken(user.Username, user.Password);
        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Get)
            .WithPath($"/api/groups/{secondGroupId}")
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task CreateUserAndAssignClientRole( string roleName, KeycloakUser user )
    {
        var userId = await keycloakFixture.CreateUser(user);
        var authResourceClient = await keycloakFixture.GetClient(AUTHORIZATION_RESOURCE_CLIENT);
        var role = await keycloakFixture.GetClientRole(roleName, authResourceClient.Id);
        await keycloakFixture.AssignClientRole(userId, authResourceClient.Id, role);
    }

    private async Task<string> GetUserAccessToken( string username, string password )
    {
        using var keycloakClient = new HttpClient { BaseAddress = new Uri(keycloakFixture.BaseUrl) };
        return await keycloakFixture.GetUserAccessToken(keycloakClient, username, password);
    }

    private async Task<Guid> CreateGroup( string name, string? description = "" )
    {
        var accessToken = await GetUserAccessToken(keycloakFixture.PlatformAdmin.Username, keycloakFixture.PlatformAdmin.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Post)
            .WithPath("/api/groups")
            .WithContent(new CreateGroupRequest(name, description ?? string.Empty))
            .WithAuthorization(accessToken)
            .Build();

        // Act
        HttpResponseMessage? response = await httpClient.SendAsync(request, CancellationToken.None);
        response.EnsureSuccessStatusCode();

        var locationHeader = response.Headers.Location?.ToString();
        var groupId = locationHeader?.Split('/').Last();

        return Guid.Parse(groupId!);
    }

    private async Task AddUserToGroup( Guid groupId, Guid userId )
    {
        var accessToken = await GetUserAccessToken(keycloakFixture.PlatformAdmin.Username, keycloakFixture.PlatformAdmin.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Post)
            .WithPath($"/api/groups/{groupId}/users/{userId}")
            .WithAuthorization(accessToken)
            .Build();

        var response = await httpClient.SendAsync(request, CancellationToken.None);
        response.EnsureSuccessStatusCode();
    }
}
