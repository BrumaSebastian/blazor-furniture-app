using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Constants;
using BlazorFurniture.Extensions;
using BlazorFurniture.Extensions.Endpoints;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.Extensions;

namespace BlazorFurniture.Controllers.User;

public class ResetPasswordEndpoint() : EndpointWithoutRequest<UserProfileResponse>
{
    public override void Configure()
    {
        Get("reset-password");
        Group<UserEndpointGroup>();
        Summary(options =>
        {
            options.Summary = "Reset password";
            options.Description = "Redirecting to oidc provider and process the action of resetting the password.";
            options.Response(StatusCodes.Status302Found);
        });

        Description(options =>
        {
            options.WithDescription("Redirecting to oidc provider and process the action of resetting the password.");
            options.WithDisplayName("Reset password");
            options.ClearDefaultProduces(StatusCodes.Status200OK);
            options.Produces(StatusCodes.Status302Found);
        });
    }

    public override async Task HandleAsync( CancellationToken ct )
    {
        var returnUrl = UriHelper.BuildAbsolute(
            HttpContext.Request.Scheme,
            HttpContext.Request.Host,
            HttpContext.Request.PathBase,
            "/"
        );

        var props = new AuthenticationProperties { RedirectUri = returnUrl };
        props.Items[Oidc.Actions.KEYCLOAK_ACTIONS] = Oidc.Actions.UPDATE_PASSWORD;

        await Send.ResultAsync(TypedResults.Challenge(
            props,
            [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]
        ));
    }
}
