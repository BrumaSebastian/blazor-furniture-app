namespace BlazorFurniture.Application.Common.Requests.QueryParams;

public interface IPaginationQueryParams
{
    int Page { get; init; }
    int PageSize { get; init; }
}
