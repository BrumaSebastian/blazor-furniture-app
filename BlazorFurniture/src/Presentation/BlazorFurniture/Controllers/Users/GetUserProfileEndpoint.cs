using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Requests;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Core.Shared.Utils.Extensions;
using BlazorFurniture.Extensions;
using BlazorFurniture.Extensions.Endpoints;
using BlazorFurniture.Shared.DTOs.Users.Responses;
using FastEndpoints;

namespace BlazorFurniture.Controllers.Users;

public class GetUserProfileEndpoint( IQueryDispatcher queryDispatcher )
    : Endpoint<GetUserByIdRequest, UserProfileResponse>
{
    public override void Configure()
    {
        Get("{userId:guid}");
        Group<UsersEndpointGroup>();
        Summary(options =>
        {
            options.Summary = "Get user profile";
            options.Description = "Endpoint to get the profile of a user.";
            options.Response<UserProfileResponse>(StatusCodes.Status200OK);
            options.Response(StatusCodes.Status403Forbidden);
        });
    }

    public override async Task HandleAsync( GetUserByIdRequest req, CancellationToken ct )
    {
        var result = await queryDispatcher.DispatchQuery<GetUserProfileQuery, UserProfileResponse>(new GetUserProfileQuery(req.UserId), ct);

        await result.Match(
            response => Send.OkAsync(response),
            error => Send.SendErrorAsync(error));
    }
}
