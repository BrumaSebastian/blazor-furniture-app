using BlazorFurniture.Common.Abstractions;
using BlazorFurniture.Modules.Keycloak.Models;

namespace BlazorFurniture.Modules.Keycloak.Commands;

public class CreateRealmCommand(RealmRepresentation realmRepresentation) : ICommand
{
    public RealmRepresentation RealmRepresentation { get; } = realmRepresentation;
}
