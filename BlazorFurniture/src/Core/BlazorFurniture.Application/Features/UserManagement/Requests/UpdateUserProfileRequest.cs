namespace BlazorFurniture.Application.Features.UserManagement.Requests;

public sealed class UpdateUserProfileRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Avatar { get; init; }
}
