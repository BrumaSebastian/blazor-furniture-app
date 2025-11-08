using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Application.Common.Dispatchers;

public class Dispatcher( IServiceProvider serviceProvider ) : ICommandDispatcher, IQueryDispatcher
{
    public async Task<Result<TResult>> DispatchCommand<TCommand, TResult>( TCommand command, CancellationToken ct = default )
        where TCommand : ICommand<TResult>
        where TResult : class
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResult>>();

        return await handler.HandleAsync(command, ct);
    }

    public async Task<Result<TResult>> DispatchQuery<TQuery, TResult>( TQuery query, CancellationToken ct = default )
        where TQuery : IQuery<TResult>
        where TResult : class
    {
        var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>();

        return await handler.HandleAsync(query, ct);
    }
}
