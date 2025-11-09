using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Infrastructure.Constants;
using BlazorFurniture.Infrastructure.External.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Commands;

internal sealed class AddUserToGroupCommandHandler(
    IGroupManagementClient groupManagementClient,
    [FromKeyedServices(KeyedServices.KEYCLOAK)] IHttpErrorMapper errorMapper )
    : ICommandHandler<AddUserToGroupCommand, EmptyResult>
{
    public async Task<Result<EmptyResult>> HandleAsync( AddUserToGroupCommand command, CancellationToken ct = default )
    {
        var result = await groupManagementClient.AddUser(command.Request.GroupId, command.Request.UserId, ct);

        return result.ToDomainResult(errorMapper);
    }
}
