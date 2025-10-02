using BlazorFurniture.Core.Shared.Configurations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace BlazorFurniture.Extensions.ServiceCollection;

public static class AuthenticationExtensions
{
    extension( IServiceCollection services )
    {
        public IServiceCollection AddAppAuthentication( IConfiguration configuration )
        {
            var openIdConnectOptions = configuration.GetSection(OpenIdConnectConfigOptions.NAME).Get<OpenIdConnectConfigOptions>()
                ?? throw new Exception($"Missing {nameof(OpenIdConnectConfigOptions)} settings");

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = openIdConnectOptions.Authority;
                options.ClientId = openIdConnectOptions.PublicClient.ClientId;
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
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = openIdConnectOptions.Authority;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = openIdConnectOptions.Authority,
                    ValidateAudience = false,
                    ValidAudiences =
                    [
                        openIdConnectOptions.DevPublicClient?.ClientId,
                        openIdConnectOptions.PublicClient?.ClientId
                    ],
                };

                options.MapInboundClaims = false;

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        return Task.CompletedTask;
                    },
                    OnChallenge = ctx =>
                    {
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = ctx =>
                    {
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = ctx =>
                    {
                        return Task.CompletedTask;
                    },
                    OnForbidden = ctx =>
                    {
                        return Task.CompletedTask;
                    }
                };
            });

            //services.ConfigureCookieOidc(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);

            return services;
        }
    }
}
