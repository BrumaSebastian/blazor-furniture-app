using BlazorFurniture.Shared.Services.Security.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BlazorFurniture.Shared.Security.Authorization;

public sealed class PermissionAuthorizationHandler( IPermissionsService permissions )
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync( AuthorizationHandlerContext context, PermissionRequirement requirement )
    {
        if (await permissions.HasPermission(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
