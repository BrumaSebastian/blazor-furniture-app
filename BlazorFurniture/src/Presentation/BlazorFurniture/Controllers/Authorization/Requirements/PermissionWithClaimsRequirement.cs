using Microsoft.AspNetCore.Authorization;

namespace BlazorFurniture.Controllers.Authorization.Requirements;

public sealed record PermissionWithClaimsRequirement( string Resource, string Scope, string[] Claims ) : IAuthorizationRequirement;
