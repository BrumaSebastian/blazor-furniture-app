using BlazorFurniture.Client.Services.API;
using BlazorFurniture.Client.Services.Interfaces;
using BlazorFurniture.Shared.DTOs.Users.Responses;

namespace BlazorFurniture.Client.Services;

public class PermissionsService( IUserApi userApi ) : IPermissionsService
{
    private UserPermissions? userPermissions;

    public void ClearCache()
    {
        userPermissions = null;
    }

    public async Task<UserPermissions?> GetUserPermissions()
    {
        if (userPermissions is not null)
            return userPermissions;

        try
        {
            userPermissions = await userApi.GetUserPermissions();
            return userPermissions;
        }
        catch (HttpRequestException)
        {
            // User is not authenticated or endpoint failed
            return null;
        }
    }

    public async Task<bool> HasPermission( string permission )
    {
        var permissions = await GetUserPermissions();
        return permissions?.Permissions?.Contains(permission) ?? false;
    }
}
