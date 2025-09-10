//using BlazorFurniture.Application.Keycloak.Services;
//using BlazorFurniture.Common.Abstractions;

//namespace BlazorFurniture.Application.Keycloak.Users.Commands.Update;

//public class UpdatePasswordHandler(IKeycloakService keycloakService) : ICommandHandler<UpdatePasswordCommand>
//{
//    private readonly IKeycloakService _keycloakService = keycloakService;

//    public async Task HandleAsync(UpdatePasswordCommand command, CancellationToken cancellationToken = default)
//    {
//        await _keycloakService.UpdatePassword(command.UserId, new()
//        {
//            CurrentPassword = command.UpdatePasswordRequest.CurrentPassword,
//            NewPassword = command.UpdatePasswordRequest.NewPassword,
//            Email = command.Email
//        });
//    }
//}
