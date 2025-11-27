using BlazorFurniture.Shared.Models.Users.Responses;

namespace BlazorFurniture.Shared.Services.Security.Interfaces;

public interface IPermissionsService
{
    public event Action? Changed;
    public Task<UserPermissionsModel?> GetUserPermissions( bool force = false, CancellationToken ct = default );
    public Task<bool> HasPermission( string permission, CancellationToken ct = default );
    public Task<bool> HasGroupPermission( string permission, Guid groupId, CancellationToken ct = default );
    public Task Refresh( CancellationToken ct = default );
    public void ClearCache();
}
