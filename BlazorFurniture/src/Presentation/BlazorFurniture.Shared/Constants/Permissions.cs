namespace BlazorFurniture.Shared.Constants;

public static class Permissions
{
    public const string Prefix = "Permission:";

    public const string CREATE_GROUP = "groups-create";
    public const string VIEW_GROUPS = "groups-read";
    public const string VIEW_USERS = "users-list";

    public static string WithPrefix( string permission ) => $"{Prefix}{permission}";
}
