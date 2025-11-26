using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Requests.Filters;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
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

        return await SendRequest<UserRepresentation>(requestMessage, ct);
    }

    public async Task<HttpResult<List<ExtendedUserRepresentation>, ErrorRepresentation>> Get( UsersQueryFilters filters, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
            .Create(HttpClient, HttpMethod.Get)
            .WithPath(Endpoints.UsersExtension())
            .AddQueryParam(KeycloakQueryParams.FIRST, filters.Page * filters.PageSize)
            .AddQueryParam(KeycloakQueryParams.PAGE_SIZE, filters.PageSize)
            .AddQueryParam(KeycloakQueryParams.SEARCH, filters.Search)
            .Build();

        return await SendRequest<List<ExtendedUserRepresentation>>(requestMessage, ct);
    }

    public async Task<HttpResult<UserPermissionsRepresentation, ErrorRepresentation>> GetPermissions( Guid userId, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
           .Create(HttpClient, HttpMethod.Get)
           .WithPath(Endpoints.UserPermissions(userId))
           .Build();

        return await SendRequest<UserPermissionsRepresentation>(requestMessage, ct);
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

        return await SendRequest<EmptyResult>(requestMessage, ct);
    }

    public async Task<HttpResult<List<UserGroupRepresentation>, ErrorRepresentation>> GetGroups( Guid userId, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
           .Create(HttpClient, HttpMethod.Get)
           .WithPath(Endpoints.UserGroups(userId))
           .Build();

        return await SendRequest<List<UserGroupRepresentation>>(requestMessage, ct);
    }

    public async Task<HttpResult<CountRepresentation, ErrorRepresentation>> Count( string? search, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
            .Create(HttpClient, HttpMethod.Get)
            .WithPath(Endpoints.UsersCount())
            .AddQueryParam(KeycloakQueryParams.SEARCH, search)
            .Build();

        return await SendRequest<CountRepresentation>(requestMessage, ct);
    }
}
