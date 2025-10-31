using BlazorFurniture.Client.Services.Interfaces;
using BlazorFurniture.Shared.DTOs.Users.Responses;
using BlazorFurniture.Shared.Services.API;
using Refit;

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
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized
                                   || ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            // User is not authenticated or lacks permissions
            return null;
        }
        catch (HttpRequestException)
        {
            // Network error or server unavailable
            return null;
        }
    }

    public async Task<bool> HasPermission( string permission )
    {
        var permissions = await GetUserPermissions();
        return permissions?.Permissions?.Contains(permission) ?? false;
    }
}
