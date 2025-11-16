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

        var accessToken = GetAccessToken(authorizationHeader);
        var claims = GetClaimsFromRoute(httpContext, requirement.Claims);
        var response = await umaAuthorizationService.Evaluate(
            accessToken,
            requirement.Resource,
            [requirement.Scope.ToString().ToLower()],
            claims!,
            httpContext.RequestAborted);

        if (response.IsFailure || !response.Value.IsAuthorized)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }

    private static Dictionary<string, List<string>> GetClaimsFromRoute( HttpContext httpContext, IEnumerable<string> claims )
    {
        Dictionary<string, List<string>> collection = [];

        foreach (var claim in claims.Where(httpContext.Request.RouteValues.ContainsKey))
        {
            collection.Add(claim, [httpContext.Request.RouteValues[claim]?.ToString() ?? string.Empty]);
        }

        return collection;
    }

    private static string GetAccessToken( string authorizationHeader )
    {
        return authorizationHeader[JwtBearerDefaults.AuthenticationScheme.Length..].TrimStart();
    }
}
