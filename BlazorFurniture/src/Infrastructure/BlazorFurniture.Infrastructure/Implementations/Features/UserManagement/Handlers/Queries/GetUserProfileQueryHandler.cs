using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Features.UserManagement.Queries;
using BlazorFurniture.Application.Features.UserManagement.Responses;

namespace BlazorFurniture.Infrastructure.Implementations.Features.UserManagement.Handlers.Queries;

internal class GetUserProfileQueryHandler : IQueryHandler<GetUserProfileQuery, UserProfileResponse>
{
    public async Task<UserProfileResponse> HandleAsync( GetUserProfileQuery query, CancellationToken cancellationToken = default )
    {
        var userProfile = new UserProfileResponse
        {
            Id = query.Id,
            Username = "JohnDoe",
            Email = ""
        };

        await Task.Delay(100, cancellationToken); // Simulate async operation

        return userProfile;
    }
}
