using BlazorFurniture.Application.Features.GroupManagement.Responses;
using BlazorFurniture.Constants;
using FastEndpoints;

namespace BlazorFurniture.Controllers.Groups;

internal sealed class GetGroupsEndpoint : EndpointWithoutRequest<IEnumerable<GroupResponse>>
{
    public override void Configure()
    {
        Get("");
        Group<GroupsEndpointGroup>();
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
            options.WithTags(ControllerTags.Groups);
            options.Produces<IEnumerable<GroupResponse>>(StatusCodes.Status200OK);
            options.Produces(StatusCodes.Status403Forbidden);
            options.Produces(StatusCodes.Status502BadGateway);
        });
    }

    public async override Task HandleAsync( CancellationToken ct )
    {
        await Send.OkAsync(
        [
            new() { Id = Guid.NewGuid(), Name = "Admin" },
            new() { Id = Guid.NewGuid(), Name = "User" }
        ], ct);
    }
}
