namespace BlazorFurniture.Shared.Models;

public sealed class PaginatedModel<T> where T : class
{
    public int Total { get; set; }
    public required IEnumerable<T> Results { get; set; }
}
