using BlazorFurniture.Application.Common.Extensions;
using BlazorFurniture.Client.Services;
using BlazorFurniture.Client.Services.Interfaces;
using BlazorFurniture.Extensions.Handlers;
using BlazorFurniture.Infrastructure.Extensions;
using BlazorFurniture.Middlewares;
using BlazorFurniture.Shared.Extensions;
using BlazorFurniture.Shared.Security.Authorization;
using BlazorFurniture.Shared.Services.API.Interfaces;
using BlazorFurniture.Shared.Services.Security;
using BlazorFurniture.Shared.Services.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BlazorFurniture.Extensions.ServiceCollection;

public static class ServiceCollectionExtensions
{
    extension( IServiceCollection services )
    {
        public IServiceCollection AddAppServices( IConfiguration configuration )
        {
            services
                .AddAppConfigurations(configuration)
                .AddAppOptions()
                .AddTransient<GlobalExceptionHandlerMiddleware>()
                .AddApplicationServices(configuration)
                .AddInfrastructureServices(configuration)
                .AddAppAuthentication(configuration)
                .AddAppAuthorization();

            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = ctx =>
                {
                    // TODO customization
                };
            });

            return services;
        }

        public IServiceCollection AddRefitServerApis()
        {
            services.AddApiClient<IUserApi>()
                .ConfigureHttpClient(ConfigureServerBaseAddressHttpClient())
                .AddHttpMessageHandler<ForwardAuthHeaderHandler>();

            services.AddApiClient<IGroupsApi>()
                .ConfigureHttpClient(ConfigureServerBaseAddressHttpClient())
                .AddHttpMessageHandler<ForwardAuthHeaderHandler>();

            return services;
        }

        public IServiceCollection AddServerSide()
        {
            services.AddRefitServerApis();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddServerSideServices();
            services.AddCascadingAuthenticationState();

            return services;
        }

        public IServiceCollection AddServerSideServices()
        {
            services.AddScoped<IPermissionsService, PermissionsService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IBreadCrumbsService, BreadcrumbsService>();

            return services;
        }
    }

    private static Action<IServiceProvider, HttpClient> ConfigureServerBaseAddressHttpClient()
    {
        return ( serviceProvider, c ) =>
        {
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

            if (httpContextAccessor is not null)
            {
                var request = httpContextAccessor.HttpContext?.Request;
                c.BaseAddress = new Uri($"{request?.Scheme}://{request?.Host}");
            }
        };
    }
}
