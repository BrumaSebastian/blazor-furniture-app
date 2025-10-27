namespace BlazorFurniture.Shared.DTOs.Users.Responses;

public sealed class UserPermissions
{
    public required PlatformRoles Role { get; init; }
    public required IReadOnlyList<string> Permissions { get; init; }
    public IReadOnlyList<GroupPermissions>? Groups { get; init; }
}
