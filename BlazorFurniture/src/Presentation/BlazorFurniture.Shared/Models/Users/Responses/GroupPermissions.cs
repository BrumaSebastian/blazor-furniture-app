using BlazorFurniture.Shared.Models.Users;

namespace BlazorFurniture.Shared.Models.Users.Responses;

public sealed class GroupPermissions
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required GroupRoles Role { get; set; }
    public required IReadOnlyList<string> Permissions { get; init; }
}
