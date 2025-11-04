using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Features.GroupManagement.Queries;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Application.Features.GroupManagement.Responses;
using BlazorFurniture.Extensions.Endpoints;
using FastEndpoints;

namespace BlazorFurniture.Controllers.Groups;

public class GetGroupEndpoint( IQueryDispatcher queryDispatcher ) : Endpoint<GetGroupByIdRequest, DetailedGroupResponse>
{
    public override void Configure()
    {
        Get("{groupId:guid}");
        Group<GroupsEndpointGroup>();
        Summary(options =>
        {
            options.Summary = "Get group by ID";
            options.Description = "Endpoint to get a group by its unique identifier.";
            options.Response<DetailedGroupResponse>(StatusCodes.Status200OK);
            options.Response(StatusCodes.Status403Forbidden);
        });
    }

    public override async Task HandleAsync( GetGroupByIdRequest req, CancellationToken ct )
    {
        var result = await queryDispatcher.DispatchQuery<GetGroupQuery, DetailedGroupResponse>(new GetGroupQuery(req), ct);

        await result.Match(
            response => Send.OkAsync(response),
            error => Send.SendErrorAsync(error));
    }
}
