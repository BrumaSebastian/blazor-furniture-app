using BlazorFurniture.Shared.Models.Users.Responses;
using BlazorFurniture.Shared.Services.API.Interfaces;
using BlazorFurniture.Shared.Services.Security.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Refit;

namespace BlazorFurniture.Shared.Services.Security;

public class PermissionsService( IUserApi userApi, AuthenticationStateProvider authStateProvider ) : IPermissionsService
{
    private static readonly TimeSpan TimeToLive = TimeSpan.FromSeconds(30);
    private UserPermissions? userPermissions;
    private DateTimeOffset expiresAt = DateTimeOffset.MinValue;
    public event Action? Changed;
    private bool subscribed;

    public async Task<UserPermissions?> GetUserPermissions( bool force = false, CancellationToken ct = default )
    {
        EnsureSubscribedToAuthChanges();

        if (!force && userPermissions is not null && DateTimeOffset.UtcNow < expiresAt)
            return userPermissions;

        // No cache or force: fetch now
        return await FetchAndUpdateAsync(ct);
    }

    public async Task<bool> HasPermission( string permission, CancellationToken ct = default )
    {
        var permissions = await GetUserPermissions(ct: ct);
        return permissions?.Permissions?.Contains(permission) ?? false;
    }

    public Task Refresh( CancellationToken ct = default )
    {
        return GetUserPermissions(force: true, ct);
    }

    public void ClearCache()
    {
        userPermissions = null;
        expiresAt = DateTimeOffset.MinValue;
        Changed?.Invoke();
    }

    public void Dispose()
    {
        if (subscribed)
            authStateProvider.AuthenticationStateChanged -= OnAuthChanged;
    }

    private void EnsureSubscribedToAuthChanges()
    {
        if (subscribed) return;

        authStateProvider.AuthenticationStateChanged += OnAuthChanged;

        subscribed = true;
    }

    private async Task<UserPermissions?> FetchAndUpdateAsync( CancellationToken ct )
    {
        try
        {
            userPermissions = await userApi.GetUserPermissions(ct);
            expiresAt = DateTimeOffset.UtcNow.Add(TimeToLive);
            Changed?.Invoke();

            return userPermissions;
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized
                                   || ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            SetNullAndNotify();
            return null;
        }
        catch (HttpRequestException)
        {
            // Keep old cache if available (stale)
            return userPermissions;
        }
    }

    private void SetNullAndNotify()
    {
        userPermissions = null;
        expiresAt = DateTimeOffset.MinValue;
        Changed?.Invoke();
    }
    private async void OnAuthChanged( Task<AuthenticationState> _ )
    {
        ClearCache();
        await Refresh(); // optional: prefetch after login/refresh
    }
}
