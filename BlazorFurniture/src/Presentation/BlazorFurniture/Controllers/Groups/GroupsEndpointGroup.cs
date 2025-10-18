using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace BlazorFurniture.Controllers.Groups;

internal sealed class GroupsEndpointGroup : Group
{
    public static string Name => "groups";

    public GroupsEndpointGroup() => Configure(Name, options =>
    {
        options.Description(d => d
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status502BadGateway));
        options.AuthSchemeNames?.AddRange([JwtBearerDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]);
    });
}
