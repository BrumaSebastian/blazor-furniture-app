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
    public const string UpdateGroupUserPolicy = "UpdateGroupUser";

    extension( AuthorizationOptions options )
    {
        public void RegisterGroupPolicies()
        {
            options.AddPolicy(ListGroupsPolicy, policy =>
                policy.Requirements.Add(new PermissionRequirement(GroupsResource, Scopes.List)));

            options.AddPolicy(CreateGroupPolicy, policy =>
                policy.Requirements.Add(new PermissionRequirement(GroupsResource, Scopes.Create)));

            options.AddPolicy(ReadGroupPolicy, policy =>
                policy.Requirements.Add(new PermissionWithClaimsRequirement(GroupsResource, Scopes.Read, [Claims.GroupId])));

            options.AddPolicy(UpdateGroupPolicy, policy =>
                policy.Requirements.Add(new PermissionWithClaimsRequirement(GroupsResource, Scopes.Update, [Claims.GroupId])));

            options.AddPolicy(ReadGroupUserPolicy, configurePolicy: policy =>
                policy.Requirements.Add(new PermissionWithClaimsRequirement(GroupUsersResource, Scopes.Read, [Claims.GroupId])));

            options.AddPolicy(ListGroupUsersPolicy, configurePolicy: policy =>
                policy.Requirements.Add(new PermissionWithClaimsRequirement(GroupUsersResource, Scopes.List, [Claims.GroupId])));

            options.AddPolicy(AddGroupUserPolicy, configurePolicy: policy =>
                policy.Requirements.Add(new PermissionWithClaimsRequirement(GroupUsersResource, Scopes.Add, [Claims.GroupId])));

            options.AddPolicy(RemoveGroupUserPolicy, configurePolicy: policy =>
                policy.Requirements.Add(new PermissionWithClaimsRequirement(GroupUsersResource, Scopes.Remove, [Claims.GroupId])));

            options.AddPolicy(UpdateGroupUserPolicy, configurePolicy: policy =>
                policy.Requirements.Add(new PermissionWithClaimsRequirement(GroupUsersResource, Scopes.Update, [Claims.GroupId])));
        }
    }
}
