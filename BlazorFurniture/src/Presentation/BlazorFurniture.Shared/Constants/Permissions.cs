namespace BlazorFurniture.Shared.Constants;

public static class Permissions
{
    public const string Prefix = "Permission:";

    public const string CREATE_GROUP = "groups-create";
    public const string VIEW_GROUPS = "groups-read";
    public const string VIEW_USERS = "users-list";
    public const string REMOVE_GROUP_MEMBER = "group-users-remove";
    public const string UPDATE_GROUP_MEMBER = "group-users-update";
    public const string DASHBOARD_MANAGEMENT = "dashboard-management";

    public static string GetPolicy( string permission ) => $"{Prefix}{permission}";
}
