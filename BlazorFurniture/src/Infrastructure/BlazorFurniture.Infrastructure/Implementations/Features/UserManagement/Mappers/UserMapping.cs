using BlazorFurniture.Application.Features.GroupManagement.Responses;
using BlazorFurniture.Application.Features.UserManagement.Requests;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Domain.Entities.Keycloak;

namespace BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Mappers;

internal static class UserMapping
{
    extension( UserRepresentation source )
    {
        public UserProfileResponse ToResponse() => new()
        {
            Id = source.Id,
            Username = source.Username!,
            Email = source.Email,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Avatar = GetUserAvatar(source.Attributes)
        };
    }

    extension( ExtendedUserRepresentation source )
    {
        public UserResponse ToResponse() => new()
        {
            Id = source.Id,
            Email = source.Email!,
            FirstName = source.FirstName!,
            LastName = source.LastName!,
            Role = source.Role,
            Groups = source.Groups,
            Avatar = GetUserAvatar(source.Attributes)
        };
    }

    extension( UpdateUserProfileRequest source )
    {
        public UserRepresentation ToRepresentation() => new()
        {
            Username = "",
            Email = source.Email,
            FirstName = source.FirstName,
            LastName = source.LastName,
        };
    }

    extension( UserPermissionsRepresentation source )
    {
        public UserPermissionsResponse ToResponse() => new()
        {
            Role = source.Role,
            Permissions = source.Permissions,
            Groups = source.Groups?.Select(g => new GroupPermissionsResponse()
            {
                Id = g.Id,
                Name = g.Name,
                Role = g.Role,
                Permissions = g.Permissions,
            }).ToList() ?? [],
        };
    }

    extension( GroupUserRepresentation source )
    {
        public GroupUserResponse ToResponse() => new()
        {
            Id = source.Id,
            Email = source.Email!,
            FirstName = source.FirstName!,
            LastName = source.LastName!,
            Role = source.Role,
            Avatar = GetUserAvatar(source.Attributes)
        };
    }

    extension( UserGroupRepresentation source )
    {
        public UserGroupResponse ToResponse() => new()
        {
            Id = source.Id,
            Name = source.Name,
            Role = source.Role,
            Avatar = GetUserAvatar(source.Attributes),
        };
    }

    private static string? GetUserAvatar( Dictionary<string, List<string>>? keyValuePairs )
    {
        if (keyValuePairs is null)
        {
            return null;
        }

        return keyValuePairs.TryGetValue(UserRepresentation.ATTRIBUTE_AVATAR, out var avatars) && avatars.Count > 0
                ? avatars[0]
                : null;
    }
}
