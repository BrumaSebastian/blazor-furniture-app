namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients.Endpoints;

internal sealed partial class Endpoint
{
    private string BaseUsersEndpoint => $"{AdminBaseUrl}/users";

    public string Users() => BaseUsersEndpoint;
    public string User(string userId) => $"{BaseUsersEndpoint}/{userId}";
}
