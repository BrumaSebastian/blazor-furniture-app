using BlazorFurniture.Domain.Enums;

namespace BlazorFurniture.Application.Features.UserManagement.Responses;

public sealed class UserResponse
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public PlatformRoles Role { get; set; }
    public List<Guid> Groups { get; set; }
}
