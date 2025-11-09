using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Features.UserManagement.Requests;
using BlazorFurniture.Application.Features.UserManagement.Responses;

namespace BlazorFurniture.Application.Features.UserManagement.Queries;

public sealed record GetUserGroupsQuery( GetUserByIdRequest Request ) : IQuery<IEnumerable<UserGroupResponse>>;
