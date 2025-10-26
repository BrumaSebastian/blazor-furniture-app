using FastEndpoints;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public sealed class AddUserToGroupRequest : GetGroupRequest
{
    [RouteParam]
    public Guid UserId { get; init; }
}
