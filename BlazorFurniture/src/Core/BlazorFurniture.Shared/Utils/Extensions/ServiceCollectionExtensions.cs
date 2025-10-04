using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Core.Shared.Utils.Extensions;

public static class ServiceCollectionExtensions
{
    extension ( IServiceCollection services )
    {
        public IServiceCollection AddHttpClientWithBaseUrl<TInterface, TImplementation>( string baseUrl )
                where TInterface : class
                where TImplementation : class, TInterface
        {
            services.AddHttpClient<TInterface, TImplementation>(client =>
            {
                client.BaseAddress = new Uri(baseUrl);
            });

            return services;
        }
    }
}
