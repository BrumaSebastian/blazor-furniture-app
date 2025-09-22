using BlazorFurniture.Application.DTOs.Users.Responses;
using BlazorFurniture.Constants;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BlazorFurniture.Controllers.User;

public class GetProfileEndpoint : EndpointWithoutRequest<UserProfileResponse>
{
    public override void Configure()
    {
        Get("/api/user/profile");
        AuthSchemes(OpenIdConnectDefaults.AuthenticationScheme);
        //AllowAnonymous();
        Summary(options =>
        {
            options.Summary = "Get user profile";
            options.Description = "Endpoint to get the profile of the currently authenticated user.";
            options.Response<UserProfileResponse>(200);
        });

        Description(options =>
        {
            options.WithDescription("This endpoint retrieves the profile information of the currently authenticated user.");
            options.WithDisplayName("Retrieve Profile");
            options.WithTags(ControllerTags.User);
            options.Produces<UserProfileResponse>(200);
        });
    }

    public override async Task HandleAsync(CancellationToken ct )
    {
        await Send.OkAsync(new()
        {
            Id = Guid.NewGuid().ToString(),
            Username = "john_doe",
        }, ct);
    }
}
