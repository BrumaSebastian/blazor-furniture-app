namespace BlazorFurniture.Application.Features.UserManagement.Responses;

public sealed class UserPermissionsResponse
{
    public required string Role { get; init; }
    public required List<string> Permissions { get; init; }
    public List<GroupPermissionsResponse>? Groups { get; init; }
}
