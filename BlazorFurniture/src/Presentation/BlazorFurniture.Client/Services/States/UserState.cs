using BlazorFurniture.Client.Services.States.Interfaces;
using BlazorFurniture.Shared.Extensions;
using BlazorFurniture.Shared.Models.Users.Responses;
using BlazorFurniture.Shared.Services.API.Interfaces;

namespace BlazorFurniture.Client.Services.States;

public class UserState( IUsersClient usersClient ) : IUserState
{
    public UserProfileModel CurrentUser { get; private set; } = default!;

    public event Action? Changed;
    private readonly SemaphoreSlim gate = new(1, 1);

    public async Task Refresh( CancellationToken ct = default )
    {
        await gate.WaitAsync(ct);

        try
        {
            var response = await usersClient.GetUserProfile(ct);
            var result = response.ToApiResult();

            if (result.IsSuccess)
            {
                CurrentUser = result.Data!;
                Changed?.Invoke();
            }
        }
        finally
        {
            gate.Release();
        }
    }

    public void SetUser( UserProfileModel user )
    {
        CurrentUser = user;
        Changed?.Invoke();
    }
}
