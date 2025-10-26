using BlazorFurniture.Domain.Enums;

namespace BlazorFurniture.Application.Features.UserManagement.Responses;

public sealed class GroupPermissionsResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required GroupRoles Role { get; set; }
    public required IReadOnlyList<string> Permissions { get; init; }
}
