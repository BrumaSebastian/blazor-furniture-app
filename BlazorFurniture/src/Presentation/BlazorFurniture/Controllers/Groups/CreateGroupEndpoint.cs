using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Extensions;
using BlazorFurniture.Extensions.Endpoints;
using BlazorFurniture.Validators.Groups;
using FastEndpoints;

namespace BlazorFurniture.Controllers.Groups;

internal sealed class CreateGroupEndpoint( ICommandDispatcher commandDispatcher ) : Endpoint<CreateGroupRequest>
{
    public override void Configure()
    {
        Post("");
        Group<GroupsEndpointGroup>();
        Validator<CreateGroupRequestValidator>();
        Summary(options =>
        {
            options.Summary = "Create group";
            options.Description = "Creates a group with roles";
            options.Response(StatusCodes.Status201Created);
            options.Response(StatusCodes.Status400BadRequest);
            options.Response(StatusCodes.Status403Forbidden);
            options.Response(StatusCodes.Status409Conflict);
            options.Response(StatusCodes.Status502BadGateway);
        });

        Description(options =>
        {
            options.WithDisplayName("Create group");
            options.WithDescription("Creates a group with roles");
            options.Produces(StatusCodes.Status204NoContent);
            options.Produces(StatusCodes.Status400BadRequest);
            options.Produces(StatusCodes.Status403Forbidden);
            options.Produces(StatusCodes.Status409Conflict);
            options.Produces(StatusCodes.Status502BadGateway);
        });
    }

    public async override Task HandleAsync( CreateGroupRequest req, CancellationToken ct )
    {
        var result = await commandDispatcher.Dispatch<CreateGroupCommand, Result<HttpHeaderLocationResult>>(new CreateGroupCommand(req), ct);

        await result.Match(
            response => Send.CreatedAtAsync(result.Value.Location?.ToString() ?? string.Empty),
            error => Send.SendErrorAsync(error));
    }
}
