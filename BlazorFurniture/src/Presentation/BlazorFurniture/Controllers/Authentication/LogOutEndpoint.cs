using BlazorFurniture.Application.Features.Authentication.Requests;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BlazorFurniture.Controllers.Authentication;

public class LogOutEndpoint : Endpoint<LogOutRequest>
{
    public override void Configure()
    {
        Get("authentication/logout");
        RoutePrefixOverride(string.Empty);
        AllowAnonymous();

        Description(options =>
        {
            options.WithDescription("This endpoint logs out the user.");
            options.WithDisplayName("Log Out");
        });
    }

    public override async Task HandleAsync( LogOutRequest req, CancellationToken ct )
    {
        await Send.ResultAsync(TypedResults.SignOut(GetAuthProperties(req.RedirectUrl, HttpContext),
            [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]));
    }

    private static AuthenticationProperties GetAuthProperties( string? returnUrl, HttpContext httpContext )
    {
        string pathBase = httpContext.Request.PathBase.Value!;

        // Prevent open redirects.
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = "/";
        }
        else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
        {
            returnUrl = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
        }
        else if (returnUrl[0] != '/')
        {
            returnUrl = $"{pathBase}{returnUrl}";
        }

        return new AuthenticationProperties { RedirectUri = returnUrl };
    }
}
