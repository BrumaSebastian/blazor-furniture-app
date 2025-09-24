using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Application.Common.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services )
    {
        public IServiceCollection AddApplicationServices()
        {
            services.AddCqrs();

            return services;
        }
    }
}
