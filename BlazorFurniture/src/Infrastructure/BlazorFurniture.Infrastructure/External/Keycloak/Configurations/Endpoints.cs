namespace BlazorFurniture.Infrastructure.External.Keycloak.Configurations;

internal sealed class Endpoints( string Realm )
{
    private string BaseRealmUrl => $"realms/{Realm}";
    private string AdminRestApiBaseUrl => $"admin/realms/{Realm}";

    public string AccessToken() => $"{BaseRealmUrl}/protocol/openid-connect/token";

    // Users
    public string Users() => $"{AdminRestApiBaseUrl}/users";
    public string UsersExtension() => $"{Users()}-extension";
    public string UserById( Guid userId ) => $"{UsersExtension()}/{userId}";
    public string UserPermissions( Guid userId ) => $"{UserById(userId)}/permissions";

    // Groups
    public string Groups() => $"{AdminRestApiBaseUrl}/groups";
    public string GroupsExtension() => $"{Groups()}-extension";
    public string GroupsCount() => $"{GroupsExtension()}/count";
    public string GroupById( Guid id ) => $"{Groups()}/{id}";
    public string GroupByIdExtension( Guid id ) => $"{GroupsExtension()}/{id}";
    public string GroupMembersExtension( Guid groupId ) => $"{GroupByIdExtension(groupId)}/members";
    public string GroupMembersCount( Guid groupId ) => $"{GroupMembersExtension(groupId)}/count";
    public string GroupRoles( Guid groupId ) => $"{GroupByIdExtension(groupId)}/roles";
    public string GroupMemberByIdExtension( Guid groupId, Guid userId ) => $"{GroupMembersExtension(groupId)}/{userId}";
}
