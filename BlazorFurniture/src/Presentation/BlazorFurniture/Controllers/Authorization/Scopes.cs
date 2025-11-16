namespace BlazorFurniture.Controllers.Authorization;

/// <summary>
/// <c>Create</c>, <c>Read</c>, <c>Update</c>, <c>Delete</c>, <c>List</c> must be supported as per
/// <a href="https://docs.kantarainitiative.org/uma/wg/oauth-uma-federated-authz-2.0-09.html#reg-api">OAuth 2.0 UMA Federated Authorization</a>.
/// </summary>
public enum Scopes
{
    Undefined,
    Create,
    Read,
    Update,
    Delete,
    List,
    Add,
    Remove
}
