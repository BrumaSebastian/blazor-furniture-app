using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Features.GroupManagement.Responses;

namespace BlazorFurniture.Application.Features.GroupManagement.Queries;

public record GetGroupsQuery : IQuery<List<GroupResponse>>;
