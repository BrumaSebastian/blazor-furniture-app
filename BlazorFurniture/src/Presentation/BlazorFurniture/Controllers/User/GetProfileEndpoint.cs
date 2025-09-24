using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Constants;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BlazorFurniture.Controllers.User;

public class GetProfileEndpoint(IQueryDispatcher QueryDispatcher) : EndpointWithoutRequest<UserProfileResponse>
{
    public override void Configure()
    {
        Get("/api/user/profile");
        //AuthSchemes(OpenIdConnectDefaults.AuthenticationScheme);
        AllowAnonymous();
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
        var userProfile = await QueryDispatcher.DispatchQuery<GetUserProfileQuery, UserProfileResponse>(new GetUserProfileQuery(Guid.NewGuid()), ct);
        await Send.OkAsync(userProfile, ct);
    }
}
