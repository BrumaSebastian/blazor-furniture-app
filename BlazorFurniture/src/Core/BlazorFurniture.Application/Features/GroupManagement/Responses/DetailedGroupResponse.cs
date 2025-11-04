namespace BlazorFurniture.Application.Features.GroupManagement.Responses;

public class DetailedGroupResponse : GroupResponse
{
    public string? Description { get; set; }
    public uint NumberOfMembers { get; set; }
    public IEnumerable<GroupRoleResponse> Roles { get; set; } = [];
}
