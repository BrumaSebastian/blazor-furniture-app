namespace BlazorFurniture.Domain.Entities.Keycloak;

public class UserRepresentation
{
    public Guid Id { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public bool EmailVerified { get; set; }
    public bool Enabled { get; set; }
    public List<string> RealmRoles { get; set; } = [];
}
