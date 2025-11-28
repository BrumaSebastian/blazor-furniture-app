using BlazorFurniture.Domain.Enums;

namespace BlazorFurniture.Application.Features.GroupManagement.Responses;

public sealed class GroupUserResponse
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public GroupRoles Role { get; set; }
    public string? Avatar { get; set; }
}
