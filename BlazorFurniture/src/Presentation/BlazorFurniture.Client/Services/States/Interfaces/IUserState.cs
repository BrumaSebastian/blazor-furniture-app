using BlazorFurniture.Shared.Models.Users.Responses;

namespace BlazorFurniture.Client.Services.States.Interfaces;

public interface IUserState
{
    UserProfileModel? CurrentUser { get; }
    event Action? Changed;

    void SetUser( UserProfileModel user );
    Task Refresh( CancellationToken ct = default );
}
