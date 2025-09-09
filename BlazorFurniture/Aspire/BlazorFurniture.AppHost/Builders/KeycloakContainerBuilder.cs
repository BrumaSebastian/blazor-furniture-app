namespace BlazorFurniture.AppHost.Builders;

internal static class KeycloakContainerResourceBuilderExtensions
{
    public static IResourceBuilder<ContainerResource> WithKeycloakAdminAccount(this IResourceBuilder<ContainerResource> builder, string username, string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(username);
        ArgumentException.ThrowIfNullOrEmpty(password);

        builder.WithEnvironment("KEYCLOAK_ADMIN", username);
        builder.WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", password);

        return builder;
    }

    public static IResourceBuilder<ContainerResource> WithKeycloakDatabase(this IResourceBuilder<ContainerResource> builder, string database, string url, string username, string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(database);
        ArgumentException.ThrowIfNullOrEmpty(url);
        ArgumentException.ThrowIfNullOrEmpty(username);
        ArgumentException.ThrowIfNullOrEmpty(password);

        builder.WithEnvironment("KC_DB", database);
        builder.WithEnvironment("KC_DB_URL", url);
        builder.WithEnvironment("KC_DB_USERNAME", username);
        builder.WithEnvironment("KC_DB_PASSWORD", password);

        return builder;
    }
}
