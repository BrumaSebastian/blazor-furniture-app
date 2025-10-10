using System.Net;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Queries;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;

namespace BlazorFurniture.UnitTests.Infrastructure.UserManagement;

public class GetUserProfileQueryHandlerTests
{
    private sealed class FakeClient : IUserManagementClient
    {
        private readonly Func<Guid, HttpResult<UserRepresentation, ErrorRepresentation>> _get;
        public FakeClient(Func<Guid, HttpResult<UserRepresentation, ErrorRepresentation>> get) => _get = get;

        public Task<HttpResult<UserRepresentation, ErrorRepresentation>> Get(Guid userId, CancellationToken ct)
            => Task.FromResult(_get(userId));

        // Not used in these tests
        public Task<HttpResult<UserPermissionsRepresentation, ErrorRepresentation>> GetPermissions(Guid userId, CancellationToken ct)
            => throw new NotImplementedException();
        public Task<HttpResult<GroupRepresentation, ErrorRepresentation>> GetGroups(Guid userId, CancellationToken ct)
            => throw new NotImplementedException();
        public Task<HttpResult<EmptyResult, ErrorRepresentation>> UpdateProfile(UserRepresentation userRepresentation, CancellationToken ct)
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
    public async Task HandleAsync_ReturnsMappedProfile_OnSuccess()
    {
        // Arrange
        var id = Guid.NewGuid();
        var client = new FakeClient(_ => HttpResult<UserRepresentation, ErrorRepresentation>.Succeeded(new UserRepresentation
        {
            Id = id,
            Username = "john",
            FirstName = "John",
            LastName = "Doe",
            Email = "john@site.test"
        }));
        var handler = new GetUserProfileQueryHandler(client, new FakeErrorMapper());

        // Act
        var result = await handler.HandleAsync(new GetUserProfileQuery(id));

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("john", result.Value.Username);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
        Assert.Equal("john@site.test", result.Value.Email);
    }

    [Fact]
    public async Task HandleAsync_PropagatesNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var client = new FakeClient(_ => HttpResult<UserRepresentation, ErrorRepresentation>.Failed(new ErrorRepresentation
        {
            Error = "not_found"
        }, HttpStatusCode.NotFound));
        var handler = new GetUserProfileQueryHandler(client, new FakeErrorMapper());

        // Act
        var result = await handler.HandleAsync(new GetUserProfileQuery(id));

        // Assert
        Assert.True(result.IsFailure);
        Assert.IsType<NotFoundError>(result.Error);
    }
}
