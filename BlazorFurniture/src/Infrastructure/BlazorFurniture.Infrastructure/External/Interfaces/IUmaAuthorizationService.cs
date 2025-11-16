using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;

namespace BlazorFurniture.Infrastructure.External.Interfaces;

public interface IUmaAuthorizationService
{
    Task<HttpResult<UmaAuthorizationResponse, ErrorRepresentation>> Evaluate( string userAccessToken, string resource, List<string> scopes, CancellationToken ct );
    Task<HttpResult<UmaAuthorizationResponse, ErrorRepresentation>> Evaluate( string userAccessToken, string resource, List<string> scopes, IReadOnlyDictionary<string, List<string>> claims, CancellationToken ct );
    Task<HttpResult<List<UmaPermissionsResponse>, ErrorRepresentation>> CheckPermissions( string userAccessToken, CancellationToken ct );
}
