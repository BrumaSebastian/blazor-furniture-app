using BlazorFurniture.Application.Common.Decorators;
using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace BlazorFurniture.Application.Common.Extensions;

public static class CqrsServiceCollectionExtensions
{
    extension( IServiceCollection services )
    {
        public IServiceCollection AddCqrs()
        {
            // Base services
            services.AddScoped<Dispatcher>();

            // Apply decorators in the right order
            services.AddScoped<ICommandDispatcher>(sp =>
            {
                var dispatcher = sp.GetRequiredService<Dispatcher>();
                return dispatcher;
            });

            services.AddScoped<ICommandDispatcher>(sp =>
            {
                var dispatcher = sp.GetRequiredService<Dispatcher>();
                return new ValidationDispatcherDecorator(
                    dispatcher,
                    dispatcher,
                    sp,
                    sp.GetRequiredService<ILogger<ValidationDispatcherDecorator>>());
            });

            services.AddScoped<IQueryDispatcher>(sp =>
            {
                var dispatcher = sp.GetRequiredService<Dispatcher>();
                var validatedDispatcher = new ValidationDispatcherDecorator(
                    dispatcher,
                    dispatcher,
                    sp,
                    sp.GetRequiredService<ILogger<ValidationDispatcherDecorator>>());

                return new CachingDispatcherDecorator(
                    validatedDispatcher,
                    sp.GetRequiredService<IMemoryCache>(),
                    sp.GetRequiredService<ILogger<CachingDispatcherDecorator>>());
            });

            var assembly = typeof(CqrsServiceCollectionExtensions).Assembly;

            return services;
        }
    }
}
