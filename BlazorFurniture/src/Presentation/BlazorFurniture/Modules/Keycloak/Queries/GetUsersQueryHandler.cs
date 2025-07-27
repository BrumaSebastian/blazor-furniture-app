using BlazorFurniture.Common.Abstractions;
using BlazorFurniture.Modules.Keycloak.Models;
using BlazorFurniture.Modules.Keycloak.Services;

namespace BlazorFurniture.Modules.Keycloak.Queries;

public class GetUsersQueryHandler(IKeycloakService keycloakService) : IQueryHandler<GetUsersQuery, List<User>>
{
    private readonly IKeycloakService keycloakService = keycloakService;

    public async Task<List<User>> HandleAsync(GetUsersQuery query, CancellationToken cancellationToken = default)
    {
        return await keycloakService.GetUsersAsync();
    }
}
