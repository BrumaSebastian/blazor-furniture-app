using BlazorFurniture.Shared.Models;
using BlazorFurniture.Shared.Models.Users;
using BlazorFurniture.Shared.Models.Users.Responses;
using Refit;

namespace BlazorFurniture.Shared.Services.API.Interfaces;

public interface IUsersApi
{
    [Get("/api/user/permissions")]
    Task<UserPermissionsModel> GetUserPermissions(CancellationToken ct);

    [Get("/api/users/{userId}")]
    Task<IApiResponse<UserProfileModel>> GetUserProfile( Guid userId, CancellationToken ct );

    [Get("/api/users/{userId}/groups")]
    Task<IApiResponse<List<UserGroupMembershipModel>>> GetUserGroups( Guid userId, CancellationToken ct );

    [Get("/api/users")]
    Task<IApiResponse<PaginatedModel<UserModel>>> GetUsers( int page, int pageSize, string? search, CancellationToken ct );
}
