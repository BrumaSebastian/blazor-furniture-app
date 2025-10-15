namespace BlazorFurniture.Application.Common.Requests.QueryParams;

public class PaginationQueryParam
{
    public int Page
    {
        get;
        set => field = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get;
        set => field = value < 1 ? 10 : Math.Min(value, 100);
    }
}
