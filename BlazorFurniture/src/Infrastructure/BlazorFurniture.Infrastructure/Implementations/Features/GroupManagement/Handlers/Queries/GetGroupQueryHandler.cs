using BlazorFurniture.Application.Common.Extensions;
using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Queries;
using BlazorFurniture.Application.Features.GroupManagement.Responses;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Infrastructure.Constants;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Queries;

internal class GetGroupQueryHandler(
    IGroupManagementClient groupManagementClient,
    [FromKeyedServices(KeyedServices.KEYCLOAK)] IHttpErrorMapper errorMapper ) : IQueryHandler<GetGroupQuery, DetailedGroupResponse>
{
    public async Task<Result<DetailedGroupResponse>> HandleAsync( GetGroupQuery query, CancellationToken ct = default )
    {
        var groupResult = await groupManagementClient.Get(query.Request.GroupId, ct).ToDomainResult(errorMapper);

        if (groupResult.IsFailure)
        {
            return groupResult.PropagateFailure<DetailedGroupResponse>();
        }

        var groupMembersCount = await groupManagementClient.GetUsersCount(query.Request.GroupId, null, ct).ToDomainResult(errorMapper);

        if (groupMembersCount.IsFailure)
        {
            return groupMembersCount.PropagateFailure<DetailedGroupResponse>();
        }

        var groupRolesResult = await groupManagementClient.GetRoles(query.Request.GroupId, ct).ToDomainResult(errorMapper);

        if (groupRolesResult.IsFailure)
        {
            return groupRolesResult.PropagateFailure<DetailedGroupResponse>();
        }

        return groupResult.Map(g =>
            {
                var group = GroupMapping.ToDetailedGroupResponse(g);
                group.NumberOfMembers = groupMembersCount.Value.Count;
                group.Roles = groupRolesResult.Value.Select(gr => GroupMapping.ToGroupRoleResponse(gr));

                return group;
            });
    }
}
