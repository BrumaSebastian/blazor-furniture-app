using BlazorFurniture.Modules.Keycloak.Extensions;
using BlazorFurniture.Modules.Keycloak.Models;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace BlazorFurniture.Modules.Keycloak.Services;

public class KeycloakEndpointsFactory(
    IHttpContextAccessor httpContextAccessor,
    IOptions<KeycloakConfiguration> keycloakConfig)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly KeycloakConfiguration _defaultConfig = keycloakConfig.Value;

    public KeycloakEndpoints CreateEndpoints()
    {
        var realm = GetRealmFromToken() ?? _defaultConfig.ServiceClient.Realm;
        return new KeycloakEndpoints(_defaultConfig.BaseUrl, realm);
    }

    private string? GetRealmFromToken()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return null;

        // Option 1: Extract from issuer claim (iss)
        // Format: http://keycloak-server/realms/realm-name
        var issuer = httpContext.User.FindFirstValue("iss");
        if (!string.IsNullOrEmpty(issuer))
        {
            var segments = issuer.Split('/');
            var realmIndex = Array.IndexOf(segments, "realms");
            if (realmIndex >= 0 && realmIndex < segments.Length - 1)
                return segments[realmIndex + 1];
        }

        // Option 2: Look for a custom claim that directly specifies realm
        return httpContext.User.FindFirstValue("realm") ??
               httpContext.User.FindFirstValue("tenant_id");
    }
}
