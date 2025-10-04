using BlazorFurniture.Application.Common.Extensions;
using BlazorFurniture.Infrastructure.Extensions;

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
                .AddApplicationServices()
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
