using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Controllers.Groups;
using BlazorFurniture.IntegrationTests.Controllers.Setup;
using FastEndpoints;
using FastEndpoints.Testing;
using System.Net;

namespace BlazorFurniture.IntegrationTests.Controllers.Groups;

public class UpdateGroupEndpointTests( ProgramSut sut ) : TestBase<ProgramSut>
{
    [Fact]
    public async Task UpdateGroup_ReturnsNoContent_WhenGroupIsUpdatedSuccessfully()
    {
        // Arrange
        // TODO: Seed a group in your test database
        var groupId = Guid.NewGuid();
        var request = new UpdateGroupRequest
        {
            GroupId = groupId,
            Name = "Updated Group Name"
        };

        // Act
        var (responseMessage, problem) = await sut.Client
            .PUTAsync<UpdateGroupEndpoint, UpdateGroupRequest, ProblemDetails>(request);

        // Assert
        Assert.True(responseMessage.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NoContent, responseMessage.StatusCode);
        Assert.Null(problem);
    }

    [Fact]
    public async Task UpdateGroup_ReturnsNotFound_WhenGroupDoesNotExist()
    {
        // Arrange
        var nonExistentGroupId = Guid.NewGuid();
        var request = new UpdateGroupRequest
        {
            GroupId = nonExistentGroupId,
            Name = "Updated Group Name"
        };

        // Act
        var (responseMessage, problem) = await sut.Client
            .PUTAsync<UpdateGroupEndpoint, UpdateGroupRequest, ProblemDetails>(request);

        // Assert
        Assert.False(responseMessage.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);
        Assert.NotNull(problem);
    }

    [Fact]
    public async Task UpdateGroup_ReturnsConflict_WhenNameAlreadyExists()
    {
        // Arrange
        // TODO: Seed two groups in your test database
        var groupId = Guid.NewGuid();
        var duplicateName = "Existing Group Name";
        var request = new UpdateGroupRequest
        {
            GroupId = groupId,
            Name = duplicateName
        };

        // Act
        var (responseMessage, problem) = await sut.Client
            .PUTAsync<UpdateGroupEndpoint, UpdateGroupRequest, ProblemDetails>(request);

        // Assert
        Assert.False(responseMessage.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.Conflict, responseMessage.StatusCode);
        Assert.NotNull(problem);
    }

    [Fact]
    public async Task UpdateGroup_ReturnsBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var request = new UpdateGroupRequest
        {
            GroupId = Guid.NewGuid(),
            Name = "" // Invalid
        };

        // Act
        var (responseMessage, problem) = await sut.Client
            .PUTAsync<UpdateGroupEndpoint, UpdateGroupRequest, ProblemDetails>(request);

        // Assert
        Assert.False(responseMessage.IsSuccessStatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, responseMessage.StatusCode);
        Assert.NotNull(problem);
    }
}
