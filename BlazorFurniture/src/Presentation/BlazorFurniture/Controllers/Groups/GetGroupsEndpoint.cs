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

internal sealed class GetGroupsEndpoint( IQueryDispatcher queryDispatcher ) : Endpoint<GetGroupsRequest, PaginatedResponse<GroupResponse>>
{
    public override void Configure()
    {
        Get("");
        Group<GroupsEndpointGroup>();
        Validator<GetGroupsRequestValidator>();
        Policies(GroupPolicies.ListGroupsPolicy);
        Summary(options =>
        {
            options.Summary = "Get all groups";
            options.Description = "Gets all groups with roles";
            options.Response(StatusCodes.Status200OK);
            options.Response(StatusCodes.Status403Forbidden);
            options.Response(StatusCodes.Status502BadGateway);
        });

        Description(options =>
        {
            options.WithDescription("This endpoint retrieves all groups.");
            options.WithDisplayName("Retrieve All Groups");
            options.Produces<IEnumerable<GroupResponse>>(StatusCodes.Status200OK);
            options.Produces(StatusCodes.Status403Forbidden);
            options.Produces(StatusCodes.Status502BadGateway);
        });
    }

    public override async Task HandleAsync( GetGroupsRequest req, CancellationToken ct )
    {
        var result = await queryDispatcher.DispatchQuery<GetGroupsQuery, PaginatedResponse<GroupResponse>>(new GetGroupsQuery(req), ct);

        await result.Match(
            response => Send.OkAsync(response),
            error => Send.SendErrorAsync(error));
    }
}
