using BlazorFurniture.Application.Common.Extensions;
using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;

namespace BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Queries;

internal class GetUserProfileQueryHandler( IUserManagementClient userManagementClient ) : IQueryHandler<GetUserProfileQuery, UserProfileResponse>
{
    public async Task<Result<UserProfileResponse>> HandleAsync( GetUserProfileQuery query, CancellationToken ct = default )
    {
        var result = await userManagementClient.Get(query.Id, ct)
            .ToResult((status, error) => KeycloakErrorMapper.MapFor<UserRepresentation>(status, error, query.Id));

        return result.Map(u => u.ToUserProfile());
    }
}
