using BlazorFurniture.Shared.Models.Groups;
using Refit;

namespace BlazorFurniture.Shared.Services.API;

public interface IGroupsApi
{
    [Post("/api/groups")]
    Task<IApiResponse> CreateGroup(CreateGroupModel model, CancellationToken ct);
}
