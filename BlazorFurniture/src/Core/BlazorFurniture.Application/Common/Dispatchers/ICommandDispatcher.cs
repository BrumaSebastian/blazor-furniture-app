using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;

namespace BlazorFurniture.Application.Common.Dispatchers;

public interface ICommandDispatcher
{
    Task<Result<TResult>> DispatchCommand<TCommand, TResult>( TCommand command, CancellationToken ct = default )
        where TCommand : ICommand<TResult>
        where TResult : class;
}
