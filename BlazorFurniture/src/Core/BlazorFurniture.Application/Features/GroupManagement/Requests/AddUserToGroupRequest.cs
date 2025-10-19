using FastEndpoints;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public sealed class AddUserToGroupRequest
{
    [RouteParam]
    public Guid GroupId { get; set; }
    [RouteParam]
    public Guid UserId { get; set; }
}
