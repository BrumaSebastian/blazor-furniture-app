namespace BlazorFurniture.Infrastructure.External.Keycloak.Utils;

internal static class UmaAuthorizationConstants
{
    public const string UMA_TICKET_GRANT_TYPE = "urn:ietf:params:oauth:grant-type:uma-ticket";
    public const string UMA_RESPONSE_MODE_DECISION = "decision";
    public const string UMA_RESPONSE_MODE_PERMISSIONS = "permissions";
    public const string UMA_PERMISSION_PARAM = "permission";
    public const string AUDIENCE_PARAM = "audience";
}
