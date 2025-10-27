using BlazorFurniture.Shared.DTOs.Users.Responses;

namespace BlazorFurniture.Client.Services.Interfaces;

public interface IPermissionsService
{
    Task<UserPermissions?> GetUserPermissions();
    Task<bool> HasPermission( string permission );
    //Task<bool> CanAccessAdminPanel();
    void ClearCache();
}
