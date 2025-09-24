using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Features.UserManagement.Responses;

namespace BlazorFurniture.Application.Features.UserManagement.Queries;

public sealed record GetUserProfileQuery(Guid Id) : IQuery<UserProfileResponse>;
