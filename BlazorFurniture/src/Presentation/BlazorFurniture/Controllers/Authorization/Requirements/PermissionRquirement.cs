using Microsoft.AspNetCore.Authorization;

namespace BlazorFurniture.Controllers.Authorization.Requirements;

public record PermissionRquirement(string Resource, string Scope) : IAuthorizationRequirement;
