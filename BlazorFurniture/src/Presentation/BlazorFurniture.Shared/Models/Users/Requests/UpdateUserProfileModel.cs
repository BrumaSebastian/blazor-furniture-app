namespace BlazorFurniture.Shared.Models.Users.Requests;

public sealed class UpdateUserProfileModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Avatar { get; set; }
}
