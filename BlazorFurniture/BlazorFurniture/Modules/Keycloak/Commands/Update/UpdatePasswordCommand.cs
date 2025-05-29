using BlazorFurniture.Common.Abstractions;
using BlazorFurniture.Modules.Keycloak.Models;

namespace BlazorFurniture.Modules.Keycloak.Commands.Update;

public class UpdatePasswordCommand(UpdatePasswordRequest updatePasswordRequest, string email, string userId) : ICommand
{
    public UpdatePasswordRequest UpdatePasswordRequest { get; } = updatePasswordRequest;
    public string Email { get; } = email;
    public string UserId { get; } = userId;
}
