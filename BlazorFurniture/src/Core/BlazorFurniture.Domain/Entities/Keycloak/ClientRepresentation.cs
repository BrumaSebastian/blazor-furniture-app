namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed class ClientRepresentation
{
    public Guid Id { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
