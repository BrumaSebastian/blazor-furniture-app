using FastEndpoints;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public class GetGroupByIdRequest
{
    [RouteParam]
    public Guid GroupId { get; init; }
}
