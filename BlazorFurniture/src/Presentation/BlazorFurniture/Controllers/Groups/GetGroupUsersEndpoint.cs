using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Responses;
using BlazorFurniture.Application.Features.GroupManagement.Queries;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Application.Features.GroupManagement.Responses;
using BlazorFurniture.Controllers.Authorization.Policies;
using BlazorFurniture.Extensions.Endpoints;
using BlazorFurniture.Validators.Groups;
using FastEndpoints;

namespace BlazorFurniture.Controllers.Groups;

internal sealed class GetGroupUsersEndpoint( IQueryDispatcher queryDispatcher ) : Endpoint<GetGroupUsersRequest, PaginatedResponse<GroupUserResponse>>
{
    public override void Configure()
    {
        Get("{groupId:guid}/users");
        Group<GroupsEndpointGroup>();
        Validator<GetGroupUsersRequestValidator>();
        Policies(GroupPolicies.ListGroupUsersPolicy);
        Summary(options =>
        {
            options.Summary = "Get all users of a group";
            options.Description = "Gets all users of a group with their role";
            options.Response(StatusCodes.Status200OK);
            options.Response(StatusCodes.Status403Forbidden);
        });
    }

    public override async Task HandleAsync( GetGroupUsersRequest req, CancellationToken ct )
    {
        var result = await queryDispatcher.DispatchQuery<GetGroupUsersQuery, PaginatedResponse<GroupUserResponse>>(new GetGroupUsersQuery(req), ct);

        await result.Match(
            response => Send.OkAsync(response),
            error => Send.SendErrorAsync(error));
    }
}
