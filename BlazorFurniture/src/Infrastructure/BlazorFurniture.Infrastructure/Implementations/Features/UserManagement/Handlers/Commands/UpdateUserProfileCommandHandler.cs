using BlazorFurniture.Application.Common.Extensions;
using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Commands;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.Constants;
using BlazorFurniture.Infrastructure.External.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Commands;

internal sealed class UpdateUserProfileCommandHandler(
    IUserManagementClient userManagementClient,
    [FromKeyedServices(KeyedServices.KEYCLOAK)] IHttpErrorMapper errorMapper )
    : ICommandHandler<UpdateUserProfileCommand, EmptyResult>
{
    public async Task<Result<EmptyResult>> HandleAsync( UpdateUserProfileCommand command, CancellationToken ct = default )
    {
        var getUser = await userManagementClient.Get(command.UserId, ct)
            .ToDomainResult(errorMapper, command.UserId);

        if (!getUser.TryGetValue(out var userRepresentation))
            return getUser.PropagateFailure<EmptyResult>();

        userRepresentation.FirstName = command.Request.FirstName;
        userRepresentation.LastName = command.Request.LastName;
        userRepresentation.Email = command.Request.Email;
        userRepresentation.Attributes ??= [];
        userRepresentation.Attributes[UserRepresentation.ATTRIBUTE_AVATAR] = [command.Request.Avatar];

        var updateUser = await userManagementClient.UpdateProfile(userRepresentation, ct)
            .ToDomainResult(errorMapper, command.UserId);

        return updateUser;
    }
}
