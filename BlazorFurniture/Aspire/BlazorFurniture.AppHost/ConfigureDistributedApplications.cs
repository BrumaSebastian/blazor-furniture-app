using BlazorFurniture.AppHost.Builders;
using BlazorFurniture.AppHost.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace BlazorFurniture.AppHost;

internal static class ConfigureDistributedApplications
{
    extension(IDistributedApplicationBuilder applicationBuilder)
    {
        public IResourceBuilder<IResource> AddKeycloak()
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
                .WithLifetime(ContainerLifetime.Persistent)
                .AddDatabase(databaseOptions.DatabaseName);

            var options = applicationBuilder.Configuration.GetSection("Keycloak").Get<KeycloakOptions>()
                ?? throw new InvalidOperationException("Missing keycloak configuration");

            string dbUrl = string.IsNullOrWhiteSpace(options.DatabaseURL)
                ? $"jdbc:postgresql://{databaseOptions.ContainerName}:{databaseOptions.HostPort}/{databaseOptions.DatabaseName}"
                : options.DatabaseURL;
            var keycloakContainer = applicationBuilder.AddContainer(options.ContainerName, options.Image)
                    .WithKeycloakAdminAccount(options.AdminUsername, options.AdminPassword)
                    .WithKeycloakDatabase(options.DatabaseType, dbUrl, options.DatabaseUsername, options.DatabasePassword)
                    .WithArgs(options.Args)
                    .WithHttpEndpoint(options.HostPort, options.ContainerPort)
                    .WithLifetime(ContainerLifetime.Persistent)
                    .WithReference(postgresContainer);

            if (applicationBuilder.Environment.IsDevelopment())
            {
                foreach (var providerPath in options.Providers.Where(p => !string.IsNullOrWhiteSpace(p)))
                {
                    keycloakContainer = keycloakContainer.WithBindMount(providerPath, $"/opt/keycloak/providers/{Path.GetFileName(providerPath)}");
                }
            }

            return keycloakContainer;
        }

        public void AddMaildev()
        {
            var options = applicationBuilder.Configuration.GetSection("Maildev").Get<MaildevOptions>()
                ?? throw new InvalidOperationException("Missing maildev configuration");

            applicationBuilder.AddContainer(options.ContainerName, options.Image)
                .WithEnvironment("MAILDEV_SMTP_PORT", options.Ports.HostSmtp.ToString())
                .WithEnvironment("MAILDEV_WEB_PORT", options.Ports.HostWeb.ToString())
                .WithEndpoint(options.Ports.HostSmtp, options.Ports.ContainerSmtp, name: MaildevOptions.SmtpEndpointName)
                .WithUrlForEndpoint(MaildevOptions.SmtpEndpointName, e =>
                {
                    e.DisplayText = MaildevOptions.SmtpEndpointName;
                })
                .WithHttpEndpoint(options.Ports.HostWeb, options.Ports.ContainerWeb, name: MaildevOptions.WebEndpointName)
                .WithUrlForEndpoint(MaildevOptions.WebEndpointName, e =>
                {
                    e.DisplayText = MaildevOptions.WebEndpointName;
                })
                .WithLifetime(ContainerLifetime.Persistent);
        }
    }
}
