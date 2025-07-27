namespace BlazorFurniture.AppHost.Configurations;

internal sealed class KeycloakDatabaseOptions : IConfig
{
    public string CONTAINER_NAME { get; set; }
    public string IMAGE { get; set; }
    public string POSTGRES_USER { get; set; }
    public string POSTGRES_PASSWORD { get; set; }
    public string POSTGRES_DB { get; set; }
    public string VOLUME_PATH { get; set; }
    public int CONTAINER_PORT { get; set; }
    public int HOST_PORT { get; set; }
    public string Url { get; set; }
}
