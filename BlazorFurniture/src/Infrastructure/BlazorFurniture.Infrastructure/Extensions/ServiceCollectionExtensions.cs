using BlazorFurniture.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BlazorFurniture.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services )
    {
        public IServiceCollection AddInfrastructureServices()
        {
            services.RegisterHandlers(Assembly.GetExecutingAssembly());

            return services;
        }

        private IServiceCollection RegisterHandlers( Assembly assembly )
        {
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

            return services;
        }
    }
}
