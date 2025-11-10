using Microsoft.AspNetCore.Authorization;

namespace BlazorFurniture.Controllers.Authorization.Policies;

public static class GroupPolicies
{
    public const string GroupManagementResource = "groups";

    extension(AuthorizationOptions options )
    {
        public void RegisterGroupPolicies()
        {
            options.AddPolicy("CanManageGroups", policy =>
                policy.Requirements());
        }
    }
}
