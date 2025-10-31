using BlazorFurniture.Authentication;
using BlazorFurniture.Constants;
using BlazorFurniture.Core.Shared.Configurations;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;

namespace BlazorFurniture.Extensions.ServiceCollection;

public static class AuthenticationExtensions
{
    private static readonly string CookieOrBearer = "CookieOrBearer";

    extension( IServiceCollection services )
    {
        public IServiceCollection AddAppAuthentication( IConfiguration configuration )
        {
            var openIdConnectOptions = configuration.GetSection(OpenIdConnectConfigOptions.NAME).Get<OpenIdConnectConfigOptions>()
                ?? throw new Exception($"Missing {nameof(OpenIdConnectConfigOptions)} settings");

            services
            .AddAuthenticationCookie(validFor: TimeSpan.FromHours(1), options =>
            {
                options.SlidingExpiration = true;
            })
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieOrBearer;
                options.DefaultAuthenticateScheme = CookieOrBearer;
                options.DefaultChallengeScheme = CookieOrBearer;
            })
            .AddPolicyScheme(CookieOrBearer, nameof(CookieOrBearer), options =>
            {
                options.ForwardDefaultSelector = ctx =>
                {
                    var isBearerAuth = ctx.Request.Headers.Authorization
                    .Any(v => AuthenticationHeaderValue.TryParse(v, out var header)
                        && header.Scheme.Equals(JwtBearerDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase));
                    return isBearerAuth ? JwtBearerDefaults.AuthenticationScheme : CookieAuthenticationDefaults.AuthenticationScheme;
                };
            })
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = openIdConnectOptions.Authority;
                options.ClientId = openIdConnectOptions.PublicClient.ClientId;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.UsePkce = true;
                options.SaveTokens = true;
                options.RequireHttpsMetadata = false;
                options.Scope.Add(OpenIdConnectScope.OpenIdProfile);
                options.Scope.Add(OpenIdConnectScope.Email);
                options.Scope.Add(OpenIdConnectScope.OfflineAccess);
                options.MapInboundClaims = false;
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "role";
                options.Events ??= new OpenIdConnectEvents();
                options.Events.OnRedirectToIdentityProvider = context =>
                {
                    if (context.Properties?.Items != null &&
                        context.Properties.Items.TryGetValue(Oidc.Actions.KEYCLOAK_ACTIONS, out var action) &&
                        !string.IsNullOrWhiteSpace(action))
                    {
                        context.ProtocolMessage.SetParameter(Oidc.Actions.KEYCLOAK_ACTIONS, action);
                    }
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
            });

            services.ConfigureCookieOidc(CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);

            return services;
        }
    }
}
