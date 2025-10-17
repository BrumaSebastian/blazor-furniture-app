namespace BlazorFurniture.Infrastructure.External.Keycloak.Configurations;

internal sealed class Endpoints( string Realm )
{
    private string BaseRealmUrl => $"realms/{Realm}";
    private string AdminRestApiBaseUrl => $"admin/realms/{Realm}";

    public string AccessToken() => $"{BaseRealmUrl}/protocol/openid-connect/token";

    // Users
    public string Users() => $"{AdminRestApiBaseUrl}/users";
    public string UserById( Guid userId ) => $"{AdminRestApiBaseUrl}/users/{userId}";

    // Groups
    public string Groups() => $"{AdminRestApiBaseUrl}/groups";
    public string GroupsExtension() => $"{Groups()}-extension";
    public string GroupsCount() => $"{Groups()}/count";
}
