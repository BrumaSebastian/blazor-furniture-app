using System.Text.Json.Serialization;

namespace BlazorFurniture.AppHost.Configurations;

internal sealed class KeycloakDatabaseOptions
{
    public string ContainerName { get; set; }
    public string Image { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
    public string DatabaseName { get; set; }
    public string VolumePath { get; set; }
    public int ContainerPath { get; set; }
    public int HostPort { get; set; }
    public string Url { get; set; }
}
