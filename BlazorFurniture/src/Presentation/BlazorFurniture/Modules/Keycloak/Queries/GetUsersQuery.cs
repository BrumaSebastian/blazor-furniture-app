using BlazorFurniture.Common.Abstractions;
using BlazorFurniture.Modules.Keycloak.Models;

namespace BlazorFurniture.Modules.Keycloak.Queries;

[CacheableQuery(30)]
public record GetUsersQuery : IQuery<List<User>>;
