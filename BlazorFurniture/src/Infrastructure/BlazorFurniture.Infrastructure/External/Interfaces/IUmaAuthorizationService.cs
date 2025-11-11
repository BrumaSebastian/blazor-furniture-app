using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;

namespace BlazorFurniture.Infrastructure.External.Interfaces;

public interface IUmaAuthorizationService
{
    Task<HttpResult<EmptyResult, ErrorRepresentation>> Evaluate( string accessToken, string permission, CancellationToken ct );
    Task<HttpResult<EmptyResult, ErrorRepresentation>> Evaluate( string accessToken, string permission, Dictionary<string, string> claims, CancellationToken ct );
    Task<HttpResult<List<UmaPermissionsResponse>, ErrorRepresentation>> CheckPermissions( string accessToken, CancellationToken ct );
}
