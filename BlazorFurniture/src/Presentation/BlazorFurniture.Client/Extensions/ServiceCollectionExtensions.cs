using BlazorFurniture.Client.Services.API;
using BlazorFurniture.Shared.Extensions;
using BlazorFurniture.Shared.Services.API.Interfaces;

namespace BlazorFurniture.Client.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services )
    {
        public IServiceCollection AddServerApis( string baseAddress )
        {
            services.AddApiClient<IUsersClient>(baseAddress)
                .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddApiClient<IGroupsClient>(baseAddress)
                .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            return services;
        }
    }
}
