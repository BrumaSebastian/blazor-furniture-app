namespace BlazorFurniture.Application.Features.GroupManagement.Requests;

public sealed record AddUserToGroupRequest( Guid GroupId, Guid UserId );
