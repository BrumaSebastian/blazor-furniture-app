using BlazorFurniture.Modules.Keycloak.Models;

namespace BlazorFurniture.Modules.Keycloak.Services;

public interface IKeycloakRealmService
{
    //Task<List<Realm>> GetRealmsAsync();
    //Task<Realm> GetRealmAsync(string realmName);
    Task<RealmRepresentation> CreateRealmAsync(RealmRepresentation realm);
    //Task<Realm> UpdateRealmAsync(Realm realm);
    //Task DeleteRealmAsync(string realmName);
}
