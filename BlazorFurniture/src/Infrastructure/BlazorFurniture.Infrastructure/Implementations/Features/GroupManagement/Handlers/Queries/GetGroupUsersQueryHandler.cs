using BlazorFurniture.Application.Common.Extensions;
using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Common.Responses;
using BlazorFurniture.Application.Features.GroupManagement.Queries;
using BlazorFurniture.Application.Features.GroupManagement.Requests.Filters;
using BlazorFurniture.Application.Features.GroupManagement.Responses;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Infrastructure.Constants;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Queries;

internal sealed class GetGroupUsersQueryHandler(
    IGroupManagementClient groupManagementClient,
    [FromKeyedServices(KeyedServices.KEYCLOAK)] IHttpErrorMapper errorMapper ) :
    IQueryHandler<GetGroupUsersQuery, PaginatedResponse<GroupUserResponse>>
{
    public async Task<Result<PaginatedResponse<GroupUserResponse>>> HandleAsync( GetGroupUsersQuery query, CancellationToken ct = default )
    {
        var countGroupsResult = await groupManagementClient.GetUsersCount(query.Request.GroupId, ct).ToDomainResult(errorMapper);

        if (countGroupsResult.IsFailure)
            return countGroupsResult.PropagateFailure<PaginatedResponse<GroupUserResponse>>();

        var queryFilter = new GroupUsersQueryFilter
        {
            Page = query.Request.Page,
            PageSize = query.Request.PageSize,
            Search = query.Request.Search
        };

        var groupUserResult = await groupManagementClient.GetUsers(query.Request.GroupId, queryFilter, ct).ToDomainResult(errorMapper);

        return groupUserResult.Map(users =>
            new PaginatedResponse<GroupUserResponse>
            {
                Results = users.Select(u => u.ToResponse()),
                Total = countGroupsResult.Value.Count
            });
    }
}
