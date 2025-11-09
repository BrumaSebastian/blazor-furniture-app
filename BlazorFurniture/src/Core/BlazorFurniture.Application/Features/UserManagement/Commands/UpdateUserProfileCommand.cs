using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Requests;

namespace BlazorFurniture.Application.Features.UserManagement.Commands;

public sealed record UpdateUserProfileCommand( Guid UserId, UpdateUserProfileRequest Request )
    : ICommand<EmptyResult>;
