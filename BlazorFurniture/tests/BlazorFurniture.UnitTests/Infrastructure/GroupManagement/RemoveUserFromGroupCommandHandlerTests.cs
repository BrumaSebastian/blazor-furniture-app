using AutoFixture;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Commands;
using Moq;
using System.Net;

namespace BlazorFurniture.UnitTests.Infrastructure.GroupManagement;

public class RemoveUserFromGroupCommandHandlerTests
{
    private readonly Fixture fixture;
    private readonly IHttpErrorMapper errorMapper;
    public RemoveUserFromGroupCommandHandlerTests()
    {
        fixture = new Fixture();
        errorMapper = new KeycloakHttpErrorMapper();
    }

    [Fact]
    public async Task HandleAsync_RemoveUserFromGroup_OnSuccess()
    {
        // Arrange
        var request = fixture.Create<RemoveUserFromGroupRequest>();
        var command = new RemoveUserFromGroupCommand(request);
        var client = new Mock<IGroupManagementClient>();
        client.Setup(c => c.RemoveUser(request.GroupId, request.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<EmptyResult, ErrorRepresentation>.Succeeded(new EmptyResult()));
        var handler = new RemoveUserFromGroupCommandHandler(client.Object, errorMapper);

        // Act
        var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        client.Verify(c => c.RemoveUser(request.GroupId, request.UserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_PropagatesError_OnFailure()
    {
        // Arrange
        var request = fixture.Create<RemoveUserFromGroupRequest>();
        var command = new RemoveUserFromGroupCommand(request);
        var client = new Mock<IGroupManagementClient>();
        client.Setup(c => c.RemoveUser(request.GroupId, request.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<EmptyResult, ErrorRepresentation>
                .Failed(new ErrorRepresentation { Error = "user not found" }, HttpStatusCode.NotFound));
        var handler = new RemoveUserFromGroupCommandHandler(client.Object, errorMapper);

        // Act
        var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(typeof(NotFoundError), result.Error!.GetType());
        client.Verify(c => c.RemoveUser(request.GroupId, request.UserId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
