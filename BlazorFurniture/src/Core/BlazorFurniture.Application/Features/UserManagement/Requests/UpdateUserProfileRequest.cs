namespace BlazorFurniture.Application.Features.UserManagement.Requests;

public class UpdateUserProfileRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
}
