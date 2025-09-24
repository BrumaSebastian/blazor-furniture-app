using BlazorFurniture.Application.Common.Interfaces;

namespace BlazorFurniture.Application.Common.Dispatchers;

public interface IQueryDispatcher
{
    Task<TResult> DispatchQuery<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) 
        where TQuery : IQuery<TResult>;
}
