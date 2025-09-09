namespace BlazorFurniture.Domain.Entities.Keycloak;

public class CredentialRepresentation
{
    public string? Type { get; set; }
    public bool Temporary { get; set; }
    public string? Value { get; set; }
}
