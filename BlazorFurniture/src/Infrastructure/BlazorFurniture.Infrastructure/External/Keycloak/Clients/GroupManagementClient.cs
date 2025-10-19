using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Requests.Filters;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients;

internal class GroupManagementClient( Endpoints endpoints, HttpClient httpClient, KeycloakConfiguration configuration, IMemoryCache cache )
    : KeycloakBaseHttpClient(endpoints, httpClient, configuration, cache), IGroupManagementClient
{
    private const string GROUP_TOP_LEVEL_ONLY_QUERY_PARAM = "top";

    public Task<HttpResult<EmptyResult, ErrorRepresentation>> AddUser( Guid groupId, IEnumerable<Guid> userIds, CancellationToken ct )
    {
        throw new NotImplementedException();
    }

    public Task<HttpResult<EmptyResult, ErrorRepresentation>> AddUser( Guid groupId, Guid userId, CancellationToken ct )
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

    public async Task<HttpResult<GroupRepresentation, ErrorRepresentation>> Get( Guid groupId, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
           .Create(HttpClient, HttpMethod.Get)
           .WithPath(Endpoints.GroupById(groupId))
           .Build();

        return await SendRequest<GroupRepresentation, ErrorRepresentation>(requestMessage, ct);
    }

    public async Task<HttpResult<List<GroupRepresentation>, ErrorRepresentation>> Get( GroupQueryFilters filters, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
            .Create(HttpClient, HttpMethod.Get)
            .WithPath(Endpoints.Groups())
            .AddQueryParam(KeycloakQueryParams.PAGE, filters.Page)
            .AddQueryParam(KeycloakQueryParams.PAGE_SIZE, filters.PageSize)
            .AddQueryParam(KeycloakQueryParams.SEARCH, filters.Name)
            .Build();

        return await SendRequest<List<GroupRepresentation>, ErrorRepresentation>(requestMessage, ct);
    }

    public async Task<HttpResult<CountRepresentation, ErrorRepresentation>> GetGroupsCount( CancellationToken ct )
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

    public async Task<HttpResult<EmptyResult, ErrorRepresentation>> Update( Guid groupId, GroupRepresentation groupRepresentation, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
           .Create(HttpClient, HttpMethod.Put)
           .WithPath(Endpoints.GroupById(groupId))
           .WithContent(groupRepresentation)
           .Build();

        return await SendRequest<EmptyResult, ErrorRepresentation>(requestMessage, ct);
    }
}
