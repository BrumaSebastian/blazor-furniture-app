using BlazorFurniture.Application.Common.Requests.QueryParams;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests.Filters;

public sealed class GroupUsersQueryFilter : IPaginationQueryParams, ISearchQueryParam
{
    public int? Page { get; init; }
    public int? PageSize { get; init; }
    public string? Search { get; init; }
}
