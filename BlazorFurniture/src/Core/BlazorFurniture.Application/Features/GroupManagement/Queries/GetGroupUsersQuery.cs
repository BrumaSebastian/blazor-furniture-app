using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Responses;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Application.Features.GroupManagement.Responses;

namespace BlazorFurniture.Application.Features.GroupManagement.Queries;

public sealed record GetGroupUsersQuery( GetGroupUsersRequest Request ) : IQuery<PaginatedResponse<GroupUserResponse>>;
