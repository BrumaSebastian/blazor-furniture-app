using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Commands;
using BlazorFurniture.Application.Features.UserManagement.Requests;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Extensions;
using BlazorFurniture.Extensions.Endpoints;
using FastEndpoints;


namespace BlazorFurniture.Controllers.User;

public class UpdateProfileEndpoint( ICommandDispatcher commandDispatcher ) : Endpoint<UpdateUserProfileRequest>
{
    public override void Configure()
    {
        Put("profile");
        Group<UserEndpointGroup>();

        Summary(options =>
        {
            options.Summary = "Update user profile";
            options.Description = "Endpoint to update the profile of the currently authenticated user.";
            options.Response(StatusCodes.Status204NoContent, "Profile updated");
            options.Response(StatusCodes.Status403Forbidden);
        });

        Description(options =>
        {
            options.WithDescription("This endpoint updates the profile information of the currently authenticated user.");
            options.WithDisplayName("Profile Update");
            options.Produces(StatusCodes.Status204NoContent);
            options.Produces(StatusCodes.Status400BadRequest);
            options.Produces(StatusCodes.Status403Forbidden);
            options.Produces(StatusCodes.Status404NotFound);
            options.Produces(StatusCodes.Status409Conflict);
            options.Produces(StatusCodes.Status502BadGateway);
        });
    }

    public override async Task HandleAsync( UpdateUserProfileRequest req, CancellationToken ct )
    {
        var userId = HttpContext.GetUserIdFromClaims();
        var result = await commandDispatcher.Dispatch<UpdateUserProfileCommand, Result<EmptyResult>>(new UpdateUserProfileCommand(userId, req), ct);

        await result.Match(
            response => Send.NoContentAsync(),
            errors => result.Error switch
            {
                ConflictError e => Send.ConflictAsync(e),
                NotFoundError e => Send.NotFoundAsync(e),
                _ => Send.ErrorsAsync()
            });
    }
}
