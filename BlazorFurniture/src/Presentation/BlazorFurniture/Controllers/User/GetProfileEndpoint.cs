using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Constants;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Core.Shared.Utils.Extensions;
using BlazorFurniture.Extensions;
using BlazorFurniture.Extensions.Endpoints;
using FastEndpoints;

namespace BlazorFurniture.Controllers.User;

public class GetProfileEndpoint(
    IQueryDispatcher queryDispatcher,
    IEmailNotificationService emailNotificationService,
    ILogger<GetProfileEndpoint> logger )
    : EndpointWithoutRequest<UserProfileResponse>
{
    public override void Configure()
    {
        Get("profile");
        Group<UserEndpointGroup>();
        Summary(options =>
        {
            options.Summary = "Get user profile";
            options.Description = "Endpoint to get the profile of the currently authenticated user.";
            options.Response<UserProfileResponse>(StatusCodes.Status200OK);
            options.Response(StatusCodes.Status403Forbidden);
        });

        Description(options =>
        {
            options.WithDescription("This endpoint retrieves the profile information of the currently authenticated user.");
            options.WithDisplayName("Retrieve Profile");
            options.WithTags(ControllerTags.User);
            options.Produces<UserProfileResponse>(StatusCodes.Status200OK);
            options.Produces(StatusCodes.Status403Forbidden);
            options.Produces(StatusCodes.Status404NotFound);
            options.Produces(StatusCodes.Status502BadGateway);
        });
    }

    public override async Task HandleAsync( CancellationToken ct )
    {
        var userId = HttpContext.GetUserIdFromClaims();
        var result = await queryDispatcher.DispatchQuery<GetUserProfileQuery, UserProfileResponse>(new GetUserProfileQuery(userId), ct);

        await result.Match(
            response =>
            {
                _ = emailNotificationService.SendWelcomeEmail(new(response.Username, response.Email!, new System.Globalization.CultureInfo("en")), ct)
                    .LogOnFaulted(logger);

                return Send.OkAsync(result.Value);
            },
            errors => result.Error switch
            {
                NotFoundError e => Send.NotFoundAsync(e),
                _ => Send.ErrorsAsync()
            });
    }
}
