using System.Security.Claims;

namespace BlazorFurniture.Shared.Extensions;

public static class ClaimsPrincipalExtensions
{
    extension( ClaimsPrincipal claims )
    {
        public Guid? GetUserId()
        {
            var userIdClaim = claims.FindFirst("sub")?.Value;

            if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            {
                return null;
            }

            return userId;
        }
    }
}
