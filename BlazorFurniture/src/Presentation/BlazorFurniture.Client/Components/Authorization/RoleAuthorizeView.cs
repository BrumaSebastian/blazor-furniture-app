using BlazorFurniture.Shared.Models.Users;
using BlazorFurniture.Shared.Services.Security.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorFurniture.Client.Components.Authorization;

public partial class RoleAuthorizeView : AuthorizeView
{
    [Inject] public required IPermissionsService PermissionService { get; init; }
    [Inject] public required AuthenticationStateProvider AuthStateProvider { get; init; }

    [Parameter] public Guid? GroupId { get; set; }
    [Parameter] public PlatformRoles? PlatformRole { get; set; }
    [Parameter] public GroupRoles? GroupRole { get; set; }
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

        if (PlatformRole is not null)
        {
            hasPermission = await PermissionService.HasPlatformRole(PlatformRole.Value);
        }
        
        if (!hasPermission && GroupRole is not null)
        {
            if (!GroupId.HasValue)
            {
                throw new InvalidOperationException($"Specify Group Id.");
            }

            hasPermission = await PermissionService.HasGroupRole(GroupRole.Value, GroupId.Value);
        }
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
