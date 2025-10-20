using FastEndpoints;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public sealed class AddUserToGroupRequest
{
    [RouteParam]
    public Guid GroupId { get; init; }
    [RouteParam]
    public Guid UserId { get; init; }
}
