namespace BlazorFurniture.Modules.Keycloak.Models;

public record CreateUserRequest(string Username, string Email, string FirstName, string LastName, List<string> Roles);
