namespace BlazorFurniture.Application.DTOs.Users.Responses;

public class UserProfileResponse
{
    public required string Id { get; init; }
    public required string Username { get; init; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
