using BlazorFurniture.Application.Features.GroupManagement.Requests.Filters;
using FastEndpoints;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public sealed class GetGroupsRequest
{
    [FromQuery]
    public required GroupQueryFilters QueryFilters { get; set; }
}
