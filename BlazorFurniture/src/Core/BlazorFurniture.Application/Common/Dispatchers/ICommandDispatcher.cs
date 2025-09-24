using BlazorFurniture.Application.Common.Interfaces;

namespace BlazorFurniture.Application.Common.Dispatchers;

public interface ICommandDispatcher
{
    Task Dispatch<TCommand>(TCommand command, CancellationToken cancellationToken = default) 
        where TCommand : ICommand;
        
    Task<TResult> Dispatch<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default) 
        where TCommand : ICommand<TResult>;
}