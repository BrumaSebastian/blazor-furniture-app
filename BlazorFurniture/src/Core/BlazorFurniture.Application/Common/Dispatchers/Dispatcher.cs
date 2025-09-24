using BlazorFurniture.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazorFurniture.Application.Common.Dispatchers;

public class Dispatcher(IServiceProvider serviceProvider, ILogger<Dispatcher> logger) : ICommandDispatcher, IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<Dispatcher> _logger = logger;

    public async Task Dispatch<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        await handler.HandleAsync(command, cancellationToken);
    }

    public async Task<TResult> Dispatch<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
    {
        using var scope = _serviceProvider.CreateScope();
        var handler = _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return await handler.HandleAsync(command, cancellationToken);
    }

    public async Task<TResult> DispatchQuery<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) 
        where TQuery : IQuery<TResult>
    {
        try
        {
            _logger.LogDebug("Dispatching query {QueryType}", typeof(TQuery).Name);

            using var scope = _serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            var result = await handler.HandleAsync(query, cancellationToken);

            _logger.LogDebug("Query {QueryType} handled successfully", typeof(TQuery).Name);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling query {QueryType}: {ErrorMessage}", typeof(TQuery).Name, ex.Message);
            throw;
        }
    }
}
