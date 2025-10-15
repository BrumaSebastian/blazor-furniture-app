using BlazorFurniture.Application.Features.GroupManagement.Responses;
using BlazorFurniture.Domain.Entities.Keycloak;

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
    }

    extension( IEnumerable<GroupRepresentation> source )
    {
        public List<GroupResponse> ToGroupResponses() => source.Select(g => g.ToGroupResponse()).ToList();
    }
}
