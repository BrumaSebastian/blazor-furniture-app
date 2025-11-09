using BlazorFurniture.Shared.Models.Users.Responses;
using Refit;

namespace BlazorFurniture.Shared.Services.API.Interfaces;

public interface IUserApi
{
    [Get("/api/user/permissions")]
    Task<UserPermissionsModel> GetUserPermissions(CancellationToken ct);

    [Get("/api/users/{userId}")]
    Task<UserProfileModel> GetUserProfile( Guid userId, CancellationToken ct );

    [Get("/api/users/{userId}/groups")]
    Task<IApiResponse<List<UserGroupMembershipModel>>> GetUserGroups( Guid userId, CancellationToken ct );
}
