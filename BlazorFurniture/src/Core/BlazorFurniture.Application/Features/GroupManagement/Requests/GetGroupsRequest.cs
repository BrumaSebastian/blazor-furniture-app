using BlazorFurniture.Application.Common.Requests.QueryParams;
using FastEndpoints;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public sealed class GetGroupsRequest
{
    [FromQuery]
    public required PaginationQueryParam Pagination { get; set; }
}
