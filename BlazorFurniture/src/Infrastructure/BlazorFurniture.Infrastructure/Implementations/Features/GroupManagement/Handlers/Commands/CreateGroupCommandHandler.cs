using BlazorFurniture.Application.Common.Extensions;
using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Infrastructure.Constants;
using BlazorFurniture.Infrastructure.External.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Handlers.Commands;

internal sealed class CreateGroupCommandHandler(
    IGroupManagementClient groupManagementClient,
    [FromKeyedServices(KeyedServices.KEYCLOAK)] IHttpErrorMapper errorMapper )
    : ICommandHandler<CreateGroupCommand, HttpHeaderLocationResult>
{
    public async Task<Result<HttpHeaderLocationResult>> HandleAsync( CreateGroupCommand command, CancellationToken ct = default )
    {
        var createGroupResult = await groupManagementClient.Create(command.Request.Name, command.Request.Description, ct)
            .ToDomainResult(errorMapper);

        return createGroupResult;
    }
}
