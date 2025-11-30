using BlazorFurniture.Client.Services.States.Interfaces;
using BlazorFurniture.Shared.Extensions;
using BlazorFurniture.Shared.Models.Users.Responses;
using BlazorFurniture.Shared.Services.API.Interfaces;

namespace BlazorFurniture.Client.Services.States;

public class UserState( IUsersClient usersClient ) : IUserState
{
    public UserProfileModel CurrentUser { get; private set; } = default!;

    public event Action? Changed;
    private readonly Lock userLock = new();

    public async Task Refresh( CancellationToken ct = default )
    {
        var response = await usersClient.GetUserProfile(ct);
        var result = response.ToApiResult();
        Action? changedHandler = null;

        lock (userLock)
        {
            if (result.IsSuccess)
            {
                CurrentUser = result.Data!;
                changedHandler = Changed;
            }
        }

        changedHandler?.Invoke();
    }

    public void SetUser( UserProfileModel user )
    {
        Action? changedHandler;

        lock (userLock)
        {
            CurrentUser = user;
            changedHandler = Changed;
        }

        changedHandler?.Invoke();
    }
}
