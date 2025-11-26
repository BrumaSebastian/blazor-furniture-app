using BlazorFurniture.Application.Common.Requests.QueryParams;

namespace BlazorFurniture.Application.Features.UserManagement.Requests.Filters;

public sealed class UsersQueryFilters : IPaginationQueryParams
{
    public int? Page { get; init; }
    public int? PageSize { get; init; }
    public string? Search { get; init; }
}
