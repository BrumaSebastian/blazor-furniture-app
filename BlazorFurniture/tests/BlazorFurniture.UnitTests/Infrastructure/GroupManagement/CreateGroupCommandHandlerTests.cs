using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Commands;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;
using Moq;

namespace BlazorFurniture.UnitTests.Infrastructure.GroupManagement;

public class CreateGroupCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_CreatesGroup_OnSuccess()
    {
        // Arrange
        var groupName = "TestGroup";
        var id = Guid.NewGuid();
        var command = new CreateGroupCommand(new CreateGroupRequest(groupName));
        var client = new Mock<IGroupManagementClient>();
        var location = new Uri($"https://example.com/groups/{id}");
        client.Setup(c => c.Create(groupName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<HttpHeaderLocationResult, ErrorRepresentation>
                .Succeeded(new HttpHeaderLocationResult { Location = location }));
        var handler = new CreateGroupCommandHandler(client.Object, new KeycloakHttpErrorMapper());

        // Act
        var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(location, result.Value.Location);
        client.Verify(c => c.Create(groupName, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_PropagatesError_OnFailure()
    {
        // Arrange
        var groupName = "TestGroup";
        var command = new CreateGroupCommand(new CreateGroupRequest(groupName));
        var client = new Mock<IGroupManagementClient>();
        client.Setup(c => c.Create(groupName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<HttpHeaderLocationResult, ErrorRepresentation>
                .Failed(new ErrorRepresentation { Error = "name already registered" }, System.Net.HttpStatusCode.Conflict));
        var handler = new CreateGroupCommandHandler(client.Object, new KeycloakHttpErrorMapper());

        // Act
        var result = await handler.HandleAsync(command, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(typeof(ConflictError), result.Error!.GetType());
        client.Verify(c => c.Create(groupName, It.IsAny<CancellationToken>()), Times.Once);
    }
}
