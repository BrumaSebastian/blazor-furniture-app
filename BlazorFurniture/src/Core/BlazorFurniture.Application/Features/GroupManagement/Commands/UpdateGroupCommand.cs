using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.GroupManagement.Requests;

namespace BlazorFurniture.Application.Features.GroupManagement.Commands;

public sealed record UpdateGroupCommand( UpdateGroupRequest Request ) : ICommand<Result<EmptyResult>>;
