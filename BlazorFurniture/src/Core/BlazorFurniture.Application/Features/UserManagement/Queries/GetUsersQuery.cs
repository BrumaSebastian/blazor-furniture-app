using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Responses;
using BlazorFurniture.Application.Features.UserManagement.Requests;
using BlazorFurniture.Application.Features.UserManagement.Responses;

namespace BlazorFurniture.Application.Features.UserManagement.Queries;

public sealed record GetUsersQuery(GetUsersRequest Request) : IQuery<PaginatedResponse<UserResponse>>;
