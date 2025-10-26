using BlazorFurniture.Application.Common.Responses;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Application.Features.GroupManagement.Responses;
using BlazorFurniture.Validators.Groups;
using FastEndpoints;

namespace BlazorFurniture.Controllers.Groups;

internal sealed class GetGroupUsersEndpoint : Endpoint<GetGroupUsersRequest, PaginatedResponse<GroupUserResponse>>
{
    public override void Configure()
    {
        Get("{groupId:guid}/users");
        Group<GroupsEndpointGroup>();
        Validator<GetGroupUsersRequestValidator>();
        Summary(options =>
        {
            options.Summary = "Get all users of a group";
            options.Description = "Gets all users of a group with their role";
            options.Response(StatusCodes.Status200OK);
            options.Response(StatusCodes.Status403Forbidden);
        });
    }

    public override Task HandleAsync( GetGroupUsersRequest req, CancellationToken ct )
    {
        return base.HandleAsync(req, ct);
    }
}
