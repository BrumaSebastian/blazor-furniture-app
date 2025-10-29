namespace BlazorFurniture.Shared.DTOs.Users.Responses;

public sealed class UserProfile
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}
