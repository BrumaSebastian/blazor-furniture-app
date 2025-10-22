using AutoFixture;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Commands;
using BlazorFurniture.Application.Features.UserManagement.Requests;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Commands;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;
using Moq;
using System.Net;

namespace BlazorFurniture.UnitTests.Infrastructure.UserManagement;

public class UpdateUserProfileCommandHandlerTests
{
    private readonly Fixture fixture;
    private readonly Mock<IUserManagementClient> clientMock;
    private readonly IHttpErrorMapper errorMapper;
    private readonly UpdateUserProfileCommandHandler handler;

    public UpdateUserProfileCommandHandlerTests()
    {
        fixture = new Fixture();
        clientMock = new Mock<IUserManagementClient>();
        errorMapper = new KeycloakHttpErrorMapper();
        handler = new UpdateUserProfileCommandHandler(clientMock.Object, errorMapper);
    }

    [Fact]
    public async Task HandleAsync_UpdatesUser_OnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = fixture.Create<UpdateUserProfileRequest>();
        var userRepresentation = fixture.Build<UserRepresentation>()
            .With(u => u.Id, id)
            .Create();

        clientMock.Setup(c => c.Get(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<UserRepresentation, ErrorRepresentation>.Succeeded(userRepresentation));

        clientMock.Setup(c => c.UpdateProfile(
            It.Is<UserRepresentation>(u =>
                u.Id == id &&
                u.FirstName == request.FirstName &&
                u.LastName == request.LastName &&
                u.Email == request.Email),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<EmptyResult, ErrorRepresentation>.NoContent());

        // Act
        var result = await handler.HandleAsync(new UpdateUserProfileCommand(id, request), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        clientMock.Verify(c => c.Get(id, It.IsAny<CancellationToken>()), Times.Once);
        clientMock.Verify(c => c.UpdateProfile(It.IsAny<UserRepresentation>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_PropagatesNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = fixture.Create<UpdateUserProfileRequest>();
        var errorRepresentation = fixture.Build<ErrorRepresentation>()
            .With(e => e.Error, "nf")
            .Without(e => e.Errors)
            .Create();

        clientMock.Setup(c => c.Get(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResult<UserRepresentation, ErrorRepresentation>.Failed(
                errorRepresentation,
                HttpStatusCode.NotFound));

        // Act
        var result = await handler.HandleAsync(new UpdateUserProfileCommand(id, request), TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.IsType<NotFoundError>(result.Error);
        clientMock.Verify(c => c.Get(id, It.IsAny<CancellationToken>()), Times.Once);
        clientMock.Verify(c => c.UpdateProfile(It.IsAny<UserRepresentation>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
