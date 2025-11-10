using BlazorFurniture.Controllers.Authorization.Requirements;
using BlazorFurniture.Infrastructure.External.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BlazorFurniture.Controllers.Authorization.Handlers;

public class UmaAuthorizationHandler( IUmaAuthorizationService umaAuthorizationService )
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync( AuthorizationHandlerContext context, PermissionRequirement requirement )
    {
        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return;
        }

        var authorizationHeader = httpContext.Request.Headers.Authorization.ToString();

        if (string.IsNullOrEmpty(authorizationHeader))
        {
            context.Fail();
            return;
        }

        var permission = CreatePermission(requirement);
        var response = await umaAuthorizationService.Evaluate(authorizationHeader, permission, httpContext.RequestAborted);

        if (response.IsFailure)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }

    private static string CreatePermission( PermissionRequirement requirement )
    {
        return requirement.Scope is Scopes.Undefined
            ? requirement.Resource
            : $"{requirement.Resource}#{requirement.Scope.ToString().ToLowerInvariant()}";
    }
}
