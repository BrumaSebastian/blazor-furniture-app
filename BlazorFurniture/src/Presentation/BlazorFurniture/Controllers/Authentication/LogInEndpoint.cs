using BlazorFurniture.Application.Features.Authentication.Requests;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BlazorFurniture.Controllers.Authentication;

public class LogInEndpoint : Endpoint<LogInRequest>
{
    public override void Configure()
    {
        Get("/authentication/login");
        AllowAnonymous();

        Description(options =>
        {
            options.WithDescription("This endpoint authenticates the user.");
            options.WithDisplayName("Authenticate");
        });
    }

    public override async Task HandleAsync( LogInRequest req, CancellationToken ct )
    {
        await Send.ResultAsync(TypedResults.Challenge(GetAuthProperties(req.RedirectUrl, HttpContext),
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
