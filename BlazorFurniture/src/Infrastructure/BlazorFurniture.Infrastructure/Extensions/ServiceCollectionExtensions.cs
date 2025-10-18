using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Core.Shared.Utils.Extensions;
using BlazorFurniture.Infrastructure.Constants;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.External.Keycloak.Clients;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;
using BlazorFurniture.Infrastructure.Implementations.Features.Notifications;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace BlazorFurniture.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    extension( IServiceCollection services )
    {
        public IServiceCollection AddInfrastructureServices( IConfiguration configuration )
        {
            services.RegisterHandlers(Assembly.GetExecutingAssembly());
            services.AddKeycloak(configuration);
            services.AddErrorMappers();

            services.AddScoped<IRazorHtmlRenderer, RazorHtmlRenderer>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEmailNotificationService, EmailNotificationService>();
            services.AddKeyedSingleton<IResourceManager, EmailResourceManager>(KeyedServices.EMAIL_RESOURCE_MANAGER);

            return services;
        }

        public IServiceCollection AddKeycloak( IConfiguration configuration )
        {
            var keycloakConfig = configuration.GetSection(KeycloakConfiguration.Name).Get<KeycloakConfiguration>()
                ?? throw new Exception("Keycloak configuration is missing.");

            services.Configure<KeycloakConfiguration>(configuration.GetSection(KeycloakConfiguration.Name));
            services.AddSingleton(rg => rg.GetRequiredService<IOptions<KeycloakConfiguration>>().Value);
            services.AddScoped<EndpointsFactory>();
            services.AddScoped(sp => sp.GetRequiredService<EndpointsFactory>().Create());
            services.AddHttpClientWithBaseUrl<IUserManagementClient, UserManagementClient>(keycloakConfig.Url);
            services.AddHttpClientWithBaseUrl<IGroupManagementClient, GroupManagementClient>(keycloakConfig.Url);

            return services;
        }

        private IServiceCollection RegisterHandlers( Assembly assembly )
        {
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

                        if (genericTypeDefinition == commandWithResultHandlerType ||
                            genericTypeDefinition == queryHandlerType)
                        {
                            services.AddScoped(interfaceType, type);
                        }
                    }
                }
            }

            return services;
        }

        private IServiceCollection AddErrorMappers()
        {
            services.AddKeyedScoped<IHttpErrorMapper, KeycloakHttpErrorMapper>(KeyedServices.KEYCLOAK);

            return services;
        }
    }
}
