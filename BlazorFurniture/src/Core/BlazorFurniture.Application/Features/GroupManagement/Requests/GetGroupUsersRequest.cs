using BlazorFurniture.Application.Common.Requests.QueryParams;
using FastEndpoints;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public class GetGroupUsersRequest : GetGroupRequest, IPaginationQueryParams
{
    public int? Page { get; init; } = 0;
    public int? PageSize { get; init; } = 10;
    [QueryParam]
    public string? Search { get; set; }
}
