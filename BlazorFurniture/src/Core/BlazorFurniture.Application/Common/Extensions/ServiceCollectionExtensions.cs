//using BlazorFurniture.Application.Keycloak.Services;
//using Microsoft.Extensions.DependencyInjection;
//using System.Security.Claims;

//namespace BlazorFurniture.Application.Common.Extensions;

//internal static class ServiceCollectionExtensions
//{
//    internal static IServiceCollection AddSwaggerGenWithAuth(this IServiceCollection services, IConfiguration configuration)
//    {
//        KeycloakConfiguration keycloakConfig = configuration.GetSection("Keycloak").Get<KeycloakConfiguration>() ??
//            throw new Exception("Keycloak configuration is not configured");

//        var supportedRealms = new[] { "master", "test-realm", "Development" };

//        services.AddSwaggerGen(options =>
//        {
//            options.CustomSchemaIds(id => id.FullName!.Replace('+', '-'));

//            // Add a security definition for each realm
//            foreach (var realm in supportedRealms)
//            {
//                var securitySchemeName = $"Keycloak-{realm}";

//                // Build the authorization URL with the specific realm
//                var authUrl = $"{keycloakConfig.LocalUrl ?? keycloakConfig.BaseUrl}/realms/{realm}/protocol/openid-connect/auth";
//                var tokenUrl = $"{keycloakConfig.LocalUrl ?? keycloakConfig.BaseUrl}/realms/{realm}/protocol/openid-connect/token";

//                options.AddSecurityDefinition(securitySchemeName, new OpenApiSecurityScheme
//                {
//                    Type = SecuritySchemeType.OAuth2,
//                    Description = $"Keycloak authentication for {realm} realm",
//                    Flows = new OpenApiOAuthFlows
//                    {
//                        Implicit = new OpenApiOAuthFlow
//                        {
//                            AuthorizationUrl = new Uri(authUrl),
//                            Scopes = new Dictionary<string, string>
//                            {
//                                { "openid", "openid" },
//                                { "profile", "profile" },
//                                { "organization", "organization" },

//                            }
//                        },
//                        AuthorizationCode = new OpenApiOAuthFlow
//                        {
//                            AuthorizationUrl = new Uri(authUrl),
//                            TokenUrl = new Uri(tokenUrl), // Added TokenUrl
//                            Scopes = new Dictionary<string, string>
//                            {
//                                { "openid", "openid" },
//                                { "profile", "profile" },
//                                { "email", "email" }, // Added email scope as it's commonly used
//                                { "organization", "organization" },
//                            }
//                        },
//                    }
//                });

//                // Add security requirement for this realm
//                options.AddSecurityRequirement(new OpenApiSecurityRequirement
//                {
//                    {
//                        new OpenApiSecurityScheme
//                        {
//                            Reference = new OpenApiReference
//                            {
//                                Id = securitySchemeName,
//                                Type = ReferenceType.SecurityScheme
//                            },
//                            Scheme = JwtBearerDefaults.AuthenticationScheme,
//                            Name = JwtBearerDefaults.AuthenticationScheme,
//                            In = ParameterLocation.Header
//                        },
//                        new List<string>()
//                    }
//                });
//            }
//        });

//        return services;
//    }

//    internal static IServiceCollection AddKeycloakServices(this IServiceCollection services, IConfiguration configuration)
//    {
//        services.Configure<KeycloakConfiguration>(configuration.GetSection("Keycloak"));

//        KeycloakConfiguration keycloakConfig = configuration.GetSection("Keycloak").Get<KeycloakConfiguration>() ??
//            throw new Exception("Keycloak configuration is not configured");

//        services.AddTransient<IClaimsTransformation, KeycloakRoleClaimsTransformer>();

//        services.AddHttpClient<KeycloakHttpClient>(client =>
//        {
//            client.BaseAddress = new Uri(keycloakConfig.BaseUrl);
//        });

//        services.AddScoped<IKeycloakService, KeycloakService>();
//        services.AddScoped<IKeycloakRealmService, KeycloakRealmService>();
//        services.AddHttpContextAccessor();
//        services.AddScoped<KeycloakEndpointsFactory>();
//        services.AddScoped(sp => sp.GetRequiredService<KeycloakEndpointsFactory>().CreateEndpoints());

//        return services;
//    }

//    internal static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
//    {
//        KeycloakConfiguration keycloakConfig = configuration.GetSection("Keycloak").Get<KeycloakConfiguration>() ??
//            throw new Exception("Keycloak configuration is not configured");

//        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
//            {
//                options.RequireHttpsMetadata = false;
//                options.Audience = keycloakConfig.Authentication.Audience;
//                options.MetadataAddress = keycloakConfig.Authentication.MetadataAddress;
//                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//                {
//                    ValidIssuer = keycloakConfig.Authentication.ValidIssuer,
//                    RoleClaimType = ClaimTypes.Role
//                };

//                options.Events = new JwtBearerEvents
//                {
//                    OnTokenValidated = context =>
//                    {
//                        if (context.Principal?.Identity is ClaimsIdentity claimsIdentity)
//                        {
//                            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//                            var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;

//                            if (!string.IsNullOrEmpty(userId))
//                            {
//                                context.HttpContext.Items["JwtUserId"] = userId;
//                            }

//                            if (!string.IsNullOrEmpty(email))
//                            {
//                                context.HttpContext.Items["JwtEmail"] = email;
//                            }
//                        }

//                        return Task.CompletedTask;
//                    },
//                    OnAuthenticationFailed = context =>
//                    {
//                        // Handle authentication failure
//                        return Task.CompletedTask;
//                    }
//                };
//            });

//        return services;
//    }
//}
