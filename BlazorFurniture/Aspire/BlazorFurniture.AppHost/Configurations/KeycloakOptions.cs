namespace BlazorFurniture.AppHost.Configurations;

internal sealed class KeycloakOptions
{
    public required string KEYCLOAK_ADMIN { get; set; }
    public required string KEYCLOAK_ADMIN_PASSWORD { get; set; }
    public required string KC_DB { get; set; }
    public required string KC_DB_URL { get; set; }
    public required string KC_DB_USERNAME { get; set; }
    public required string KC_DB_PASSWORD { get; set; }
    public required string CONTAINER_NAME { get; set; }
    public required string IMAGE { get; set; }
    public string[] ARGS { get; set; } = [];
    public int CONTAINER_PORT { get; set; }
    public int HOST_PORT { get; set; }
    public List<string> Providers { get; set; } = [];
}
