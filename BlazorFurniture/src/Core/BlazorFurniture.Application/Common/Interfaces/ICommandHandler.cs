using BlazorFurniture.Application.Common.Models;

namespace BlazorFurniture.Application.Common.Interfaces;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
    where TResult : class
{
    Task<Result<TResult>> HandleAsync( TCommand command, CancellationToken ct = default );
}
