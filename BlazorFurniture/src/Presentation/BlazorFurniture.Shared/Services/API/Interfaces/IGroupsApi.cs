using BlazorFurniture.Shared.Models;
using BlazorFurniture.Shared.Models.Groups;
using BlazorFurniture.Shared.Models.Users;
using Refit;

namespace BlazorFurniture.Shared.Services.API.Interfaces;

public interface IGroupsApi
{
    [Post("/api/groups")]
    Task<IApiResponse> Create(CreateGroupModel model, CancellationToken ct);

    [Get("/api/groups")]
    Task<IApiResponse<PaginatedModel<GroupModel>>> Get( int page, int pageSize, string? name, CancellationToken ct );

    [Get("/api/groups/{groupId}")]
    Task<IApiResponse<DetailedGroupModel>> Get( Guid groupId, CancellationToken ct = default);

    [Get("/api/groups/{groupId}/users")]
    Task<IApiResponse<PaginatedModel<GroupUserModel>>> GetUsers( Guid groupId, int page, int pageSize, string? name, CancellationToken ct );

    [Post("/api/groups/{groupId}/users/{userId}")]
    Task<IApiResponse> AddMember( Guid groupId, Guid userId, CancellationToken ct );
}
