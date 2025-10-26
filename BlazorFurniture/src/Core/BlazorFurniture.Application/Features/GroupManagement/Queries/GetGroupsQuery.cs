using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Responses;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Application.Features.GroupManagement.Responses;

namespace BlazorFurniture.Application.Features.GroupManagement.Queries;

public record GetGroupsQuery( GetGroupsRequest Request ) : IQuery<PaginatedResponse<GroupResponse>>;
