using BlazorFurniture.Common.Abstractions;
using BlazorFurniture.Modules.Keycloak.Services;

namespace BlazorFurniture.Modules.Keycloak.Commands;

public class CreateRealmHandler(IKeycloakRealmService keycloakRealmService) : ICommandHandler<CreateRealmCommand>
{
    private readonly IKeycloakRealmService _keycloakRealmService = keycloakRealmService;

    public async Task HandleAsync(CreateRealmCommand command, CancellationToken cancellationToken = default)
    {
        var res = await _keycloakRealmService.CreateRealmAsync(command.RealmRepresentation);
    }
}
