using BlazorFurniture.Application.Common.Interfaces;

namespace BlazorFurniture.Application.Common.Dispatchers;

public interface ICommandDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) 
        where TCommand : ICommand;
        
    Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default) 
        where TCommand : ICommand<TResult>;
}