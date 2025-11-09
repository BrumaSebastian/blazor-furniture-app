using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Infrastructure.Constants;
using BlazorFurniture.Infrastructure.External.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Commands;

internal sealed class UpdateGroupCommandHandler(
    IGroupManagementClient groupManagementClient,
    [FromKeyedServices(KeyedServices.KEYCLOAK)] IHttpErrorMapper errorMapper )
    : ICommandHandler<UpdateGroupCommand, EmptyResult>
{
    public async Task<Result<EmptyResult>> HandleAsync( UpdateGroupCommand command, CancellationToken ct = default)
    {
        var groupResult = await groupManagementClient.Get(command.Request.GroupId, ct);

        if (groupResult.IsFailure)
        {
            return groupResult.ToDomainResult(errorMapper, command.Request.GroupId).PropagateFailure<EmptyResult>();
        }

        var groupRepresentation = groupResult.Value;
        groupRepresentation.Name = command.Request.Name;

        var updateResult = await groupManagementClient.Update(command.Request.GroupId, groupRepresentation, ct);

        return updateResult.ToDomainResult(errorMapper, command.Request.GroupId);
    }
}
