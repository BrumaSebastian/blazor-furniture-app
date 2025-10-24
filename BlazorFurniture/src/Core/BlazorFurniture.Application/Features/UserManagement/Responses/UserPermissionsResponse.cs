namespace BlazorFurniture.Application.Features.UserManagement.Responses;

public sealed class UserPermissionsResponse
{
    public required string Role { get; init; }
    public required IReadOnlyList<string> Permissions { get; init; }
    public IReadOnlyList<GroupPermissionsResponse>? Groups { get; init; }
}
