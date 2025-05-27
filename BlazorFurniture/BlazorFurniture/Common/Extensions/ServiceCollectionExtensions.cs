using BlazorFurniture.Modules.Keycloak.Clients;
using BlazorFurniture.Modules.Keycloak.Extensions;
using BlazorFurniture.Modules.Keycloak.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace BlazorFurniture.Common.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
    {
        KeycloakConfiguration keycloakConfig = configuration.GetSection("Keycloak").Get<KeycloakConfiguration>() ??
            throw new Exception("Keycloak configuration is not configured");

        var supportedRealms = new[] { "master", "test-realm" };

        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            // Add a security definition for each realm
            foreach (var realm in supportedRealms)
            {
                var securitySchemeName = $"Keycloak-{realm}";

                // Build the authorization URL with the specific realm
                var auth = $"{keycloakConfig.LocalUrl ?? keycloakConfig.BaseUrl}/realms/master/protocol/openid-connect/auth";
                var authUrl = auth.Replace("/realms/master", $"/realms/{realm}");

                options.AddSecurityDefinition(securitySchemeName, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = $"Keycloak authentication for {realm} realm",
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(authUrl),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "openid" },
                                { "profile", "profile" },
                                { "organization", "organization" },

                            }
                        }
                    }
                });

                // Add security requirement for this realm
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = securitySchemeName,
                                Type = ReferenceType.SecurityScheme
                            },
                            Scheme = JwtBearerDefaults.AuthenticationScheme,
                            Name = JwtBearerDefaults.AuthenticationScheme,
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            }
        });

        return services;
    }

    internal static IServiceCollection AddKeycloakServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KeycloakConfiguration>(configuration.GetSection("Keycloak"));

        KeycloakConfiguration keycloakConfig = configuration.GetSection("Keycloak").Get<KeycloakConfiguration>() ??
            throw new Exception("Keycloak configuration is not configured");

        services.AddTransient<IClaimsTransformation, KeycloakRoleClaimsTransformer>();

        services.AddHttpClient<KeycloakHttpClient>(client =>
        {
            client.BaseAddress = new Uri(keycloakConfig.BaseUrl);
        });

        services.AddScoped<IKeycloakService, KeycloakService>();
        services.AddScoped<IKeycloakRealmService, KeycloakRealmService>();
        services.AddHttpContextAccessor();
        services.AddScoped<KeycloakEndpointsFactory>();
        services.AddScoped(sp => sp.GetRequiredService<KeycloakEndpointsFactory>().CreateEndpoints());

        return services;
    }

    internal static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        KeycloakConfiguration keycloakConfig = configuration.GetSection("Keycloak").Get<KeycloakConfiguration>() ??
            throw new Exception("Keycloak configuration is not configured");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.Audience = keycloakConfig.Authentication.Audience;
                options.MetadataAddress = keycloakConfig.Authentication.MetadataAddress;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuer = keycloakConfig.Authentication.ValidIssuer,
                    RoleClaimType = ClaimTypes.Role
                };
            });

        return services;
    }
}
