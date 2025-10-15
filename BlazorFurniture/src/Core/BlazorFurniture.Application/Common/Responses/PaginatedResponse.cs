namespace BlazorFurniture.Application.Common.Responses;

public sealed class PaginatedResponse<T> where T : class
{
    public int Total { get; set; }
    public required IEnumerable<T> Results { get; set; }
}
