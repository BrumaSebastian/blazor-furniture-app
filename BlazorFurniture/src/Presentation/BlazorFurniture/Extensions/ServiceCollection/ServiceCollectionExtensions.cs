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
                .AddInfrastructureServices()
                .AddAppAuthentication(configuration)
                .AddAppAuthorization();

            return services;
        }
    }
}
