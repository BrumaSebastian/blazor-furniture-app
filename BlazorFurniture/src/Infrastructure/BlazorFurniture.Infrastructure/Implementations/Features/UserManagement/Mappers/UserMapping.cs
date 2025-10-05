using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Domain.Entities.Keycloak;

namespace BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;

internal static class UserMapping
{
    extension(UserRepresentation userRepresentation )
    {
        public UserProfileResponse ToUserProfile() => new()
        {
            Id = userRepresentation.Id,
            Username = userRepresentation.Username!,
            Email = userRepresentation.Email,
            FirstName = userRepresentation.FirstName,
            LastName = userRepresentation.LastName,
        };
    }
}
