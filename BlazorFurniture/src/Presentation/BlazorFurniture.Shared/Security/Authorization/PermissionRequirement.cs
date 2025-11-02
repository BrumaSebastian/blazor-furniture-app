using Microsoft.AspNetCore.Authorization;

namespace BlazorFurniture.Shared.Security.Authorization;

public sealed class PermissionRequirement( string permission ) : IAuthorizationRequirement
{
    public string Permission { get; } = permission;
}
