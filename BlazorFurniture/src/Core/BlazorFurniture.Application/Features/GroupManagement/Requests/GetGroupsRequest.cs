using BlazorFurniture.Application.Features.GroupManagement.Requests.Filters;
using FastEndpoints;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public sealed record GetGroupsRequest
{
    [FromQuery]
    public GroupQueryFilters Filters { get; init; } = new();
}
