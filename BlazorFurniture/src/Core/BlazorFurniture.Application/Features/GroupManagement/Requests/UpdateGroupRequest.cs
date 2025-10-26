namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public sealed class UpdateGroupRequest : GetGroupRequest
{
    public required string Name { get; set; }
}
