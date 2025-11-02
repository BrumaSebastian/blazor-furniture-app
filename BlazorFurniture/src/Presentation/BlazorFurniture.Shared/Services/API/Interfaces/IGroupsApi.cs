using BlazorFurniture.Shared.Models;
using BlazorFurniture.Shared.Models.Groups;
using Refit;

namespace BlazorFurniture.Shared.Services.API.Interfaces;

public interface IGroupsApi
{
    [Post("/api/groups")]
    Task<IApiResponse> Create(CreateGroupModel model, CancellationToken ct);

    [Get("/api/groups")]
    Task<IApiResponse<PaginatedModel<GroupModel>>> Get( int page, int pageSize, string? name, CancellationToken ct );
}
