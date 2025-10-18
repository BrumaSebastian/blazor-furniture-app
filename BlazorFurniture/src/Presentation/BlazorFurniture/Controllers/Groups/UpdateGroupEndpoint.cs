using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Commands;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Core.Shared.Errors;
using BlazorFurniture.Extensions.Endpoints;
using FastEndpoints;

namespace BlazorFurniture.Controllers.Groups;

internal sealed class UpdateGroupEndpoint( ICommandDispatcher commandDispatcher ) : Endpoint<UpdateGroupRequest>
{
    public override void Configure()
    {
        Put("{id:guid}");
        Group<GroupsEndpointGroup>();

        Summary(options =>
        {
            options.Summary = "Update group";
            options.Description = "Updates an existing group by its ID.";
            options.Response(StatusCodes.Status204NoContent);
            options.Response<ProblemDetails>(StatusCodes.Status400BadRequest);
            options.Response(StatusCodes.Status404NotFound);
            options.Response(StatusCodes.Status409Conflict);
            options.Response(StatusCodes.Status502BadGateway);
        });

        Description(options =>
        {
            options.ProducesProblem(StatusCodes.Status400BadRequest);
        });
    }

    public override async Task HandleAsync( UpdateGroupRequest req, CancellationToken ct )
    {
        var result = await commandDispatcher.Dispatch<UpdateGroupCommand, Result<EmptyResult>>(new UpdateGroupCommand(req), ct);

        await result.Match(
            response => Send.NoContentAsync(),
            errors => result.Error switch
            {
                NotFoundError e => Send.NotFoundAsync(e),
                ConflictError e => Send.ConflictAsync(e),
                _ => Send.ErrorsAsync()
            });
    }
}
