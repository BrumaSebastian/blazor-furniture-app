using AutoFixture;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Core.Shared.Errors;
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
    private readonly Fixture fixture;

    public GetUserProfileQueryHandlerTests()
    {
        fixture = new Fixture();
    }

    [Fact]
    public async Task HandleAsync_ReturnsMappedProfile_OnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var client = new Mock<IUserManagementClient>();
        var userRepresentation = fixture.Build<UserRepresentation>()
            .With(u => u.Id, id)
            .Create();

        client.Setup(c => c.Get(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<UserRepresentation, ErrorRepresentation>.Succeeded(userRepresentation));

        var handler = new GetUserProfileQueryHandler(client.Object, new KeycloakHttpErrorMapper());

        // Act
        var result = await handler.HandleAsync(new GetUserProfileQuery(id), TestContext.Current.CancellationToken);

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
        var errorRepresentation = fixture.Build<ErrorRepresentation>()
            .With(e => e.Error, "not_found")
            .Without(e => e.Errors)
            .Create();

        client
            .Setup(c => c.Get(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<UserRepresentation, ErrorRepresentation>.Failed(
                errorRepresentation,
                HttpStatusCode.NotFound));

        var handler = new GetUserProfileQueryHandler(client.Object, new KeycloakHttpErrorMapper());

        // Act
        var result = await handler.HandleAsync(new GetUserProfileQuery(id), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
        client.Verify(c => c.Get(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
