using FastEndpoints;

namespace BlazorFurniture.Application.Common.Requests.QueryParams;

public interface ISearchQueryParam
{
    [QueryParam]
    string? Search { get; init; }
}
