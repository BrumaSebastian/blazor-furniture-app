using BlazorFurniture.Application.Features.UserManagement.Requests;
using BlazorFurniture.Constants;
using FastEndpoints;

namespace BlazorFurniture.Controllers.User;

public class UpdateProfileEndpoint : Endpoint<UpdateUserProfileRequest>
{
    public override void Configure()
    {
        Put("/api/user/profile");
        AllowAnonymous();

        Summary(options =>
        {
            options.Summary = "Update user profile";
            options.Description = "Endpoint to update the profile of the currently authenticated user.";
            options.Response(204, "Profile updated");
        });

        Description(options =>
        {
            options.WithDescription("This endpoint updates the profile information of the currently authenticated user.");
            options.WithDisplayName("Profile Update");
            options.WithTags(ControllerTags.User);
            options.Produces(204);
        });
    }

    public override async Task HandleAsync( UpdateUserProfileRequest req, CancellationToken ct )
    {
        await Send.NoContentAsync(ct);
    }
}
