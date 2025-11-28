namespace BlazorFurniture.Shared.Models.Users;

public sealed class UserModel
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public PlatformRoles Role { get; set; }
    public IEnumerable<Guid> Groups { get; set; } = [];
    public string? Avatar { get; set; }
}
