using FastEndpoints;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public sealed class UpdateUserGroupRoleRequest : GetGroupRequest
{
    [RouteParam]
    public Guid UserId { get; init; }
    [RouteParam]
    public Guid RoleId { get; init; }
}
