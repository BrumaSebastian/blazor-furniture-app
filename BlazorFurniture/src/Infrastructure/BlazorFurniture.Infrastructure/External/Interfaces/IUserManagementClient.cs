using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Requests.Filters;
using BlazorFurniture.Domain.Entities.Keycloak;

namespace BlazorFurniture.Infrastructure.External.Interfaces;

internal interface IUserManagementClient
{
    Task<HttpResult<UserRepresentation, ErrorRepresentation>> Get( Guid userId, CancellationToken ct );
    Task<HttpResult<List<ExtendedUserRepresentation>, ErrorRepresentation>> Get( UsersQueryFilters filters, CancellationToken ct );
    Task<HttpResult<CountRepresentation, ErrorRepresentation>> Count( string? search, CancellationToken ct );
    Task<HttpResult<UserPermissionsRepresentation, ErrorRepresentation>> GetPermissions( Guid userId, CancellationToken ct );
    Task<HttpResult<List<UserGroupRepresentation>, ErrorRepresentation>> GetGroups( Guid userId, CancellationToken ct );
    Task<HttpResult<EmptyResult, ErrorRepresentation>> UpdateProfile( UserRepresentation userRepresentation, CancellationToken ct );
    Task<HttpResult<EmptyResult, ErrorRepresentation>> UpdateCredentials( UserRepresentation userRepresentation, CancellationToken ct );
}
