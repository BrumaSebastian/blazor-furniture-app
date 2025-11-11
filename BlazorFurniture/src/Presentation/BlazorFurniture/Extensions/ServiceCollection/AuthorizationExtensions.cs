using BlazorFurniture.Controllers.Authorization.Handlers;
using BlazorFurniture.Controllers.Authorization.Policies;
using Microsoft.AspNetCore.Authorization;

namespace BlazorFurniture.Extensions.ServiceCollection;

public static class AuthorizationExtensions
{
    extension( IServiceCollection services )
    {
        public IServiceCollection AddAppAuthorization()
        {
            services.AddAuthorization(options =>
            {
                options.RegisterGroupPolicies();
            });

            services.AddScoped<IAuthorizationHandler, UmaAuthorizationHandler>();

            return services;
        }
    }
}
