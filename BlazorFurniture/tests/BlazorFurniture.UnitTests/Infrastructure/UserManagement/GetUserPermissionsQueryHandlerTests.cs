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
using PlatformRoles = BlazorFurniture.Domain.Enums.PlatformRoles;

namespace BlazorFurniture.UnitTests.Infrastructure.UserManagement;

public class GetUserPermissionsQueryHandlerTests
{
    private readonly Fixture fixture;
    private readonly IUserManagementClient clientMock;
    private readonly IHttpErrorMapper errorMapper;
    private readonly GetUserPermissionsQueryHandler handler;

    public GetUserPermissionsQueryHandlerTests()
    {
        fixture = new Fixture();
        clientMock = Substitute.For<IUserManagementClient>();
        errorMapper = new KeycloakHttpErrorMapper();
        handler = new GetUserPermissionsQueryHandler(clientMock, errorMapper);
    }

    [Fact]
    public async Task HandleAsync_ReturnsMappedPermissions_OnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var userPermissionRepresentation = fixture.Build<UserPermissionsRepresentation>()
            .With(u => u.Role, PlatformRoles.User)
            .Create();

        clientMock.GetPermissions(id, Arg.Any<CancellationToken>())
            .Returns(HttpResult<UserPermissionsRepresentation, ErrorRepresentation>.Succeeded(userPermissionRepresentation));

        // Act
        var result = await handler.HandleAsync(new GetUserPermissionsQuery(id), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userPermissionRepresentation.Role, result.Value.Role);
        Assert.Equal(userPermissionRepresentation.Permissions, result.Value.Permissions);
        Assert.Equal(userPermissionRepresentation.Groups.Select(g => g.Name), result.Value.Groups!.Select(g => g.Name));
        await clientMock.Received(1).GetPermissions(id, Arg.Any<CancellationToken>());
    }
}
