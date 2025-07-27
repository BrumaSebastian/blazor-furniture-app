using BlazorFurniture.AppHost.Configurations;
using Microsoft.Extensions.Configuration;

namespace BlazorFurniture.AppHost;

internal static class ConfigureDistributedApplications
{
    public static IDistributedApplicationBuilder AddKeycloak(this IDistributedApplicationBuilder applicationBuilder)
    {
        var databaseOptions = applicationBuilder.Configuration.GetSection("KeycloakDatabase").Get<KeycloakDatabaseOptions>()
            ?? throw new InvalidOperationException("Missing keycloak database configuration");

        var userName = applicationBuilder.AddParameter(nameof(databaseOptions.User), databaseOptions.User, true);
        var password = applicationBuilder.AddParameter(nameof(databaseOptions.Password), databaseOptions.Password, true);

        var postgresContainer = applicationBuilder.AddPostgres(databaseOptions.ContainerName)
            .WithUserName(userName)
            .WithPassword(password)
            .WithImage(databaseOptions.Image)
            .WithVolume("postgres_data", databaseOptions.VolumePath)
            .WithHostPort(databaseOptions.HostPort)
            .AddDatabase(databaseOptions.DatabaseName);

        var options = applicationBuilder.Configuration.GetSection("Keycloak").Get<KeycloakOptions>()
            ?? throw new InvalidOperationException("Missing keycloak configuration");

        applicationBuilder.AddContainer(options.CONTAINER_NAME, options.IMAGE)
            .WithEnvironment(nameof(KeycloakOptions.KEYCLOAK_ADMIN), options.KEYCLOAK_ADMIN)
            .WithEnvironment(nameof(KeycloakOptions.KEYCLOAK_ADMIN_PASSWORD), options.KEYCLOAK_ADMIN_PASSWORD)
            .WithEnvironment(nameof(KeycloakOptions.KC_DB), options.KC_DB)
            .WithEnvironment(nameof(KeycloakOptions.KC_DB_URL), $"jdbc:postgresql://{databaseOptions.ContainerName}:{databaseOptions.HostPort.ToString()}/{databaseOptions.DatabaseName}")
            .WithEnvironment(nameof(KeycloakOptions.KC_DB_USERNAME), options.KC_DB_USERNAME)
            .WithEnvironment(nameof(KeycloakOptions.KC_DB_PASSWORD), options.KC_DB_PASSWORD)
            .WithArgs(options.ARGS)
            .WithHttpEndpoint(options.HOST_PORT, options.CONTAINER_PORT)
            .WithReference(postgresContainer);

        return applicationBuilder;
    }
}
