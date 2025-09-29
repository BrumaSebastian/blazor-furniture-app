using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Infrastructure.External.Interfaces;

namespace BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Queries;

internal class GetUserProfileQueryHandler(IUserManagementClient userManagementClient) : IQueryHandler<GetUserProfileQuery, UserProfileResponse> 
{
    public async Task<Result<UserProfileResponse>> HandleAsync( GetUserProfileQuery query, CancellationToken cancellationToken = default )
    {
        var result = await userManagementClient.Get(query.Id, cancellationToken);

        return result ? Result<UserProfileResponse>.Succeeded(new UserProfileResponse
        {
            Id = result.Value.Id,
            Username = result.Value.Username!,
            Email = result.Value.Email,
            FirstName = result.Value.FirstName,
            LastName = result.Value.LastName,
        }) : Result<UserProfileResponse>.Failed(result.Error!);
    }
}
