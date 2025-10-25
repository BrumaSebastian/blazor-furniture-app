using AutoFixture;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Queries;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;
using NSubstitute;
using System.Net;

namespace BlazorFurniture.UnitTests.Infrastructure.UserManagement;

public class GetUserProfileQueryHandlerTests
{
    private readonly Fixture fixture;
    private readonly IUserManagementClient clientMock;
    private readonly IHttpErrorMapper errorMapper;
    private readonly GetUserProfileQueryHandler handler;

    public GetUserProfileQueryHandlerTests()
    {
        fixture = new Fixture();
        clientMock = Substitute.For<IUserManagementClient>();
        errorMapper = new KeycloakHttpErrorMapper();
        handler = new GetUserProfileQueryHandler(clientMock, errorMapper);
    }

    [Fact]
    public async Task HandleAsync_ReturnsMappedProfile_OnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userRepresentation = fixture.Build<UserRepresentation>()
            .With(u => u.Id, id)
            .Create();

        clientMock.Get(id, Arg.Any<CancellationToken>())
            .Returns(HttpResult<UserRepresentation, ErrorRepresentation>.Succeeded(userRepresentation));

        // Act
        var result = await handler.HandleAsync(new GetUserProfileQuery(id), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userRepresentation.Username, result.Value.Username);
        Assert.Equal(userRepresentation.FirstName, result.Value.FirstName);
        Assert.Equal(userRepresentation.LastName, result.Value.LastName);
        Assert.Equal(userRepresentation.Email, result.Value.Email);
        await clientMock.Received(1).Get(id, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_PropagatesNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var errorRepresentation = fixture.Build<ErrorRepresentation>()
            .With(e => e.Error, "not_found")
            .Without(e => e.Errors)
            .Create();

        clientMock.Get(id, Arg.Any<CancellationToken>())
            .Returns(HttpResult<UserRepresentation, ErrorRepresentation>.Failed(
                errorRepresentation, HttpStatusCode.NotFound));

        // Act
        var result = await handler.HandleAsync(new GetUserProfileQuery(id), TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.IsType<NotFoundError>(result.Error);
        await clientMock.Received(1).Get(id, Arg.Any<CancellationToken>());
    }
}
