namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients.Endpoints;

internal sealed partial class Endpoint
{
    private string OidBase => $"{BaseRealmUrl}/protocol/openid-connect";

    public string Authorization => $"{OidBase}/auth";
    public string Token => $"{OidBase}/token";
    public string UserInfo => $"{OidBase}/userinfo";
    public string Logout => $"{OidBase}/logout";
    public string WellKnownConfig => $"{BaseRealmUrl}/.well-known/openid-configuration";
}
