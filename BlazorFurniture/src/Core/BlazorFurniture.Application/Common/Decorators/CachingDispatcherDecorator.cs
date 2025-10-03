using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace BlazorFurniture.Application.Common.Decorators;

[Obsolete("Not used. Will be removed or implemented later.", error: false)]
public class CachingDispatcherDecorator(
    IQueryDispatcher queryDispatcher,
    IMemoryCache cache,
    ILogger<CachingDispatcherDecorator> logger ) : IQueryDispatcher
{
    private readonly IQueryDispatcher _queryDispatcher = queryDispatcher;
    private readonly IMemoryCache _cache = cache;
    private readonly ILogger<CachingDispatcherDecorator> _logger = logger;

    public async Task<Result<TResult>> DispatchQuery<TQuery, TResult>( TQuery query, CancellationToken ct = default )
        where TQuery : IQuery<TResult>
        where TResult : class
    {
        // Check if query is cacheable
        var queryCacheAttribute = typeof( TQuery ).GetCustomAttribute<CacheableQueryAttribute>();

        if (queryCacheAttribute is null)
        {
            return await _queryDispatcher.DispatchQuery<TQuery, TResult>( query, ct );
        }

        // Generate cache key
        var cacheKey = $"Query:{typeof( TQuery ).Name}:{query.GetHashCode()}";

        // Try get from cache
        if (_cache.TryGetValue(cacheKey, out TResult? cachedResult))
        {
            _logger.LogDebug( "Cache hit for query {QueryType}", typeof( TQuery ).Name );
            return cachedResult!;
        }

        // Get from dispatcher
        _logger.LogDebug( "Cache miss for query {QueryType}", typeof( TQuery ).Name );
        var result = await _queryDispatcher.DispatchQuery<TQuery, TResult>( query, ct );

        // Cache the result
        _cache.Set( cacheKey, result,
            new MemoryCacheEntryOptions().SetAbsoluteExpiration(
                TimeSpan.FromSeconds( queryCacheAttribute.DurationInSeconds ) ) );

        return result;
    }
}

[AttributeUsage( AttributeTargets.Class )]
public class CacheableQueryAttribute( int durationInSeconds = 60 ) : Attribute
{
    public int DurationInSeconds { get; } = durationInSeconds;
}
