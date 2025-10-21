using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Extensions.Endpoints;
using FastEndpoints;

namespace BlazorFurniture.Controllers.Groups;

internal sealed class RemoveUserFromGroupEndpoint( ICommandDispatcher commandDispatcher ) : Endpoint<RemoveUserFromGroupRequest>
{
    public override void Configure()
    {
        Delete("{groupId:guid}/users/{userId:guid}");
        Group<GroupsEndpointGroup>();

        Summary(options =>
        {
            options.Summary = "Remove a user from a group";
            options.Response(StatusCodes.Status204NoContent);
            options.Response(StatusCodes.Status404NotFound);
        });
    }

    public override async Task HandleAsync( RemoveUserFromGroupRequest req, CancellationToken ct )
    {
        var result = await commandDispatcher.Dispatch<RemoveUserFromGroupCommand, Result<EmptyResult>>(new RemoveUserFromGroupCommand(req), ct);

        await result.Match(
            response => Send.NoContentAsync(),
            error => Send.SendErrorAsync(error));
    }
}
