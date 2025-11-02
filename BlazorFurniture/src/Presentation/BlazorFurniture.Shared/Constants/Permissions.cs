namespace BlazorFurniture.Shared.Constants;

public static class Permissions
{
    public const string Prefix = "Permission:";

    public const string CREATE_GROUP = "create-group";
    public const string VIEW_GROUPS = "view-groups";
    public const string VIEW_USERS = "view-users";

    public static string WithPrefix( string permission ) => $"{Prefix}{permission}";
}
