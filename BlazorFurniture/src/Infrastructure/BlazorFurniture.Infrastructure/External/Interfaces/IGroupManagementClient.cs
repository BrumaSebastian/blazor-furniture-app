using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Requests.Filters;
using BlazorFurniture.Domain.Entities.Keycloak;

namespace BlazorFurniture.Infrastructure.External.Interfaces;

internal interface IGroupManagementClient
{
    Task<HttpResult<HttpHeaderLocationResult, ErrorRepresentation>> Create( string groupName, string description, CancellationToken ct );
    Task<HttpResult<GroupRepresentation, ErrorRepresentation>> Get( Guid groupId, CancellationToken ct );
    Task<HttpResult<List<GroupRepresentation>, ErrorRepresentation>> Get( GroupQueryFilters filters, CancellationToken ct );
    Task<HttpResult<CountRepresentation, ErrorRepresentation>> GetGroupsCount( string? search, CancellationToken ct );
    Task<HttpResult<CountRepresentation, ErrorRepresentation>> GetUsersCount( Guid groupId, string? search, CancellationToken ct );
    Task<HttpResult<List<GroupRoleRepresentation>, ErrorRepresentation>> GetRoles( Guid groupId, CancellationToken ct );
    Task<HttpResult<EmptyResult, ErrorRepresentation>> AddUser( Guid groupId, Guid userId, CancellationToken ct );
    Task<HttpResult<List<GroupUserRepresentation>, ErrorRepresentation>> GetUsers( Guid groupId, GroupUsersQueryFilter filters, CancellationToken ct );
    Task<HttpResult<EmptyResult, ErrorRepresentation>> RemoveUser( Guid groupId, Guid userId, CancellationToken ct );
    Task<HttpResult<EmptyResult, ErrorRepresentation>> Update( Guid groupId, GroupRepresentation groupRepresentation, CancellationToken ct );
    Task<HttpResult<EmptyResult, ErrorRepresentation>> UpdateUserRole( Guid groupId, Guid userId, Guid roleId, CancellationToken ct );
}
