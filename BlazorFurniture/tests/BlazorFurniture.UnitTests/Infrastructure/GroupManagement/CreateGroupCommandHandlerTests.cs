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

public class CreateGroupCommandHandlerTests
{
    private readonly IGroupManagementClient clientMock;
    private readonly IHttpErrorMapper errorMapper;
    private readonly CreateGroupCommandHandler handler;

    public CreateGroupCommandHandlerTests()
    {
        clientMock = Substitute.For<IGroupManagementClient>();
        errorMapper = new KeycloakHttpErrorMapper();
        handler = new CreateGroupCommandHandler(clientMock, errorMapper);
    }

    [Fact]
    public async Task HandleAsync_CreatesGroup_OnSuccess()
    {
        // Arrange
        var groupName = "TestGroup";
        var id = Guid.NewGuid();
        var command = new CreateGroupCommand(new CreateGroupRequest(groupName, string.Empty));
        var location = new Uri($"https://example.com/groups/{id}");

        clientMock.Create(groupName, string.Empty, Arg.Any<CancellationToken>())
            .Returns(HttpResult<HttpHeaderLocationResult, ErrorRepresentation>
                .Succeeded(new HttpHeaderLocationResult { Location = location }));

        // Act
        var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(location, result.Value.Location);
        await clientMock.Received(1).Create(groupName, string.Empty, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_PropagatesError_OnFailure()
    {
        // Arrange
        var groupName = "TestGroup";
        var command = new CreateGroupCommand(new CreateGroupRequest(groupName, string.Empty));

        clientMock.Create(groupName, string.Empty, Arg.Any<CancellationToken>())
            .Returns(HttpResult<HttpHeaderLocationResult, ErrorRepresentation>
                .Failed(new ErrorRepresentation { Error = "name already registered" }, HttpStatusCode.Conflict));

        // Act
        var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(typeof(ConflictError), result.Error!.GetType());
        await clientMock.Received(1).Create(groupName, string.Empty, Arg.Any<CancellationToken>());
    }
}
