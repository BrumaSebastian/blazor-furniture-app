namespace BlazorFurniture.Application.Features.UserManagement.Requests;

public sealed class UpdateUserProfileRequest
{
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
}
