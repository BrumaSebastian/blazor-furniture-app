namespace BlazorFurniture.Extensions.ServiceCollection;

public static class AuthorizationExtensions
{
    extension( IServiceCollection services )
    {
        public IServiceCollection AddAppAuthorization()
        {
            services.AddAuthorization();

            return services;
        }
    }
}
