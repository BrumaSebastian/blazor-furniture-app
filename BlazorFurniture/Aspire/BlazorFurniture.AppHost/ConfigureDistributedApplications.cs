using BlazorFurniture.AppHost.Configurations;
using Microsoft.Extensions.Configuration;

namespace BlazorFurniture.AppHost;

internal static class ConfigureDistributedApplications
{
    public static IDistributedApplicationBuilder AddKeycloak(this IDistributedApplicationBuilder applicationBuilder)
    {
        var databaseOptions = applicationBuilder.Configuration.GetSection("KeycloakDatabase").Get<KeycloakDatabaseOptions>()
            ?? throw new InvalidOperationException("Missing keycloak database configuration");

        var userName = applicationBuilder.AddParameter("postgresUser", databaseOptions.POSTGRES_USER, true);
        var password = applicationBuilder.AddParameter("postgresPassword", databaseOptions.POSTGRES_PASSWORD, true);

        var postgresContainer = applicationBuilder.AddPostgres(databaseOptions.CONTAINER_NAME)
            .WithUserName(userName)
            .WithPassword(password)
            .WithImage(databaseOptions.IMAGE)
            .WithVolume("postgres_data", databaseOptions.VOLUME_PATH)
            .WithHostPort(databaseOptions.HOST_PORT)
            .AddDatabase(databaseOptions.POSTGRES_DB);

        var options = applicationBuilder.Configuration.GetSection("Keycloak").Get<KeycloakOptions>()
            ?? throw new InvalidOperationException("Missing keycloak configuration");

        applicationBuilder.AddContainer(options.CONTAINER_NAME, options.IMAGE)
            .WithEnvironment(nameof(KeycloakOptions.KEYCLOAK_ADMIN), options.KEYCLOAK_ADMIN)
            .WithEnvironment(nameof(KeycloakOptions.KEYCLOAK_ADMIN_PASSWORD), options.KEYCLOAK_ADMIN_PASSWORD)
            .WithEnvironment(nameof(KeycloakOptions.KC_DB), options.KC_DB)
            .WithEnvironment(nameof(KeycloakOptions.KC_DB_URL), $"jdbc:postgresql://{databaseOptions.CONTAINER_NAME}:{databaseOptions.HOST_PORT.ToString()}/{databaseOptions.POSTGRES_DB}")
            .WithEnvironment(nameof(KeycloakOptions.KC_DB_USERNAME), options.KC_DB_USERNAME)
            .WithEnvironment(nameof(KeycloakOptions.KC_DB_PASSWORD), options.KC_DB_PASSWORD)
            .WithArgs(options.ARGS)
            .WithHttpEndpoint(options.HOST_PORT, options.CONTAINER_PORT)
            .WithReference(postgresContainer);

        return applicationBuilder;
    }
}
