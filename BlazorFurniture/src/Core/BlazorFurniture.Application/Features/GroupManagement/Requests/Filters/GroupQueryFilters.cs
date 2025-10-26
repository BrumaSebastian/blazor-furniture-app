using BlazorFurniture.Application.Common.Requests.QueryParams;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests.Filters;

public record class GroupQueryFilters : IPaginationQueryParams
{
    public int? Page { get; init; }
    public int? PageSize { get; init; }
    public string? Search { get; init; }
}
