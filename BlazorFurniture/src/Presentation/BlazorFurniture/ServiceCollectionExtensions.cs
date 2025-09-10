using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace BlazorFurniture;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
        {
            options.Authority = configuration["OpenIDConnectSettings:Authority"];
            options.ClientId = configuration["OpenIDConnectSettings:ClientId"];
            options.ClientSecret = configuration["OpenIDConnectSettings:ClientSecret"];
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.UsePkce = true;
            options.SaveTokens = true;
            options.RequireHttpsMetadata = false;
            // Configure the scopes you need
            options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
            options.Scope.Add(OpenIdConnectScope.Email);
            //options.Scope.Add(OpenIdConnectScope.OfflineAccess);
            // Configure the token validation parameters
            options.MapInboundClaims = false;
            options.TokenValidationParameters.NameClaimType = "name";
            options.TokenValidationParameters.RoleClaimType = "role";
            // Configure the OpenID Connect events if needed
            options.Events.OnRemoteFailure = context =>
            {
                context.HandleResponse();
                //context.Response.Redirect("/Error?message=" + context.Failure.Message);
                return Task.CompletedTask;
            };
        });

        //services.ConfigureCookieOidc(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);

        return services;
    }

    public static IServiceCollection AddAppAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();

        return services;
    }
}
