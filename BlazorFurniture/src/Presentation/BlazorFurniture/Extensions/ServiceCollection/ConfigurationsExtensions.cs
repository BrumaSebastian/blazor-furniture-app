using BlazorFurniture.Core.Shared.Configurations;
using BlazorFurniture.Extensions.ServiceCollection;
using Microsoft.Extensions.Options;

namespace BlazorFurniture.Extensions.ServiceCollection;

public static class ConfigurationsExtensions
{
    extension( IServiceCollection services )
    {
        public IServiceCollection AddAppConfigurations( IConfiguration configuration )
        {
            services.Configure<OpenIdConectOptions>(configuration.GetSection(OpenIdConectOptions.NAME));

            return services;
        }

        public IServiceCollection AddAppOptions()
        {
            services.AddSingleton(registeredServices => registeredServices.GetRequiredService<IOptions<OpenIdConectOptions>>().Value);

            return services;
        }
    }
}
