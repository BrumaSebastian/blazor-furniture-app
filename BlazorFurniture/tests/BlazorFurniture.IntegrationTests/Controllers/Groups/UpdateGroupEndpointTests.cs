using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Infrastructure.External;
using BlazorFurniture.IntegrationTests.Controllers.Setup;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace BlazorFurniture.IntegrationTests.Controllers.Groups;

[Collection("Keycloak Collection")]
[Trait("Category", "Integration")]
public class UpdateGroupEndpointTests : IAsyncLifetime
{
    private readonly KeycloakFixture keycloakFixture;
    private readonly WebApplicationFactory<Program> factory;
    private HttpClient httpClient = null!;

    public UpdateGroupEndpointTests(KeycloakFixture keycloakFixture)
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
    public async Task UpdateGroup_ReturnsNoContent_WhenGroupIsUpdatedSuccessfully()
    {
        // Arrange
        var groupId = await CreateGroup("Test Group for Update", "Description");
        var accessToken = await GetUserAccessToken(
            keycloakFixture.PlatformAdmin.Username,
            keycloakFixture.PlatformAdmin.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Put)
            .WithPath($"api/groups/{groupId}")
            .WithContent(new UpdateGroupRequest
            {
                GroupId = groupId,
                Name = "Updated Group Name"
            })
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UpdateGroup_ReturnsNotFound_WhenGroupDoesNotExist()
    {
        // Arrange
        var nonExistentGroupId = Guid.NewGuid();
        var accessToken = await GetUserAccessToken(
            keycloakFixture.PlatformAdmin.Username,
            keycloakFixture.PlatformAdmin.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Put)
            .WithPath($"api/groups/{nonExistentGroupId}")
            .WithContent(new UpdateGroupRequest
            {
                GroupId = nonExistentGroupId,
                Name = "Updated Group Name"
            })
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateGroup_ReturnsConflict_WhenNameAlreadyExists()
    {
        // Arrange
        var duplicateName = "Existing Group Name";
        var groupId1 = await CreateGroup(duplicateName, "First group");
        var groupId2 = await CreateGroup("Another Group", "Second group");
        
        var accessToken = await GetUserAccessToken(
            keycloakFixture.PlatformAdmin.Username,
            keycloakFixture.PlatformAdmin.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Put)
            .WithPath($"api/groups/{groupId2}")
            .WithContent(new UpdateGroupRequest
            {
                GroupId = groupId2,
                Name = duplicateName
            })
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task UpdateGroup_ReturnsBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var groupId = await CreateGroup("Test Group", "Description");
        var accessToken = await GetUserAccessToken(
            keycloakFixture.PlatformAdmin.Username,
            keycloakFixture.PlatformAdmin.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Put)
            .WithPath($"api/groups/{groupId}")
            .WithContent(new UpdateGroupRequest
            {
                GroupId = groupId,
                Name = "" // Invalid
            })
            .WithAuthorization(accessToken)
            .Build();

        // Act
        var response = await httpClient.SendAsync(request, CancellationToken.None);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<string> GetUserAccessToken(string username, string password)
    {
        using var keycloakClient = new HttpClient { BaseAddress = new Uri(keycloakFixture.BaseUrl) };
        return await keycloakFixture.GetUserAccessToken(keycloakClient, username, password);
    }

    private async Task<Guid> CreateGroup(string name, string? description = "")
    {
        var accessToken = await GetUserAccessToken(keycloakFixture.PlatformAdmin.Username, keycloakFixture.PlatformAdmin.Password);

        var request = HttpRequestMessageBuilder.Create(httpClient, HttpMethod.Post)
            .WithPath("/api/groups")
            .WithContent(new CreateGroupRequest(name, description ?? string.Empty))
            .WithAuthorization(accessToken)
            .Build();

        HttpResponseMessage? response = await httpClient.SendAsync(request, CancellationToken.None);
        response.EnsureSuccessStatusCode();

        var locationHeader = response.Headers.Location?.ToString();
        var groupId = locationHeader?.Split('/').Last();

        return Guid.Parse(groupId!);
    }
}
