namespace BlazorFurniture.Infrastructure.External.Keycloak.Clients.Endpoints;

internal sealed partial class Endpoint(string baseUrl, string realm)
{
    private string BaseRealmUrl => $"{baseUrl}/realms/{realm}";
    private string AdminBaseUrl => $"{baseUrl}/admin/realms/{realm}";
}
