using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using Microsoft.Extensions.Logging;

namespace BlazorFurniture.Application.Common.Decorators;

[Obsolete("Not used. Will be removed or implemented later.", error: false)]
public class ValidationDispatcherDecorator(
    ICommandDispatcher commandDispatcher,
    IQueryDispatcher queryDispatcher,
    IServiceProvider serviceProvider,
    ILogger<ValidationDispatcherDecorator> logger ) : ICommandDispatcher, IQueryDispatcher
{
    private readonly ICommandDispatcher commandDispatcher = commandDispatcher;
    private readonly IQueryDispatcher queryDispatcher = queryDispatcher;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<ValidationDispatcherDecorator> _logger = logger;

    public async Task<TResult> Dispatch<TCommand, TResult>( TCommand command, CancellationToken ct = default )
        where TCommand : ICommand<TResult>
    {
        return await commandDispatcher.Dispatch<TCommand, TResult>(command, ct);
    }

    public async Task<Result<TResult>> DispatchQuery<TQuery, TResult>( TQuery query, CancellationToken ct = default )
        where TQuery : IQuery<TResult>
        where TResult : class
    {
        await ValidateAsync(query, ct);
        return await queryDispatcher.DispatchQuery<TQuery, TResult>(query, ct);
    }

    private Task ValidateAsync<T>( T request, CancellationToken ct )
    {
        return Task.CompletedTask;
        // Assumes FluentValidation is used
        //var validatorType = typeof(IValidator<>).MakeGenericType(request.GetType());

        //using var scope = _serviceProvider.CreateScope();
        //var validators = scope.ServiceProvider.GetServices(validatorType).ToList();

        //if (!validators.Any())
        //{
        //    _logger.LogDebug("No validators found for {RequestType}", typeof(T).Name);
        //    return;
        //}

        //_logger.LogDebug("Validating request {RequestType}", typeof(T).Name);

        //var context = new ValidationContext<T>(request);
        //var failures = new List<ValidationFailure>();

        //foreach (var validator in validators)
        //{
        //    var validationResult = await ((IValidator<T>)validator).ValidateAsync(context, cancellationToken);
        //    if (!validationResult.IsValid)
        //    {
        //        failures.AddRange(validationResult.Errors);
        //    }
        //}

        //if (failures.Any())
        //{
        //    _logger.LogWarning("Validation failed for {RequestType}: {Errors}", 
        //        typeof(T).Name, string.Join(", ", failures.Select(f => f.ErrorMessage)));

        //    throw new ValidationException(failures);
        //}
    }

    // Command dispatch methods with validation
}
