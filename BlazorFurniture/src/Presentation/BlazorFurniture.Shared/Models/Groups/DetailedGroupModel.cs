namespace BlazorFurniture.Shared.Models.Groups;

public class DetailedGroupModel : GroupModel
{
    public string? Description { get; set; }
    public uint NumberOfMembers { get; set; }
    public IEnumerable<GroupRoleModel> Roles { get; set; } = [];
}
