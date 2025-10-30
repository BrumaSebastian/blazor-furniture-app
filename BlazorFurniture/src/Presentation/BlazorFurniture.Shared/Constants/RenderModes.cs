using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorFurniture.Shared.Constants;

public static class RenderModes
{
    public static IComponentRenderMode InteractiveWebAssemblyWithoutPrerendering { get; } =
    new InteractiveWebAssemblyRenderMode(prerender: false);

    public static IComponentRenderMode InteractiveAutoWithoutPrerendering { get; } =
        new InteractiveAutoRenderMode(prerender: false);

    public static IComponentRenderMode InteractiveServerWithoutPrerendering { get; } =
        new InteractiveServerRenderMode(prerender: false);
}
