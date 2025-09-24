using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace BlazorFurniture.Application.Common.Decorators;

public class ValidationDispatcherDecorator(
    ICommandDispatcher commandDispatcher,
    IQueryDispatcher queryDispatcher,
    IServiceProvider serviceProvider,
    ILogger<ValidationDispatcherDecorator> logger ) : ICommandDispatcher, IQueryDispatcher
{
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher = queryDispatcher;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<ValidationDispatcherDecorator> _logger = logger;

    public Task Dispatch<TCommand>( TCommand command, CancellationToken cancellationToken = default ) where TCommand : ICommand
    {
        throw new NotImplementedException();
    }

    public Task<TResult> Dispatch<TCommand, TResult>( TCommand command, CancellationToken cancellationToken = default ) where TCommand : ICommand<TResult>
    {
        throw new NotImplementedException();
    }

    public async Task<TResult> DispatchQuery<TQuery, TResult>( TQuery query, CancellationToken cancellationToken = default )
        where TQuery : IQuery<TResult>
    {
        await ValidateAsync( query, cancellationToken );
        return await _queryDispatcher.DispatchQuery<TQuery, TResult>( query, cancellationToken );
    }

    private Task ValidateAsync<T>( T request, CancellationToken cancellationToken )
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
