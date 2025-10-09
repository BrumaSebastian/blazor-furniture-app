using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlazorFurniture.Extensions;

public static class HttpContextExtensions
{
    extension( HttpContext httpContext )
    {
        public Guid GetUserIdFromClaims()
        {
            if (!httpContext.User.Identity?.IsAuthenticated ?? true)
            {
                // TODO: create custom exceptions
                throw new Exception("User should be authenticated");
            }

            return Guid.Parse(httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? throw new Exception("Token has no sub claim"));
        }
    }
}
