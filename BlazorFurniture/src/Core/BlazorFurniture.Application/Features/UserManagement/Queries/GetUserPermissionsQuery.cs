﻿using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Features.UserManagement.Responses;

namespace BlazorFurniture.Application.Features.UserManagement.Queries;

public sealed record GetUserPermissionsQuery( Guid Id ) : IQuery<UserPermissionsResponse>;
