using BlazorFurniture.Application.Common.Extensions;
using BlazorFurniture.Infrastructure.Extensions;
using BlazorFurniture.Middlewares;

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
    }
}
