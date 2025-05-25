namespace BlazorFurniture.Modules.Keycloak.Models;

public class KeycloakEndpoints(string baseUrl, string realm)
{
    private readonly string _baseUrl = baseUrl.TrimEnd('/');
    private readonly string _realm = realm;

    public string BaseRealmUrl => $"{_baseUrl}/realms/{_realm}";

    // OpenID Connect endpoints
    public string Authorization => $"{BaseRealmUrl}/protocol/openid-connect/auth";
    public string Token => $"{BaseRealmUrl}/protocol/openid-connect/token";
    public string UserInfo => $"{BaseRealmUrl}/protocol/openid-connect/userinfo";
    public string Logout => $"{BaseRealmUrl}/protocol/openid-connect/logout";
    public string WellKnownConfig => $"{BaseRealmUrl}/.well-known/openid-configuration";

    // Admin API endpoints
    public string AdminBaseUrl => $"{_baseUrl}/admin/realms/{_realm}";
    public string Users => $"{AdminBaseUrl}/users";

    // Helper methods
    public string UserById(string userId) => $"{Users}/{userId}";
}
