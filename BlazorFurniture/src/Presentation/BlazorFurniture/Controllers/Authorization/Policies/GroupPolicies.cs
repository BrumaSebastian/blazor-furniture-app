using BlazorFurniture.Controllers.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace BlazorFurniture.Controllers.Authorization.Policies;

public static class GroupPolicies
{
    public const string GroupsResource = "groups";
    public const string GroupUsersResource = "group-users";

    // Group Policies
    public const string ListGroupsPolicy = "ListGroups";
    public const string ViewGroupsPolicy = "ViewGroup";
    public const string CreateGroupPolicy = "CreateGroup";
    public const string UpdateGroupPolicy = "UpdateGroup";
    public const string DeleteGroupPolicy = "DeleteGroup";

    // Group Users Policies
    public const string ListGroupUsersPolicy = "ListGroupUsers";
    public const string ViewGroupUserPolicy = "ViewGroupUser";
    public const string AddGroupUserPolicy = "AddGroupUser";
    public const string RemoveGroupUserPolicy = "RemoveGroupUser";

    extension(AuthorizationOptions options )
    {
        public void RegisterGroupPolicies()
        {
            options.AddPolicy(ListGroupsPolicy, policy =>
                policy.Requirements.Add(new PermissionRequirement(GroupsResource, Scopes.List)));
        }
    }
}
