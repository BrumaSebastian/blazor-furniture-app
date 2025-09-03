namespace BlazorFurniture.AppHost.Configurations;

internal sealed class MaildevOptions
{
    public required string ContainerName { get; set; }
    public required string Image { get; set; }
    public required MaildevPorts Ports { get; set; }
}

internal sealed class MaildevPorts
{
    public required int HostSmtp { get; set; }
    public required int HostWeb { get; set; }
    public required int ContainerSmtp { get; set; }
    public required int ContainerWeb { get; set; }
}