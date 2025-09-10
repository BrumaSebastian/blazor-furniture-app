namespace BlazorFurniture.AppHost.Configurations;

internal sealed class KeycloakOptions
{
    public required string AdminUsername { get; set; }
    public required string AdminPassword { get; set; }
    public required string DatabaseType { get; set; }
    public string DatabaseURL { get; set; } = default!;
    public required string DatabaseUsername { get; set; }
    public required string DatabasePassword { get; set; }
    public required string ContainerName { get; set; }
    public required string Image { get; set; }
    public string[] Args { get; set; } = [];
    public required int ContainerPort { get; set; }
    public required int HostPort { get; set; }
    public List<string> Providers { get; set; } = [];
}
