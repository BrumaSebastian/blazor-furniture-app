using Microsoft.AspNetCore.Authorization;

namespace BlazorFurniture.Controllers.Authorization.Requirements;

public sealed record PermissionWithClaimsRequirement( string Resource, Scopes Scope, string[] Claims ) : IAuthorizationRequirement;
