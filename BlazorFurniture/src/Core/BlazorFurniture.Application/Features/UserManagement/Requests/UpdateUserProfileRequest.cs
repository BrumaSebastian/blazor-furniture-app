using BlazorFurniture.Application.Common.Requests.RouteParams;

namespace BlazorFurniture.Application.Features.UserManagement.Requests;

public sealed class UpdateUserProfileRequest : IGuidParam
{
    public Guid Id { get; set; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Email { get; init; }
}
