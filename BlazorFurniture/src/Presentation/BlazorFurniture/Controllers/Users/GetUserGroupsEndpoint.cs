using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Requests;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Extensions.Endpoints;
using FastEndpoints;

namespace BlazorFurniture.Controllers.Users;

public class GetUserGroupsEndpoint( IQueryDispatcher queryDispatcher ) : Endpoint<GetUserByIdRequest, IEnumerable<UserGroupResponse>>
{
    public override void Configure()
    {
        Get("{UserId:guid}/groups");
        Group<UsersEndpointGroup>();
        Summary(options =>
        {
            options.Summary = "Retrieve user groups by ID";
            options.Description = "Endpoint to get the groups associated with a specific user by their ID.";
            options.Response(StatusCodes.Status200OK);
            options.Response<IEnumerable<UserGroupResponse>>();
        });
    }

    public override async Task HandleAsync( GetUserByIdRequest req, CancellationToken ct )
    {
        var result = await queryDispatcher.DispatchQuery<GetUserGroupsQuery, IEnumerable<UserGroupResponse>>(new GetUserGroupsQuery(req), ct);

        await result.Match(
            response => Send.OkAsync(response),
            error => Send.SendErrorAsync(error));
    }
}
