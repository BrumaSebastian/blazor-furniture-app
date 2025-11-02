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

    public async Task<HttpResult<EmptyResult, ErrorRepresentation>> AddUser( Guid groupId, Guid userId, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
           .Create(HttpClient, HttpMethod.Post)
           .WithPath(Endpoints.GroupMemberByIdExtension(groupId, userId))
           .Build();

        return await SendRequest<EmptyResult, ErrorRepresentation>(requestMessage, ct);
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
            .WithPath(Endpoints.GroupsExtension())
            .AddQueryParam(KeycloakQueryParams.FIRST, filters.Page * filters.PageSize)
            .AddQueryParam(KeycloakQueryParams.PAGE_SIZE, filters.PageSize)
            .AddQueryParam(KeycloakQueryParams.SEARCH, filters.Search)
            .AddQueryParam(GROUP_TOP_LEVEL_ONLY_QUERY_PARAM, true)
            .Build();

        return await SendRequest<List<GroupRepresentation>, ErrorRepresentation>(requestMessage, ct);
    }

    public async Task<HttpResult<CountRepresentation, ErrorRepresentation>> GetGroupsCount( string? search, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
            .Create(HttpClient, HttpMethod.Get)
            .WithPath(Endpoints.GroupsCount())
            .AddQueryParam(KeycloakQueryParams.SEARCH, search)
            .Build();

        return await SendRequest<CountRepresentation, ErrorRepresentation>(requestMessage, ct);
    }

    public async Task<HttpResult<List<GroupUserRepresentation>, ErrorRepresentation>> GetUsers( Guid groupId, GroupUsersQueryFilter filters, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
            .Create(HttpClient, HttpMethod.Get)
            .WithPath(Endpoints.GroupMembersExtension(groupId))
            .AddQueryParam(KeycloakQueryParams.SEARCH, filters.Search)
            .AddQueryParam(KeycloakQueryParams.EXACT, false)
            .AddQueryParam(KeycloakQueryParams.FIRST, filters.Page * filters.PageSize)
            .AddQueryParam(KeycloakQueryParams.PAGE_SIZE, filters.PageSize)
            .Build();

        return await SendRequest<List<GroupUserRepresentation>, ErrorRepresentation>(requestMessage, ct);
    }

    public async Task<HttpResult<CountRepresentation, ErrorRepresentation>> GetUsersCount( Guid groupId, string? search, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
            .Create(HttpClient, HttpMethod.Get)
            .WithPath(Endpoints.GroupMembersCount(groupId))
            .AddQueryParam(KeycloakQueryParams.SEARCH, search)
            .AddQueryParam(KeycloakQueryParams.EXACT, false)
            .Build();

        return await SendRequest<CountRepresentation, ErrorRepresentation>(requestMessage, ct);
    }

    public async Task<HttpResult<EmptyResult, ErrorRepresentation>> RemoveUser( Guid groupId, Guid userId, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
           .Create(HttpClient, HttpMethod.Delete)
           .WithPath(Endpoints.GroupMemberByIdExtension(groupId, userId))
           .Build();

        return await SendRequest<EmptyResult, ErrorRepresentation>(requestMessage, ct);
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
