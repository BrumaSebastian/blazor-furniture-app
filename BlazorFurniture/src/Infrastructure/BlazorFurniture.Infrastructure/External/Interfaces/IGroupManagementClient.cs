using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Domain.Entities.Keycloak;

namespace BlazorFurniture.Infrastructure.External.Interfaces;

internal interface IGroupManagementClient
{
    Task<HttpResult<HttpHeaderLocationResult, ErrorRepresentation>> Create( string groupName, CancellationToken ct );
    Task<HttpResult<GroupRepresentation, ErrorRepresentation>> Get( Guid groupId, CancellationToken ct );
    Task<HttpResult<List<GroupRepresentation>, ErrorRepresentation>> Get( CancellationToken ct );
    Task<HttpResult<CountRepresentation, ErrorRepresentation>> GetCount( CancellationToken ct );
    Task<HttpResult<EmptyResult, ErrorRepresentation>> AddUsers( Guid groupId, IEnumerable<Guid> userIds, CancellationToken ct );
    Task<HttpResult<List<UserRepresentation>, ErrorRepresentation>> GetUsers( Guid groupId, CancellationToken ct );
    Task<HttpResult<EmptyResult, ErrorRepresentation>> RemoveUser( Guid groupId, Guid userId, CancellationToken ct );
}
