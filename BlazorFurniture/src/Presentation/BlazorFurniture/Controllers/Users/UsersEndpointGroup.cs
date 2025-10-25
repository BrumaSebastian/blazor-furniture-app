using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BlazorFurniture.Controllers.Users;

internal sealed class UsersEndpointGroup : Group
{
    public static string Name => "users";

    public UsersEndpointGroup() => Configure(Name, options =>
    {
        options.Description(d => d
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status502BadGateway));
        options.AuthSchemeNames?.AddRange([JwtBearerDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]);
    });
}
