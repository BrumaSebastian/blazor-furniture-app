namespace BlazorFurniture.Shared.Models.Users.Responses;

public sealed class UserGroupMembershipModel
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required GroupRoles Role { get; set; }
}
