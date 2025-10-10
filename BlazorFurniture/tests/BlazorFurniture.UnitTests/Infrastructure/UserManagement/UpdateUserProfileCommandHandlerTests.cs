using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Commands;
using BlazorFurniture.Application.Features.UserManagement.Requests;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Commands;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;
using Moq;
using System.Net;

namespace BlazorFurniture.UnitTests.Infrastructure.UserManagement;

public class UpdateUserProfileCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_UpdatesUser_OnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateUserProfileRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@site.test"
        };
        var useRepresentation = new UserRepresentation
        {
            Id = id,
            Username = "jane",
            FirstName = "Old",
            LastName = "Name",
            Email = "old@site.test"
        };
        var client = new Mock<IUserManagementClient>();
        client.Setup(c => c.Get(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<UserRepresentation, ErrorRepresentation>.Succeeded(useRepresentation));

        client.Setup(c => c.UpdateProfile(It.Is<UserRepresentation>(u => u.Id == id && u.FirstName == "Jane" && u.LastName == "Smith" && u.Email == "jane@site.test"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<EmptyResult, ErrorRepresentation>.NoContent());

        var handler = new UpdateUserProfileCommandHandler(client.Object, new KeycloakHttpErrorMapper());

        // Act
        var result = await handler.HandleAsync(new UpdateUserProfileCommand(id, request));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        client.Verify(c => c.Get(id, It.IsAny<CancellationToken>()), Times.Once);
        client.Verify(c => c.UpdateProfile(It.IsAny<UserRepresentation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_PropagatesNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateUserProfileRequest { FirstName = "Jane", LastName = "Smith", Email = "jane@site.test" };

        var client = new Mock<IUserManagementClient>();
        client
            .Setup(c => c.Get(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<UserRepresentation, ErrorRepresentation>.Failed(new ErrorRepresentation { Error = "nf" }, HttpStatusCode.NotFound));

        var handler = new UpdateUserProfileCommandHandler(client.Object, new KeycloakHttpErrorMapper());

        // Act
        var result = await handler.HandleAsync(new UpdateUserProfileCommand(id, request));

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<BlazorFurniture.Core.Shared.Errors.NotFoundError>(result.Error);
        client.Verify(c => c.Get(id, It.IsAny<CancellationToken>()), Times.Once);
        client.Verify(c => c.UpdateProfile(It.IsAny<UserRepresentation>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
