using BlazorFurniture.Domain.Enums;

namespace BlazorFurniture.Domain.Entities.Keycloak;

public sealed record GroupUserRepresentation
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public bool Enabled { get; set; }
    public GroupRoles Role { get; set; }
}
