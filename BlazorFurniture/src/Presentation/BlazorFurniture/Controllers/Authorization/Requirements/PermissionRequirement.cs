using Microsoft.AspNetCore.Authorization;

namespace BlazorFurniture.Controllers.Authorization.Requirements;

public record PermissionRequirement(string Resource, Scopes Scope) : IAuthorizationRequirement;
