using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Features.GroupManagement.Requests;
using BlazorFurniture.Application.Features.GroupManagement.Responses;

namespace BlazorFurniture.Application.Features.GroupManagement.Queries;

public sealed record GetGroupQuery(GetGroupByIdRequest Request) : IQuery<DetailedGroupResponse>;
