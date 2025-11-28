namespace BlazorFurniture.Shared.Models.Users;

public sealed class GroupUserModel
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string FullName { get => $"{FirstName} {LastName}"; }
    public GroupRoles Role { get; set; }
    public string? Avatar { get; set; }
}
