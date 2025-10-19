using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazorFurniture.Application.Common.Dispatchers;

public class Dispatcher( IServiceProvider serviceProvider, ILogger<Dispatcher> logger ) : ICommandDispatcher, IQueryDispatcher
{
    public async Task<TResult> Dispatch<TCommand, TResult>( TCommand command, CancellationToken ct = default )
        where TCommand : ICommand<TResult>
    {
        using var scope = serviceProvider.CreateScope();
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();
        return await handler.HandleAsync(command, ct);
    }

    public async Task<Result<TResult>> DispatchQuery<TQuery, TResult>( TQuery query, CancellationToken ct = default )
        where TQuery : IQuery<TResult>
        where TResult : class
    {
        try
        {
            logger.LogDebug("Dispatching query {QueryType}", typeof(TQuery).Name);

            using var scope = serviceProvider.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();
            var result = await handler.HandleAsync(query, ct);

            logger.LogDebug("Query {QueryType} handled successfully", typeof(TQuery).Name);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling query {QueryType}: {ErrorMessage}", typeof(TQuery).Name, ex.Message);
            throw;
        }
    }
}
