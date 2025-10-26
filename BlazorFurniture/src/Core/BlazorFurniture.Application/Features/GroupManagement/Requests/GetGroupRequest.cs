using FastEndpoints;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public class GetGroupRequest
{
    [RouteParam]
    public Guid GroupId { get; init; }
}
