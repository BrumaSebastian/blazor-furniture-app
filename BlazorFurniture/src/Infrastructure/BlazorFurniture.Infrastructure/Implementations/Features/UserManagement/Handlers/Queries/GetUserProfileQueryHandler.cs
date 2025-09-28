using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Responses;
using BlazorFurniture.Core.Shared.Models.Errors;

namespace BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Queries;

internal class GetUserProfileQueryHandler : IQueryHandler<GetUserProfileQuery, UserProfileResponse> 
{
    public async Task<Result<UserProfileResponse>> HandleAsync( GetUserProfileQuery query, CancellationToken cancellationToken = default )
    {
        var userProfile = new UserProfileResponse
        {
            Id = query.Id,
            Username = "JohnDoe",
            Email = ""
        };

        await Task.Delay(100, cancellationToken); // Simulate async operation

        //return userProfile;
        //return new NotFoundError( "id12", userProfile );
        return new ConflictError( nameof(userProfile.Id), userProfile.Id.ToString(), userProfile);
    }
}
