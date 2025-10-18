using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BlazorFurniture.Controllers.User;

internal sealed class UserEndpointGroup : Group
{
    public static string Name => "user";

    public UserEndpointGroup() => Configure(Name, options =>
    {
        options.Description(d => d
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status502BadGateway));
        options.AuthSchemeNames?.AddRange([JwtBearerDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]);
    });
}
