namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed record RoleRepresentation
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
}
