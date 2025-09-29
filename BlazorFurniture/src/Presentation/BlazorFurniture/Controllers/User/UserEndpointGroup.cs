using FastEndpoints;

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
    });
}
