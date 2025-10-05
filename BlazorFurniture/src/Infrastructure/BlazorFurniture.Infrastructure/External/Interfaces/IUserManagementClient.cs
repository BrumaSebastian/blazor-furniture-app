using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;

namespace BlazorFurniture.Infrastructure.External.Interfaces;

internal interface IUserManagementClient
{
    Task<Result<UserRepresentation>> Get( Guid userId, CancellationToken ct );
    Task<Result<UserPermissionsRepresentation>> GetPermissions( Guid userId, CancellationToken ct );
}
