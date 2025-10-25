using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Commands;
using NSubstitute;
using System.Net;

namespace BlazorFurniture.UnitTests.Infrastructure.GroupManagement;

public class AddUserToGroupCommandHandlerTests
{
    private readonly IGroupManagementClient clientMock;
    private readonly IHttpErrorMapper errorMapper;
    private readonly AddUserToGroupCommandHandler handler;

    public AddUserToGroupCommandHandlerTests()
    {
        clientMock = Substitute.For<IGroupManagementClient>();
        errorMapper = new KeycloakHttpErrorMapper();
        handler = new AddUserToGroupCommandHandler(clientMock, errorMapper);
    }

    [Fact]
    public async Task HandleAsync_AddsUserToGroup_OnSuccess()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new AddUserToGroupCommand(new AddUserToGroupRequest { GroupId = groupId, UserId = userId });

        clientMock.AddUser(groupId, userId, Arg.Any<CancellationToken>())
            .Returns(HttpResult<EmptyResult, ErrorRepresentation>.Succeeded(new EmptyResult()));

        // Act
        var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        await clientMock.Received(1).AddUser(groupId, userId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_PropagatesError_OnFailure()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new AddUserToGroupCommand(new AddUserToGroupRequest { GroupId = groupId, UserId = userId });

        clientMock.AddUser(groupId, userId, Arg.Any<CancellationToken>())
            .Returns(HttpResult<EmptyResult, ErrorRepresentation>
                .Failed(new ErrorRepresentation { Error = "user not found" }, HttpStatusCode.NotFound));

        // Act
        var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(typeof(NotFoundError), result.Error!.GetType());
        await clientMock.Received(1).AddUser(groupId, userId, Arg.Any<CancellationToken>());
    }
}
