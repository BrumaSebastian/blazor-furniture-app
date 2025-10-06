using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Core.Shared.Models.Errors;
using System.Net;

namespace BlazorFurniture.Application.Common.Extensions;

public static class HttpResultExtensions
{
    public static Result<TValue> ToResult<TValue, TError>(
        this HttpResult<TValue, TError> http,
        Func<HttpStatusCode, TError, BasicError> onFailure )
        where TValue : class
        where TError : class
        => http.Match(onFailure);

    public static async Task<Result<TValue>> ToResult<TValue, TError>(
        this Task<HttpResult<TValue, TError>> httpTask,
        Func<HttpStatusCode, TError, BasicError> onFailure )
        where TValue : class
        where TError : class
        => (await httpTask).ToResult(onFailure);
}
