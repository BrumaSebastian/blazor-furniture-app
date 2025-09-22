using BlazorFurniture.Core.Shared.Configurations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace BlazorFurniture.Extensions.ServiceCollection;

public static class AuthenticationExtensions
{
    extension( IServiceCollection services )
    {
        public IServiceCollection AddAppAuthentication( IConfiguration configuration )
        {
            var openIdConnectOptions = configuration.GetSection(OpenIdConectOptions.NAME).Get<OpenIdConectOptions>()
                ?? throw new Exception($"Missing {nameof(OpenIdConectOptions)} settings");

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = openIdConnectOptions.Authority;
                options.ClientId = openIdConnectOptions.PublicClient.ClientId;
                //options.ClientSecret = configuration["OpenIDConnectSettings:ClientSecret"];
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
    }
}
