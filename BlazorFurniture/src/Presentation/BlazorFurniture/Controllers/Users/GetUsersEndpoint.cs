using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Responses;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Requests;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Extensions.Endpoints;
using FastEndpoints;

namespace BlazorFurniture.Controllers.Users;

public class GetUsersEndpoint(IQueryDispatcher queryDispatcher) : Endpoint<GetUsersRequest, PaginatedResponse<UserResponse>>
{
    public override void Configure()
    {
        Get("");
        Group<UsersEndpointGroup>();
        Summary(options =>
        {
            options.Summary = "Retrieve users";
            options.Description = "Endpoint to get the users.";
            options.Response(StatusCodes.Status200OK);
        });
    }

    public override async Task HandleAsync( GetUsersRequest req, CancellationToken ct )
    {
        var result = await queryDispatcher.DispatchQuery<GetUsersQuery, PaginatedResponse<UserResponse>>(new GetUsersQuery(req), ct);

        await result.Match(
            response => Send.OkAsync(response),
            error => Send.SendErrorAsync(error));
    }
}
