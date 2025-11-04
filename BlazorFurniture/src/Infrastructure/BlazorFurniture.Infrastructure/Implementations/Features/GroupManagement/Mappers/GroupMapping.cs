using BlazorFurniture.Application.Features.GroupManagement.Responses;
using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Infrastructure.External.Keycloak.Utils;

namespace BlazorFurniture.Infrastructure.Implementations.Features.GroupManagement.Mappers;

internal static class GroupMapping
{
    extension( GroupRepresentation source )
    {
        public GroupResponse ToGroupResponse() => new()
        {
            Id = source.Id,
            Name = source.Name,
        };

        public DetailedGroupResponse ToDetailedGroupResponse() => new DetailedGroupResponse
        {
            Id = source.Id,
            Name = source.Name,
            Description = KeycloakAttributesHelper.GetAttributeValue(source.Attributes, GroupRepresentation.DESCRIPTION_ATTRIBUTE)
        };
    }

    extension( IEnumerable<GroupRepresentation> source )
    {
        public List<GroupResponse> ToGroupResponses() => source.Select(g => g.ToGroupResponse()).ToList();
    }

    extension( GroupRoleRepresentation source )
    {
        public GroupRoleResponse ToGroupRoleResponse() => new()
        {
            Id = source.Id,
            Role = source.Role,
        };
    }
}
