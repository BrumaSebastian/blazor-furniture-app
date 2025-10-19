using BlazorFurniture.Application.Features.GroupManagement.Requests.Filters;
using FastEndpoints;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public sealed record GetGroupsRequest
{
    [FromQuery(IsRequired = false)]
    public GroupQueryFilters Filters { get; init; } = new();
}
