using BlazorFurniture.Controllers.Authorization.Requirements;
using BlazorFurniture.Infrastructure.External.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace BlazorFurniture.Controllers.Authorization.Handlers;

public class UmaWithClaimsAuthorizationHandler( IUmaAuthorizationService umaAuthorizationService )
    : AuthorizationHandler<PermissionWithClaimsRequirement>
{
    protected override async Task HandleRequirementAsync( AuthorizationHandlerContext context, PermissionWithClaimsRequirement requirement )
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
        var accessToken = GetAccessToken(authorizationHeader);
        var claims = GetClaimsFromRoute(httpContext, requirement.Claims);
        var response = await umaAuthorizationService.Evaluate(accessToken, permission, claims, httpContext.RequestAborted);

        if (response.IsFailure)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }

    private Dictionary<string, string> GetClaimsFromRoute( HttpContext httpContext, IEnumerable<string> claims )
    {
        return claims.Where(httpContext.Request.RouteValues.ContainsKey)
            .ToDictionary(claimKey => claimKey, claimKey => httpContext.Request.RouteValues[claimKey]?.ToString() ?? string.Empty);
    }

    private static string GetAccessToken( string authorizationHeader )
    {
        return authorizationHeader[JwtBearerDefaults.AuthenticationScheme.Length..].TrimStart();
    }

    private static string CreatePermission( PermissionWithClaimsRequirement requirement )
    {
        return requirement.Scope is Scopes.Undefined
            ? requirement.Resource
            : $"{requirement.Resource}#{requirement.Scope.ToString().ToLowerInvariant()}";
    }
}
