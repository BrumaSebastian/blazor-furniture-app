using BlazorFurniture.Controllers.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace BlazorFurniture.Controllers.Authorization.Policies;

public static class GroupPolicies
{
    public const string GroupsResource = "groups";
    public const string GroupUsersResource = "group-users";

    // Group Policies
    public const string ListGroupsPolicy = "ListGroups";
    public const string ReadGroupPolicy = "ReadGroup";
    public const string CreateGroupPolicy = "CreateGroup";
    public const string UpdateGroupPolicy = "UpdateGroup";
    public const string DeleteGroupPolicy = "DeleteGroup";

    // Group Users Policies
    public const string ListGroupUsersPolicy = "ListGroupUsers";
    public const string ReadGroupUserPolicy = "ReadGroupUser";
    public const string AddGroupUserPolicy = "AddGroupUser";
    public const string RemoveGroupUserPolicy = "RemoveGroupUser";

    extension( AuthorizationOptions options )
    {
        public void RegisterGroupPolicies()
        {
            options.AddPolicy(ListGroupsPolicy, policy =>
                policy.Requirements.Add(new PermissionRequirement(GroupsResource, Scopes.List)));

            options.AddPolicy(ReadGroupPolicy, policy =>
                policy.Requirements.Add(new PermissionRequirement(GroupsResource, Scopes.Read)));

            options.AddPolicy(CreateGroupPolicy, policy =>
                policy.Requirements.Add(new PermissionRequirement(GroupsResource, Scopes.Create)));

            options.AddPolicy(ListGroupUsersPolicy, configurePolicy: policy =>
                policy.Requirements.Add(new PermissionWithClaimsRequirement(GroupsResource, Scopes.Create, [Claims.GroupId])));
        }
    }
}
