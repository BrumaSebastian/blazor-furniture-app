using BlazorFurniture.Shared.Models.Users.Responses;
using Refit;

namespace BlazorFurniture.Shared.Services.API;

public interface IUserApi
{
    [Get("/api/user/permissions")]
    Task<UserPermissions> GetUserPermissions();

    [Get("/api/users/{userId}")]
    Task<UserProfile> GetUserProfile( Guid userId );
}
