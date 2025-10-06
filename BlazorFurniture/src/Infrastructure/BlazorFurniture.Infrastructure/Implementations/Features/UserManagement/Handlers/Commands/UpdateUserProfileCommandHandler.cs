using BlazorFurniture.Application.Common.Extensions;
using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Commands;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Interfaces;
using BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;

namespace BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Commands;

internal class UpdateUserProfileCommandHandler( IUserManagementClient userManagementClient )
    : ICommandHandler<UpdateUserProfileCommand, Result<EmptyResult>>
{
    public async Task<Result<EmptyResult>> HandleAsync( UpdateUserProfileCommand command, CancellationToken ct = default )
    {
        var getUser = await userManagementClient.Get(command.Request.Id, ct)
            .ToResult((status, error) => KeycloakErrorMapper.MapFor<UserRepresentation>(status, error, command.Request.Id));

        if (!getUser.TryGetValue(out var userRepresentation))
            return getUser.PropagateFailure<EmptyResult>();

        userRepresentation.FirstName = command.Request.FirstName;
        userRepresentation.LastName = command.Request.LastName;
        userRepresentation.Email = command.Request.Email;

        var updateUser = await userManagementClient.UpdateProfile(userRepresentation, ct)
            .ToResult(( status, error ) => KeycloakErrorMapper.MapFor<UserRepresentation>(status, error, command.Request.Id));

        return updateUser;
    }
}
