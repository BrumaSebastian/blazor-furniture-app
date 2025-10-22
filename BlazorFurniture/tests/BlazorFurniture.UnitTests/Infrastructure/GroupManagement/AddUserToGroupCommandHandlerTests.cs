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

public class AddUserToGroupCommandHandlerTests
{
    private readonly Mock<IGroupManagementClient> clientMock;
    private readonly IHttpErrorMapper errorMapper;
    private readonly AddUserToGroupCommandHandler handler;

    public AddUserToGroupCommandHandlerTests()
    {
        clientMock = new Mock<IGroupManagementClient>();
        errorMapper = new KeycloakHttpErrorMapper();
        handler = new AddUserToGroupCommandHandler(clientMock.Object, errorMapper);
    }

    [Fact]
    public async Task HandleAsync_AddsUserToGroup_OnSuccess()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new AddUserToGroupCommand(new AddUserToGroupRequest { GroupId = groupId, UserId = userId });

        clientMock.Setup(c => c.AddUser(groupId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<EmptyResult, ErrorRepresentation>.Succeeded(new EmptyResult()));

        // Act
        var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        clientMock.Verify(c => c.AddUser(groupId, userId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_PropagatesError_OnFailure()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new AddUserToGroupCommand(new AddUserToGroupRequest { GroupId = groupId, UserId = userId });

        clientMock.Setup(c => c.AddUser(groupId, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<EmptyResult, ErrorRepresentation>
                .Failed(new ErrorRepresentation { Error = "user not found" }, HttpStatusCode.NotFound));

        // Act
        var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(typeof(NotFoundError), result.Error!.GetType());
        clientMock.Verify(c => c.AddUser(groupId, userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
