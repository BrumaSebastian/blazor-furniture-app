//using BlazorFurniture.Application.Keycloak.Services;
//using BlazorFurniture.Common.Abstractions;

//namespace BlazorFurniture.Application.Keycloak.Users.Commands;

//public class CreateRealmHandler(IKeycloakRealmService keycloakRealmService) : ICommandHandler<CreateRealmCommand>
//{
//    private readonly IKeycloakRealmService _keycloakRealmService = keycloakRealmService;

//    public async Task HandleAsync(CreateRealmCommand command, CancellationToken cancellationToken = default)
//    {
//        var res = await _keycloakRealmService.CreateRealmAsync(command.RealmRepresentation);
//    }
//}
