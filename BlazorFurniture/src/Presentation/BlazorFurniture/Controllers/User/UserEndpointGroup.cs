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
            .Produces(401)
            .Produces(403)
            .Produces(502)
            .WithTags(Name));
        options.AuthSchemeNames?.AddRange([JwtBearerDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]);
    });
}
