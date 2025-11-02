using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace BlazorFurniture.Shared.Security.Authorization;

public sealed class PermissionPolicyProvider( IOptions<AuthorizationOptions> options ) 
    : DefaultAuthorizationPolicyProvider(options)
{
    public const string Prefix = "Permission:";

    public override Task<AuthorizationPolicy?> GetPolicyAsync( string policyName )
    {
        if (!string.IsNullOrEmpty(policyName) && policyName.StartsWith(Prefix, StringComparison.Ordinal))
        {
            var permission = policyName.Substring(Prefix.Length);
            var policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(permission))
                .RequireAuthenticatedUser()
                .Build();

            return Task.FromResult<AuthorizationPolicy?>(policy);
        }

        return base.GetPolicyAsync(policyName!);
    }
}
