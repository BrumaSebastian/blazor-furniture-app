namespace BlazorFurniture.Application.Features.UserManagement.Responses;

public sealed class GroupPermissionsResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Role { get; set; }
    public required List<string> Permissions { get; init; }
}
