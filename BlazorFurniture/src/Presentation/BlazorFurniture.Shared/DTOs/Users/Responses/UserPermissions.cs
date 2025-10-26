namespace BlazorFurniture.Shared.DTOs.Users.Responses;

public sealed class UserPermissionsResponse
{
    public required PlatformRoles Role { get; init; }
    public required IReadOnlyList<string> Permissions { get; init; }
    public IReadOnlyList<GroupPermissionsResponse>? Groups { get; init; }
}
