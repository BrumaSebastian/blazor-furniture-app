using BlazorFurniture.Application.Common.Interfaces;

namespace BlazorFurniture.Application.Common.Dispatchers;

public interface IQueryDispatcher
{
    Task<TResult> DispatchQueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) 
        where TQuery : IQuery<TResult>;
}