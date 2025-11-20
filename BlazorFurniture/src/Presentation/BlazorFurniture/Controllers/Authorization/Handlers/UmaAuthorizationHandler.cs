using BlazorFurniture.Controllers.Authorization.Requirements;
using BlazorFurniture.Infrastructure.External.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
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

        string? accessToken = null;

        if (!string.IsNullOrEmpty(authorizationHeader))
        {
            accessToken = GetAccessToken(authorizationHeader);
        }
        else if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            accessToken = await httpContext.GetTokenAsync(OpenIdConnectDefaults.AuthenticationScheme, "access_token");
        }

        if (string.IsNullOrEmpty(accessToken))
        {
            context.Fail();
            return;
        }

        var response = await umaAuthorizationService.Evaluate(accessToken, requirement.Resource, [requirement.Scope.ToString().ToLower()], httpContext.RequestAborted);

        if (response.IsFailure || !response.Value.IsAuthorized)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }

    private static string GetAccessToken( string authorizationHeader )
    {
        return authorizationHeader[JwtBearerDefaults.AuthenticationScheme.Length..].TrimStart();
    }
}
