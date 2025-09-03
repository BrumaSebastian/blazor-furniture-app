namespace BlazorFurniture.AppHost.Configurations;

internal sealed class KeycloakDatabaseOptions
{
    public required string ContainerName { get; set; }
    public required string Image { get; set; }
    public required string User { get; set; }
    public required string Password { get; set; }
    public required string DatabaseName { get; set; }
    public required string VolumePath { get; set; }
    public required int ContainerPath { get; set; }
    public required int HostPort { get; set; }
    public required string Url { get; set; }
}
