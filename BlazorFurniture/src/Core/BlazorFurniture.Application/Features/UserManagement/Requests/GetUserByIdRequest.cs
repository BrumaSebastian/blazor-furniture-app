using FastEndpoints;

namespace BlazorFurniture.Application.Features.UserManagement.Requests;

public class GetUserByIdRequest
{
    [RouteParam]
    public Guid UserId { get; init; }
}
