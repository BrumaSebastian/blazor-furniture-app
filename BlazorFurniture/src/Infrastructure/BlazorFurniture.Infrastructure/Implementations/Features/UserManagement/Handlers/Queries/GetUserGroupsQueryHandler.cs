using BlazorFurniture.Application.Common.Extensions;
using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Infrastructure.Constants;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Queries;

internal class GetUserGroupsQueryHandler(
    IUserManagementClient userManagementClient,
    [FromKeyedServices(KeyedServices.KEYCLOAK)] IHttpErrorMapper errorMapper ) : IQueryHandler<GetUserGroupsQuery, IEnumerable<UserGroupResponse>>
{
    public async Task<Result<IEnumerable<UserGroupResponse>>> HandleAsync( GetUserGroupsQuery query, CancellationToken ct = default )
    {
        var result = await userManagementClient.GetGroups(query.Request.UserId, ct).ToDomainResult(errorMapper);

        return result.Map(r => r.Select(representation => representation.ToResponse()));
    }
}
