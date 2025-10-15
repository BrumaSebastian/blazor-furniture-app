using BlazorFurniture.Application.Common.Extensions;
using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Common.Responses;
using BlazorFurniture.Application.Features.GroupManagement.Queries;
using BlazorFurniture.Application.Features.GroupManagement.Responses;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Infrastructure.Constants;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Queries;

internal class GetGroupsQueryHandler(
    IGroupManagementClient groupManagementClient,
    [FromKeyedServices(KeyedServices.KEYCLOAK)] IHttpErrorMapper errorMapper ) : IQueryHandler<GetGroupsQuery, PaginatedResponse<GroupResponse>>
{
    public async Task<Result<PaginatedResponse<GroupResponse>>> HandleAsync( GetGroupsQuery query, CancellationToken ct = default )
    {
        var result = await groupManagementClient.Get(ct).ToDomainResult(errorMapper);

        return result.Map(groups => new PaginatedResponse<GroupResponse> { Results = groups.ToGroupResponses() });
    }
}
