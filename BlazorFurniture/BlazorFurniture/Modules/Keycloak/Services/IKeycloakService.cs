using BlazorFurniture.Modules.Keycloak.Models;

namespace BlazorFurniture.Modules.Keycloak.Services;

public interface IKeycloakService
{
    Task<User> GetUserAsync(Guid id);
    Task<List<User>> GetUsersAsync();
}
