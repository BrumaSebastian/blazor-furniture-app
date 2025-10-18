using FastEndpoints;

namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public sealed class UpdateGroupRequest
{
    [RouteParam]
    public required Guid Id { get; set; }
    public required string Name { get; set; }
}
