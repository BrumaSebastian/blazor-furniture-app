using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Constants;
using BlazorFurniture.Core.Shared.Models.Errors;
using BlazorFurniture.Extensions;
using BlazorFurniture.Extensions.Endpoints;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BlazorFurniture.Controllers.User;

public class GetProfileEndpoint( IQueryDispatcher QueryDispatcher ) : EndpointWithoutRequest<UserProfileResponse>
{
    public override void Configure()
    {
        Get("profile");
        Group<UserEndpointGroup>();
        AuthSchemes(OpenIdConnectDefaults.AuthenticationScheme);
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

    public override async Task HandleAsync( CancellationToken ct )
    {
        //new GetUserProfileQuery(HttpContext.GetUserIdFromClaims()
        var result = await QueryDispatcher.DispatchQuery<GetUserProfileQuery, UserProfileResponse>(new GetUserProfileQuery(Guid.Parse("73dd88d4-f059-4677-9d74-29fba1309ba8")), ct);

        await result.Match(
            response => Send.OkAsync(result.Value),
            errors => result.Error switch
            {
                NotFoundError e => Send.NotFoundAsync(e),
                _ => Send.ErrorsAsync()
            });
    }
}
