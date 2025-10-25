using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients;

internal class UserManagementClient( Endpoints endpoints, HttpClient httpClient, KeycloakConfiguration configuration, IMemoryCache cache )
    : KeycloakBaseHttpClient(endpoints, httpClient, configuration, cache), IUserManagementClient
{
    public async Task<HttpResult<UserRepresentation, ErrorRepresentation>> Get( Guid userId, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
            .Create(HttpClient, HttpMethod.Get)
            .WithPath(Endpoints.UserById(userId))
            .Build();

        return await SendRequest<UserRepresentation, ErrorRepresentation>(requestMessage, ct);
    }

    public Task<HttpResult<GroupRepresentation, ErrorRepresentation>> GetGroups( Guid userId, CancellationToken ct )
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResult<UserPermissionsRepresentation, ErrorRepresentation>> GetPermissions( Guid userId, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
           .Create(HttpClient, HttpMethod.Get)
           .WithPath(Endpoints.UserPermissions(userId))
           .Build();

        return await SendRequest<UserPermissionsRepresentation, ErrorRepresentation>(requestMessage, ct);
    }

    public Task<HttpResult<EmptyResult, ErrorRepresentation>> UpdateCredentials( UserRepresentation userRepresentation, CancellationToken ct )
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResult<EmptyResult, ErrorRepresentation>> UpdateProfile( UserRepresentation userRepresentation, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
            .Create(HttpClient, HttpMethod.Put)
            .WithPath(Endpoints.UserById(userRepresentation.Id))
            .WithContent(userRepresentation)
            .Build();

        return await SendRequest<EmptyResult, ErrorRepresentation>(requestMessage, ct);
    }
}
