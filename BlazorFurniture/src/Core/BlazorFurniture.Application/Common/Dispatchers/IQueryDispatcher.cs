using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;

namespace BlazorFurniture.Application.Common.Dispatchers;

public interface IQueryDispatcher
{
    Task<Result<TResult>> DispatchQuery<TQuery, TResult>(TQuery query, CancellationToken ct = default) 
        where TQuery : IQuery<TResult>
        where TResult : class;
}
