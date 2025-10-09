using BlazorFurniture.Core.Shared.Errors;
using System.Net;

namespace BlazorFurniture.Application.Common.Models;

public class HttpResult<TValue, TError>
    where TValue : class, new()
    where TError : class
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public TValue Value
    {
        get
        {
            if (IsSuccess && field is null)
                throw new InvalidOperationException("The value of a successful result cannot be null");

            if (IsFailure)
                throw new InvalidOperationException("Cannot access value of a failed result");

            return field;
        }
        init;
    } = null!;

    public TError Error
    {
        get
        {
            if (IsFailure && field is null)
                throw new InvalidOperationException("The value of a failed result cannot be null");
            if (IsSuccess)
                throw new InvalidOperationException("Cannot access error field for a succeded result");
            return field;
        }
        init;
    } = null!;

    public HttpStatusCode StatusCode { get; init; }

    private HttpResult( bool isSuccess, TValue? value, TError? error, HttpStatusCode statusCode )
    {
        IsSuccess = isSuccess;
        Value = value!;
        Error = error!;
        StatusCode = statusCode;
    }

    public static HttpResult<TValue, TError> Succeeded( TValue value, HttpStatusCode statusCode = HttpStatusCode.OK )
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(true, value, null, statusCode);
    }

    public static HttpResult<TValue, TError> Failed( TError error, HttpStatusCode statusCode = HttpStatusCode.BadRequest )
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(false, null, error, statusCode);
    }

    public static HttpResult<TValue, TError> NoContent( HttpStatusCode statusCode = HttpStatusCode.NoContent )
    {
        if (typeof(TValue) != typeof(EmptyResult))
            throw new InvalidOperationException("NoContent can only be used with TValue = EmptyResult.");

        return new(true, (TValue)((object)new EmptyResult()), null, statusCode);
    }

    public bool TryGetValue( out TValue value )
    {
        value = IsSuccess ? Value : null!;
        return IsSuccess;
    }

    public Result<TValue> ToDomainResult( IHttpErrorMapper errorMapper, Guid resourceId, Type? resourceType = null )
    {
        ArgumentNullException.ThrowIfNull(errorMapper);

        return IsSuccess
            ? Result<TValue>.Succeeded(Value)
            : Result<TValue>.Failed(errorMapper.MapFor(Error, StatusCode, resourceId, resourceType));
    }
}
