using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Extensions.Endpoints;
using FastEndpoints;

namespace BlazorFurniture.Controllers.Groups;

public class UpdateUserGroupRole( ICommandDispatcher commandDispatcher ) : Endpoint<UpdateUserGroupRoleRequest>
{
    public override void Configure()
    {
        Put("{groupId:guid}/users/{userId:guid}/roles/{roleId:guid}");
        Group<GroupsEndpointGroup>();
        Summary(options =>
        {
            options.Summary = "Update user role within group";
            options.Description = "Updates the user role within a group, by removing the current and and asigning the new one";
            options.Response(StatusCodes.Status204NoContent);
            options.Response(StatusCodes.Status404NotFound);
            options.Response(StatusCodes.Status409Conflict);
            options.Response(StatusCodes.Status502BadGateway);
        });

        Description(options =>
        {
            options.ProducesProblem(StatusCodes.Status400BadRequest);
        });
    }

    public override async Task HandleAsync( UpdateUserGroupRoleRequest req, CancellationToken ct )
    {
        var result = await commandDispatcher.DispatchCommand<UpdateUserGroupRoleCommand, EmptyResult>(new UpdateUserGroupRoleCommand(req), ct);

        await result.Match(
            response => Send.NoContentAsync(),
            error => Send.SendErrorAsync(error));
    }
}
