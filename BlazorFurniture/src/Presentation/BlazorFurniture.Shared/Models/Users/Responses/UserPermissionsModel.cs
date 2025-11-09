namespace BlazorFurniture.Shared.Models.Users.Responses;

public sealed class UserPermissionsModel
{
    public required PlatformRoles Role { get; init; }
    public required IReadOnlyList<string> Permissions { get; init; }
    public IReadOnlyList<GroupPermissionsModel>? Groups { get; init; }
}
