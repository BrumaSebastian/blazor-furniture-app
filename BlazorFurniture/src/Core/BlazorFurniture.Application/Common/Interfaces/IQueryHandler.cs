using BlazorFurniture.Application.Common.Models;

namespace BlazorFurniture.Application.Common.Interfaces;

public interface IQueryHandler<in TQuery, TResult> 
    where TQuery : IQuery<TResult>
    where TResult : class
{
    Task<Result<TResult>> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
}
