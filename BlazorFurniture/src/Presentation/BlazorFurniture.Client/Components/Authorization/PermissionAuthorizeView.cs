using BlazorFurniture.Shared.Services.Security.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorFurniture.Client.Components.Authorization;

public partial class PermissionAuthorizeView : AuthorizeView
{
    [Inject] public required IPermissionsService PermissionService { get; init; }
    [Inject] public required AuthenticationStateProvider AuthStateProvider { get; init; }

    [Parameter, EditorRequired] public string Permission { get; set; } = string.Empty;
    [Parameter] public Guid? GroupId { get; set; }
    [Parameter] public string? Context { get; set; }

    private bool hasPermission;
    private AuthenticationState? authState;

    protected override async Task OnParametersSetAsync()
    {
        if (ChildContent != null && Authorized != null)
        {
            throw new InvalidOperationException($"Do not specify both '{nameof(Authorized)}' and '{nameof(ChildContent)}'.");
        }

        authState = await AuthStateProvider.GetAuthenticationStateAsync();

        if (!authState.User.Identity?.IsAuthenticated ?? true)
        {
            hasPermission = false;
            return;
        }

        hasPermission = GroupId.HasValue
             ? await PermissionService.HasGroupPermission(Permission, GroupId.Value)
             : await PermissionService.HasPermission(Permission);
    }

    protected override void BuildRenderTree( RenderTreeBuilder builder )
    {
        if (authState is null)
        {
            builder.AddContent(0, Authorizing);
        }
        else if (hasPermission)
        {
            var authorizedContent = Authorized ?? ChildContent;
            builder.AddContent(1, authorizedContent?.Invoke(authState));
        }
        else
        {
            builder.AddContent(2, NotAuthorized?.Invoke(authState));
        }
    }
}
