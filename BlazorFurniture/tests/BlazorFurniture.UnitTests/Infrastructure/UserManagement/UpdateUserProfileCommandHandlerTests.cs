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
using NSubstitute;
using System.Net;

namespace BlazorFurniture.UnitTests.Infrastructure.UserManagement;

public class UpdateUserProfileCommandHandlerTests
{
    private readonly Fixture fixture;
    private readonly IUserManagementClient clientMock;
    private readonly IHttpErrorMapper errorMapper;
    private readonly UpdateUserProfileCommandHandler handler;

    public UpdateUserProfileCommandHandlerTests()
    {
        fixture = new Fixture();
        clientMock = Substitute.For<IUserManagementClient>();
        errorMapper = new KeycloakHttpErrorMapper();
        handler = new UpdateUserProfileCommandHandler(clientMock, errorMapper);
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

        clientMock.Get(id, Arg.Any<CancellationToken>())
            .Returns(HttpResult<UserRepresentation, ErrorRepresentation>.Succeeded(userRepresentation));

        clientMock.UpdateProfile(
                Arg.Is<UserRepresentation>(u =>
                    u.Id == id
                    && u.FirstName == request.FirstName
                    && u.LastName == request.LastName
                    && u.Email == request.Email),
                Arg.Any<CancellationToken>())
            .Returns(HttpResult<EmptyResult, ErrorRepresentation>.NoContent());

        // Act
        var result = await handler.HandleAsync(new UpdateUserProfileCommand(id, request), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        await clientMock.Received(1).Get(id, Arg.Any<CancellationToken>());
        await clientMock.Received(1).UpdateProfile(Arg.Any<UserRepresentation>(), Arg.Any<CancellationToken>());
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

        clientMock.Get(id, Arg.Any<CancellationToken>())
            .Returns(HttpResult<UserRepresentation, ErrorRepresentation>.Failed(
                errorRepresentation, HttpStatusCode.NotFound));

        // Act
        var result = await handler.HandleAsync(new UpdateUserProfileCommand(id, request), TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.IsType<NotFoundError>(result.Error);
        await clientMock.Received(1).Get(id, Arg.Any<CancellationToken>());
        await clientMock.DidNotReceive().UpdateProfile(Arg.Any<UserRepresentation>(), Arg.Any<CancellationToken>());
    }
}
