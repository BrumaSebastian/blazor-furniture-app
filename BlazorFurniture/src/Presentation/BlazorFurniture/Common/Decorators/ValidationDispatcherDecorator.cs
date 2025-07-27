using BlazorFurniture.Common.Abstractions;
using BlazorFurniture.Common.Dispatchers;

public class ValidationDispatcherDecorator(
    ICommandDispatcher commandDispatcher, 
    IQueryDispatcher queryDispatcher,
    IServiceProvider serviceProvider,
    ILogger<ValidationDispatcherDecorator> logger) : /*ICommandDispatcher,*/ IQueryDispatcher
{
    private readonly ICommandDispatcher _commandDispatcher = commandDispatcher;
    private readonly IQueryDispatcher _queryDispatcher = queryDispatcher;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<ValidationDispatcherDecorator> _logger = logger;

    public async Task<TResult> DispatchQueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) 
        where TQuery : IQuery<TResult>
    {
        await ValidateAsync(query, cancellationToken);
        return await _queryDispatcher.DispatchQueryAsync<TQuery, TResult>(query, cancellationToken);
    }

    private async Task ValidateAsync<T>(T request, CancellationToken cancellationToken)
    {
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