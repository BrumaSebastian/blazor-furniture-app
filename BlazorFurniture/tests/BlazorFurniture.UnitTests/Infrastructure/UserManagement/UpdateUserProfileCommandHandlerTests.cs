using System.Net;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Commands;
using BlazorFurniture.Application.Features.UserManagement.Requests;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Commands;

namespace BlazorFurniture.UnitTests.Infrastructure.UserManagement;

public class UpdateUserProfileCommandHandlerTests
{
    private sealed class FakeClient : IUserManagementClient
    {
        private readonly Func<Guid, HttpResult<UserRepresentation, ErrorRepresentation>> _get;
        private readonly Func<UserRepresentation, HttpResult<EmptyResult, ErrorRepresentation>> _update;

        public FakeClient(
            Func<Guid, HttpResult<UserRepresentation, ErrorRepresentation>> get,
            Func<UserRepresentation, HttpResult<EmptyResult, ErrorRepresentation>> update)
        {
            _get = get;
            _update = update;
        }

        public Task<HttpResult<UserRepresentation, ErrorRepresentation>> Get(Guid userId, CancellationToken ct)
            => Task.FromResult(_get(userId));

        public Task<HttpResult<EmptyResult, ErrorRepresentation>> UpdateProfile(UserRepresentation userRepresentation, CancellationToken ct)
            => Task.FromResult(_update(userRepresentation));

        // Not used
        public Task<HttpResult<UserPermissionsRepresentation, ErrorRepresentation>> GetPermissions(Guid userId, CancellationToken ct)
            => throw new NotImplementedException();
        public Task<HttpResult<GroupRepresentation, ErrorRepresentation>> GetGroups(Guid userId, CancellationToken ct)
            => throw new NotImplementedException();
        public Task<HttpResult<EmptyResult, ErrorRepresentation>> UpdateCredentials(UserRepresentation userRepresentation, CancellationToken ct)
            => throw new NotImplementedException();
    }

    private sealed class FakeErrorMapper : IHttpErrorMapper
    {
        public BasicError MapFor<TError>(TError error, HttpStatusCode status, Guid resourceId, Type? resourceType = null) where TError : class
        {
            return status switch
            {
                HttpStatusCode.NotFound => new NotFoundError(resourceId, resourceType ?? typeof(object)),
                _ => new GenericError("unexpected")
            };
        }
    }

    [Fact]
    public async Task HandleAsync_UpdatesUser_OnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateUserProfileRequest { FirstName = "Jane", LastName = "Smith", Email = "jane@site.test" };

        var client = new FakeClient(
            _ => HttpResult<UserRepresentation, ErrorRepresentation>.Succeeded(new UserRepresentation
            {
                Id = id,
                Username = "jane",
                FirstName = "Old",
                LastName = "Name",
                Email = "old@site.test"
            }),
            u => HttpResult<EmptyResult, ErrorRepresentation>.NoContent());

        var handler = new UpdateUserProfileCommandHandler(client, new FakeErrorMapper());

        // Act
        var result = await handler.HandleAsync(new UpdateUserProfileCommand(id, request));

        // Assert
        Assert.True(result.IsSuccess);
        // EmptyResult success
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task HandleAsync_PropagatesNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateUserProfileRequest { FirstName = "Jane", LastName = "Smith", Email = "jane@site.test" };

        var client = new FakeClient(
            _ => HttpResult<UserRepresentation, ErrorRepresentation>.Failed(new ErrorRepresentation { Error = "nf" }, HttpStatusCode.NotFound),
            _ => throw new InvalidOperationException("Should not update when not found"));

        var handler = new UpdateUserProfileCommandHandler(client, new FakeErrorMapper());

        // Act
        var result = await handler.HandleAsync(new UpdateUserProfileCommand(id, request));

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
    }
}
