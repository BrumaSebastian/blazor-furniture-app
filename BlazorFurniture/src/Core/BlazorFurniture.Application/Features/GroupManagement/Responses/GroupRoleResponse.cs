using BlazorFurniture.Domain.Enums;

namespace BlazorFurniture.Application.Features.GroupManagement.Responses;

public sealed class GroupRoleResponse
{
    public Guid Id { get; set; }
    public GroupRoles Role { get; set; }
}
