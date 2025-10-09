using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Core.Shared.Errors;

namespace BlazorFurniture.Application.Common.Extensions;

public static class HttpResultExtensions
{
    public static async Task<Result<TValue>> ToDomainResult<TValue, TError>(
        this Task<HttpResult<TValue, TError>> httpTask,
        IHttpErrorMapper errorMapper,
        Guid resourceId )
        where TValue : class, new()
        where TError : class
        => (await httpTask).ToDomainResult(errorMapper, resourceId, typeof(TValue));
}
