using BlazorFurniture.Common.Abstractions;

namespace BlazorFurniture.Common.Dispatchers;

public interface IQueryDispatcher
{
    Task<TResult> DispatchQueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) 
        where TQuery : IQuery<TResult>;
}