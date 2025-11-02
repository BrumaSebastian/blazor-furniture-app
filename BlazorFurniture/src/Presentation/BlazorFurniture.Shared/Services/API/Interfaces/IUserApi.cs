using BlazorFurniture.Shared.Models.Users.Responses;
using Refit;

namespace BlazorFurniture.Shared.Services.API.Interfaces;

public interface IUserApi
{
    [Get("/api/user/permissions")]
    Task<UserPermissions> GetUserPermissions(CancellationToken ct);

    [Get("/api/users/{userId}")]
    Task<UserProfile> GetUserProfile( Guid userId, CancellationToken ct );
}
