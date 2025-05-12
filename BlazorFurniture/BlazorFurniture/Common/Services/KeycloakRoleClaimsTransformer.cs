using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace BlazorFurniture.Common.Services;

public class KeycloakRoleClaimsTransformer : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        AddRealmRoles(principal);
        AddClientRoles(principal);

        return Task.FromResult(principal);
    }

    private void AddRealmRoles(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity)principal.Identity;
        var realmAccess = principal.FindFirst("realm_access")?.Value;

        if (realmAccess is not null)
        {
            using var doc = JsonDocument.Parse(realmAccess);

            if (doc.RootElement.TryGetProperty("roles", out var roles))
            {
                foreach (var role in roles.EnumerateArray())
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()));
                }
            }
        }
    }

    private void AddClientRoles(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity)principal.Identity;
        var resourceAccess = principal.FindFirst("resource_access")?.Value;
        var clientId = principal.FindFirst("azp")?.Value;

        if (resourceAccess != null)
        {
            using var doc = JsonDocument.Parse(resourceAccess);
            if (doc.RootElement.TryGetProperty(clientId, out var clientAccess))
            {
                if (clientAccess.TryGetProperty("roles", out var roles))
                {
                    foreach (var role in roles.EnumerateArray())
                        identity.AddClaim(new Claim(ClaimTypes.Role, role.GetString()));
                }
            }
        }
    }
}
