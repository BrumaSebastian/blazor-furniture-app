using BlazorFurniture.Common.Abstractions;
using BlazorFurniture.Modules.Keycloak.Services;

namespace BlazorFurniture.Modules.Keycloak.Commands.Update;

public class UpdatePasswordHandler(IKeycloakService keycloakService) : ICommandHandler<UpdatePasswordCommand>
{
    private readonly IKeycloakService _keycloakService = keycloakService;

    public async Task HandleAsync(UpdatePasswordCommand command, CancellationToken cancellationToken = default)
    {
        await _keycloakService.UpdatePassword(command.UserId, new()
        {
            CurrentPassword = command.UpdatePasswordRequest.CurrentPassword,
            NewPassword = command.UpdatePasswordRequest.NewPassword,
            Email = command.Email
        });
    }
}
