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

        services.AddSwaggerGen(options =>
        {
            options.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

            options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{keycloakConfig.Authentication.ValidIssuer}{keycloakConfig.Endpoints.Authorization}"!),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "openid" },
                            { "profile", "profile" },
                        }
                    }
                }
            });

            var securityRequirement = new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Keycloak",
                            Type = ReferenceType.SecurityScheme
                        },
                        In = ParameterLocation.Header,
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                    },
                    []
                }
            };

            options.AddSecurityRequirement(securityRequirement);
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
