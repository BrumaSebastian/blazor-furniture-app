using MudBlazor;

namespace BlazorFurniture.Client.Services.Interfaces;

public interface IBreadCrumbsService
{
    IReadOnlyList<BreadcrumbItem> Items { get; }
    event Action? Changed;
    void Set( params BreadcrumbItem[] items );
    void Add( BreadcrumbItem item );
    void Clear();
}
