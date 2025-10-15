using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients;

internal class GroupManagementClient( Endpoints endpoints, HttpClient httpClient, KeycloakConfiguration configuration, IMemoryCache cache )
    : KeycloakBaseHttpClient(endpoints, httpClient, configuration, cache), IGroupManagementClient
{
    private const string GROUP_TOP_LEVEL_ONLY_QUERY_PARAM = "top";

    public Task<HttpResult<EmptyResult, ErrorRepresentation>> AddUsers( Guid groupId, IEnumerable<Guid> userIds, CancellationToken ct )
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResult<HttpHeaderLocationResult, ErrorRepresentation>> Create( string groupName, CancellationToken ct )
    {
        var groupRepresentation = new GroupRepresentation
        {
            Name = groupName
        };

        var requestMessage = HttpRequestMessageBuilder
            .Create(HttpClient, HttpMethod.Post)
            .WithPath(Endpoints.GroupsExtension())
            .WithContent(groupRepresentation)
            .Build();

        return await SendRequest<HttpHeaderLocationResult, ErrorRepresentation>(requestMessage, ct);
    }

    public Task<HttpResult<GroupRepresentation, ErrorRepresentation>> Get( Guid groupId, CancellationToken ct )
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResult<List<GroupRepresentation>, ErrorRepresentation>> Get( CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
            .Create(HttpClient, HttpMethod.Get)
            .WithPath(Endpoints.Groups())
            .Build();

        return await SendRequest<List<GroupRepresentation>, ErrorRepresentation>(requestMessage, ct);
    }

    public async Task<HttpResult<CountRepresentation, ErrorRepresentation>> GetCount( CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
            .Create(HttpClient, HttpMethod.Get)
            .WithPath(Endpoints.GroupsCount())
            .AddQueryParam(GROUP_TOP_LEVEL_ONLY_QUERY_PARAM, bool.TrueString)
            .Build();

        return await SendRequest<CountRepresentation, ErrorRepresentation>(requestMessage, ct);
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
