using BlazorFurniture.Application.Common.Requests.QueryParams;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests.Filters;

public record class GroupQueryFilters : IPaginationQueryParams
{
    public int? Page { get; init; } = 0;
    public int? PageSize { get; init; } = 10;
    public string? Name { get; set; }
}
