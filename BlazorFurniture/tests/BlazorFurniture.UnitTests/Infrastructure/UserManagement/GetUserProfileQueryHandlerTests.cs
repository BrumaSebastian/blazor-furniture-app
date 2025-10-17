using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Queries;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;
using Moq;
using System.Net;

namespace BlazorFurniture.UnitTests.Infrastructure.UserManagement;

public class GetUserProfileQueryHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsMappedProfile_OnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var client = new Mock<IUserManagementClient>();
        var userRepresentation = new UserRepresentation
        {
            Id = id,
            Username = "john",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@site.test"
        };

        client.Setup(c => c.Get(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<UserRepresentation, ErrorRepresentation>.Succeeded(userRepresentation));

        var handler = new GetUserProfileQueryHandler(client.Object, new KeycloakHttpErrorMapper());

        // Act
        var result = await handler.HandleAsync(new GetUserProfileQuery(id));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userRepresentation.Username, result.Value.Username);
        Assert.Equal(userRepresentation.FirstName, result.Value.FirstName);
        Assert.Equal(userRepresentation.LastName, result.Value.LastName);
        Assert.Equal(userRepresentation.Email, result.Value.Email);
        client.Verify(c => c.Get(id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_PropagatesNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var client = new Mock<IUserManagementClient>();
        client
            .Setup(c => c.Get(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<UserRepresentation, ErrorRepresentation>.Failed(new ErrorRepresentation
            {
                Error = "not_found"
            }, HttpStatusCode.NotFound));

        var handler = new GetUserProfileQueryHandler(client.Object, new KeycloakHttpErrorMapper());

        // Act
        var result = await handler.HandleAsync(new GetUserProfileQuery(id));

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<BlazorFurniture.Core.Shared.Errors.NotFoundError>(result.Error);
        client.Verify(c => c.Get(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
