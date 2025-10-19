using BlazorFurniture.Application.Common.Interfaces;

namespace BlazorFurniture.Application.Common.Dispatchers;

public interface ICommandDispatcher
{
    Task<TResult> Dispatch<TCommand, TResult>( TCommand command, CancellationToken ct = default )
        where TCommand : ICommand<TResult>;
}
