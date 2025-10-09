namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed record GroupRepresentation
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}
