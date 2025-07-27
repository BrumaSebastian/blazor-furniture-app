using BlazorFurniture.Modules.Keycloak.Models;
using BlazorFurniture.Modules.Keycloak.Models.DTOs;

namespace BlazorFurniture.Modules.Keycloak.Services;

public interface IKeycloakService
{
    Task<User> GetUserAsync(Guid id);
    Task<List<User>> GetUsersAsync();
    Task<User> GetUserByPhoneNumberAsync(string phoneNumber);
    Task<bool> UpdatePassword(string userId, UpdatePassword updatePasswordDto);
}
