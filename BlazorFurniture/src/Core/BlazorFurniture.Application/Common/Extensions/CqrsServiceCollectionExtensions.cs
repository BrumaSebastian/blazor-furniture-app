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
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        // Base services
        services.AddScoped<Dispatcher>();

        // Apply decorators in the right order
        services.AddScoped<ICommandDispatcher>(sp =>
        {
            var dispatcher = sp.GetRequiredService<Dispatcher>();
            return dispatcher;
        });
        //services.AddScoped<ICommandDispatcher>(sp =>
        //{
        //    var dispatcher = sp.GetRequiredService<Dispatcher>();
        //    return new ValidationDispatcherDecorator(
        //        dispatcher,
        //        dispatcher,
        //        sp,
        //        sp.GetRequiredService<ILogger<ValidationDispatcherDecorator>>());
        //});

        services.AddScoped<IQueryDispatcher>(sp => {
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
        RegisterHandlers(services, assembly);

        return services;
    }


    // Helper method to register handlers
    static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        // Register command handlers
        var commandHandlerType = typeof(ICommandHandler<>);
        var commandWithResultHandlerType = typeof(ICommandHandler<,>);
        var queryHandlerType = typeof(IQueryHandler<,>);

        // Find and register all handlers
        var types = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .ToList();

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces();

            foreach (var interfaceType in interfaces)
            {
                if (interfaceType.IsGenericType)
                {
                    var genericTypeDefinition = interfaceType.GetGenericTypeDefinition();

                    if (genericTypeDefinition == commandHandlerType ||
                        genericTypeDefinition == commandWithResultHandlerType ||
                        genericTypeDefinition == queryHandlerType)
                    {
                        services.AddScoped(interfaceType, type);
                    }
                }
            }
        }
    }
}