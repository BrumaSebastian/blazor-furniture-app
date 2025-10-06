using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Constants;
using BlazorFurniture.Core.Shared.Models.Errors;
using BlazorFurniture.Extensions;
using BlazorFurniture.Extensions.Endpoints;
using FastEndpoints;

namespace BlazorFurniture.Controllers.User;

public class GetProfileEndpoint( IQueryDispatcher queryDispatcher ) : EndpointWithoutRequest<UserProfileResponse>
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
        });

        Description(options =>
        {
            options.WithDescription("This endpoint retrieves the profile information of the currently authenticated user.");
            options.WithDisplayName("Retrieve Profile");
            options.WithTags(ControllerTags.User);
            options.Produces<UserProfileResponse>(StatusCodes.Status200OK);
            options.Produces<UserProfileResponse>(StatusCodes.Status404NotFound);
            options.Produces<UserProfileResponse>(StatusCodes.Status502BadGateway);
        }); 
    }

    public override async Task HandleAsync( CancellationToken ct )
    {
        var result = await queryDispatcher.DispatchQuery<GetUserProfileQuery, UserProfileResponse>(new GetUserProfileQuery(HttpContext.GetUserIdFromClaims()), ct);

        await result.Match(
            response => Send.OkAsync(result.Value),
            errors => result.Error switch
            {
                NotFoundError e => Send.NotFoundAsync(e),
                _ => Send.ErrorsAsync()
            });
    }
}
