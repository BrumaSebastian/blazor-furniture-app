using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;

namespace BlazorFurniture.Infrastructure.External.Interfaces;

public interface IUmaAuthorizationService
{
    Task<HttpResult<EmptyResult, ErrorRepresentation>> Evaluate( string accessToken, string permission, CancellationToken ct );
}
