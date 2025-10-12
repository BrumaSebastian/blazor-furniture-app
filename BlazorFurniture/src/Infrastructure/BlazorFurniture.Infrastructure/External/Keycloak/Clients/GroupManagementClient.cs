using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients;

internal class GroupManagementClient( Endpoints endpoints, HttpClient httpClient, KeycloakConfiguration configuration, IMemoryCache cache )
    : KeycloakBaseHttpClient(endpoints, httpClient, configuration, cache), IGroupManagementClient
{
    public Task<HttpResult<EmptyResult, ErrorRepresentation>> AddUsers( Guid groupId, IEnumerable<Guid> userIds, CancellationToken ct )
    {
        throw new NotImplementedException();
    }

    public Task<HttpResult<HttpHeaderLocationResult, ErrorRepresentation>> Create( string groupName, CancellationToken ct )
    {
        throw new NotImplementedException();
    }

    public Task<HttpResult<GroupRepresentation, ErrorRepresentation>> Get( Guid groupId, CancellationToken ct )
    {
        throw new NotImplementedException();
    }

    public Task<HttpResult<List<GroupRepresentation>, ErrorRepresentation>> Get( CancellationToken ct )
    {
        throw new NotImplementedException();
    }

    public Task<HttpResult<List<UserRepresentation>, ErrorRepresentation>> GetUsers( Guid groupId, CancellationToken ct )
    {
        throw new NotImplementedException();
    }

    public Task<HttpResult<EmptyResult, ErrorRepresentation>> RemoveUser( Guid groupId, Guid userId, CancellationToken ct )
    {
        throw new NotImplementedException();
    }
}
