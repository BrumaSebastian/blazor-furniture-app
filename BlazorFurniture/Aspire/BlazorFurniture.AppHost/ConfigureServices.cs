using BlazorFurniture.AppHost.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.AppHost;

internal static class ConfigureServices
{
    public static IServiceCollection AddKeycloak(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

        return services;
    }
}
