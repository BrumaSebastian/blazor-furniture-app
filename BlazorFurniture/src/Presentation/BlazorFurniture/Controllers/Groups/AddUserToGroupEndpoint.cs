using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Controllers.Authorization.Policies;
using BlazorFurniture.Extensions.Endpoints;
using FastEndpoints;

namespace BlazorFurniture.Controllers.Groups;

internal sealed class AddUserToGroupEndpoint( ICommandDispatcher commandDispatcher ) : Endpoint<AddUserToGroupRequest>
{
    public override void Configure()
    {
        Post("{groupId:guid}/users/{userId:guid}");
        Group<GroupsEndpointGroup>();
        Policies(GroupPolicies.AddGroupUserPolicy);
        Summary(options =>
        {
            options.Summary = "Adds a user into a group with role User";
            options.Response(StatusCodes.Status204NoContent);
            options.Response(StatusCodes.Status404NotFound);
        });

        Description(options =>
        {
            options.ProducesProblem(StatusCodes.Status400BadRequest);
        });
    }

    public override async Task HandleAsync( AddUserToGroupRequest req, CancellationToken ct )
    {
        var result = await commandDispatcher.DispatchCommand<AddUserToGroupCommand, EmptyResult>(new AddUserToGroupCommand(req), ct);

        await result.Match(
            response => Send.NoContentAsync(),
            error => Send.SendErrorAsync(error));
    }
}
