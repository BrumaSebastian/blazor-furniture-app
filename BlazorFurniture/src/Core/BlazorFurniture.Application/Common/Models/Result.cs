using BlazorFurniture.Core.Shared.Models.Errors;

namespace BlazorFurniture.Application.Common.Models;

public class Result<TValue> where TValue : class
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
    }

    public BasicError? Error
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
    }

    private Result( bool isSuccess, TValue? value, BasicError? error )
    {
        IsSuccess = isSuccess;
        Value = value!;
        Error = error;
    }

    public static Result<TValue> Succeeded( TValue value )
    {
        ArgumentNullException.ThrowIfNull(value);
        return new(true, value, null);
    }

    public static Result<EmptyResult> Succeded() => new(true, new(), null);

    public static Result<TValue> Failed( BasicError error )
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(false, null, error);
    }

    public static implicit operator Result<TValue>( TValue value ) => Succeeded(value);
    public static implicit operator Result<TValue>( BasicError error ) => Failed(error);

    public Result<TNew> PropagateFailure<TNew>() where TNew : class
    {
        if (IsFailure)
            return Result<TNew>.Failed(Error!);

        throw new InvalidOperationException("Cannot propagate success to a different result type.");
    }

    public TResult Match<TResult>(
        Func<TValue, TResult> success,
        Func<BasicError, TResult> failure )
    {
        ArgumentNullException.ThrowIfNull(success);
        ArgumentNullException.ThrowIfNull(failure);

        return IsSuccess ? success(Value!) : failure(Error!);
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<TValue, Task<TResult>> success,
        Func<BasicError, Task<TResult>> failure )
    {
        ArgumentNullException.ThrowIfNull(success);
        ArgumentNullException.ThrowIfNull(failure);

        return IsSuccess ? await success(Value!) : await failure(Error!);
    }

    public Result<TNew> Map<TNew>( Func<TValue, TNew> mapper ) where TNew : class
    {
        ArgumentNullException.ThrowIfNull(mapper);

        return IsSuccess
            ? Result<TNew>.Succeeded(mapper(Value!))
            : Result<TNew>.Failed(Error!);
    }

    public bool TryGetValue( out TValue value )
    {
        value = IsSuccess ? Value : null!;
        return IsSuccess;
    }
}
