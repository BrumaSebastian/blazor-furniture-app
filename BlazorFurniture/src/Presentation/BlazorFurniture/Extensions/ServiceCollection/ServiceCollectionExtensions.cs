using BlazorFurniture.Application.Common.Extensions;
using BlazorFurniture.Extensions.Handlers;
using BlazorFurniture.Infrastructure.Extensions;
using BlazorFurniture.Middlewares;
using BlazorFurniture.Shared.Extensions;
using BlazorFurniture.Shared.Services.API.Interfaces;

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
