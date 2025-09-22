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
                .AddAppAuthentication(configuration)
                .AddAppAuthorization();

            return services;
        }
    }
}
