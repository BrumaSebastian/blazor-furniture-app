using BlazorFurniture.Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorFurniture.Client.Components.Admin;

public abstract class AdminPageBase : ComponentBase
{
    [Inject] protected IBreadCrumbsService Breadcrumbs { get; init; } = default!;
}
