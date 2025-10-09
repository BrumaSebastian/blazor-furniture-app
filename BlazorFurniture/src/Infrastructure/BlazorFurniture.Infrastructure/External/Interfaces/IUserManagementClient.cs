using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;

namespace BlazorFurniture.Infrastructure.External.Interfaces;

internal interface IUserManagementClient
{
    Task<HttpResult<UserRepresentation, ErrorRepresentation>> Get( Guid userId, CancellationToken ct );
    Task<HttpResult<UserPermissionsRepresentation, ErrorRepresentation>> GetPermissions( Guid userId, CancellationToken ct );
    Task<HttpResult<GroupRepresentation, ErrorRepresentation>> GetGroups( Guid userId, CancellationToken ct );
    Task<HttpResult<EmptyResult, ErrorRepresentation>> UpdateProfile( UserRepresentation userRepresentation, CancellationToken ct );
    Task<HttpResult<EmptyResult, ErrorRepresentation>> UpdateCredentials( UserRepresentation userRepresentation, CancellationToken ct );
}
