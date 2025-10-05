using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using Microsoft.Extensions.Caching.Memory;

namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients;

internal class UserManagementClient( Endpoints endpoints, HttpClient httpClient, KeycloakConfiguration configuration, IMemoryCache cache )
    : KeycloakBaseHttpClient( endpoints, httpClient, configuration, cache ), IUserManagementClient
{
    public async Task<Result<UserRepresentation>> Get( Guid userId, CancellationToken ct )
    {
        var requestMessage = HttpRequestMessageBuilder
            .Create( HttpClient, HttpMethod.Get)
            .WithPath( Endpoints.UserById(userId))
            .Build();

        return await SendRequest<UserRepresentation>(requestMessage, ct);
    }

    public Task<Result<UserPermissionsRepresentation>> GetPermissions( Guid userId, CancellationToken ct )
    {
        throw new NotImplementedException();
    }
}
