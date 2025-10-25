using BlazorFurniture.Application.Common.Dispatchers;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Extensions;
using BlazorFurniture.Extensions.Endpoints;
using FastEndpoints;

namespace BlazorFurniture.Controllers.User;

public class GetPermissionsEndpoint( IQueryDispatcher queryDispatcher ) : EndpointWithoutRequest<UserPermissionsResponse>
{
    public override void Configure()
    {
        Get("permissions");
        Group<UserEndpointGroup>();

        Summary(options =>
        {
            options.Summary = "Retrieve user permissions";
            options.Response(StatusCodes.Status200OK);
        });
    }

    public override async Task HandleAsync( CancellationToken ct )
    {
        var userId = HttpContext.GetUserIdFromClaims();
        var result = await queryDispatcher.DispatchQuery<GetUserPermissionsQuery, UserPermissionsResponse>(new GetUserPermissionsQuery(userId), ct);

        await result.Match(
            response => Send.OkAsync(result.Value),
            error => Send.SendErrorAsync(error));
    }
}
