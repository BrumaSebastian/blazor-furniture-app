﻿using BlazorFurniture.Client.Services.API;
using BlazorFurniture.Shared.Extensions;
using BlazorFurniture.Shared.Services.API;

namespace BlazorFurniture.Client.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services )
    {
        public IServiceCollection AddServerApis( string baseAddress )
        {
            services.AddApiClient<IUserApi>(baseAddress)
                .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            services.AddApiClient<IGroupsApi>(baseAddress)
                .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

            return services;
        }
    }
}
