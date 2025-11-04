using BlazorFurniture.Shared.Models.Users;

namespace BlazorFurniture.Shared.Models.Groups;

public class GroupRoleModel
{
    public required Guid Id { get; set; }
    public required GroupRoles Role { get; set; }
}
