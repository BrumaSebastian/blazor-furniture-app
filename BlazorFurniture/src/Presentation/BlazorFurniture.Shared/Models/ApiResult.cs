using System.Net;

namespace BlazorFurniture.Shared.Models;

public sealed class ApiResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public HttpStatusCode HttpStatusCode { get; set; }
}
