using FastEndpoints;

namespace BlazorFurniture.Application.Common.Requests.QueryParams;

public interface IPaginationQueryParams
{
    [QueryParam]
    int? Page { get; init; }
    [QueryParam]
    int? PageSize { get; init; }
}
