using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Infrastructure.Constants;
using BlazorFurniture.Infrastructure.External.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Commands;

internal sealed class RemoveUserFromGroupCommandHandler(
    IGroupManagementClient groupManagementClient,
    [FromKeyedServices(KeyedServices.KEYCLOAK)] IHttpErrorMapper errorMapper )
    : ICommandHandler<RemoveUserFromGroupCommand, Result<EmptyResult>>
{
    public async Task<Result<EmptyResult>> HandleAsync( RemoveUserFromGroupCommand command, CancellationToken ct = default )
    {
        var result = await groupManagementClient.RemoveUser(command.Request.GroupId, command.Request.UserId, ct);

        return result.ToDomainResult(errorMapper);
    }
}
