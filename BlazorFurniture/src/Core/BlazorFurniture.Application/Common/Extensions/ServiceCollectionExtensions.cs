using BlazorFurniture.Application.Common.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BlazorFurniture.Application.Common.Extensions;

public static class ServiceCollectionExtensions
{
    extension( IServiceCollection services )
    {
        public IServiceCollection AddApplicationServices( IConfiguration configuration )
        {
            services.AddCqrs();
            services.AddOptions(configuration);

            return services;
        }

        public IServiceCollection AddOptions( IConfiguration configuration )
        {
            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.NAME));
            services.AddSingleton(registeredServices => registeredServices.GetRequiredService<IOptions<EmailOptions>>().Value);

            return services;
        }
    }
}
