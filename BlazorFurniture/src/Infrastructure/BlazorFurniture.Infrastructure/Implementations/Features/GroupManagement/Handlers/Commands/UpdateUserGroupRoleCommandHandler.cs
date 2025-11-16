using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Infrastructure.Constants;
using BlazorFurniture.Infrastructure.External.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Commands;

internal sealed class UpdateUserGroupRoleCommandHandler( 
    IGroupManagementClient groupManagementClient,
    [FromKeyedServices(KeyedServices.KEYCLOAK)] IHttpErrorMapper errorMapper ) 
    : ICommandHandler<UpdateUserGroupRoleCommand, EmptyResult>
{
    public async Task<Result<EmptyResult>> HandleAsync( UpdateUserGroupRoleCommand command, CancellationToken ct = default )
    {
        var updateResult = await groupManagementClient.UpdateUserRole(command.Request.GroupId, command.Request.UserId, command.Request.RoleId, ct);

        return updateResult.ToDomainResult(errorMapper, command.Request.GroupId);
    }
}
