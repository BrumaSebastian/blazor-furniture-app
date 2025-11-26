using BlazorFurniture.Application.Common.Extensions;
using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Common.Responses;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Requests.Filters;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Infrastructure.Constants;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Queries;

internal sealed class GetUsersQueryHandler( 
    IUserManagementClient userManagementClient,
    [FromKeyedServices(KeyedServices.KEYCLOAK)] IHttpErrorMapper errorMapper ) 
    : IQueryHandler<GetUsersQuery, PaginatedResponse<UserResponse>>
{
    public async Task<Result<PaginatedResponse<UserResponse>>> HandleAsync( GetUsersQuery query, CancellationToken ct = default )
    {
        var userCountResult = await userManagementClient.Count(query.Request.Search, ct).ToDomainResult(errorMapper);

        if (userCountResult.IsFailure)
            return userCountResult.PropagateFailure<PaginatedResponse<UserResponse>>();

        var usersFilters = new UsersQueryFilters
        {
            Page = query.Request.Page,
            PageSize = query.Request.PageSize,
            Search = query.Request.Search
        };

        var usersResult = await userManagementClient.Get(usersFilters, ct).ToDomainResult(errorMapper);

        return usersResult.Map(users => new PaginatedResponse<UserResponse>
        {
            Total = userCountResult.Value.Count,
            Results = users.Select(u => u.ToResponse())
        });
    }
}
