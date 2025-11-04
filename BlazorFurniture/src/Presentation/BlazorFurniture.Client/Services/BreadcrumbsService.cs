using BlazorFurniture.Client.Services.Interfaces;
using MudBlazor;

namespace BlazorFurniture.Client.Services;

public class BreadcrumbsService : IBreadCrumbsService
{
    private readonly List<BreadcrumbItem> items = [];
    public IReadOnlyList<BreadcrumbItem> Items => items;

    public event Action? Changed;

    public void Set( params BreadcrumbItem[] items )
    {
        this.items.Clear();
        this.items.AddRange(items);
        Changed?.Invoke();
    }

    public void Clear()
    {
        if (items.Count == 0) return;
        items.Clear();
        Changed?.Invoke();
    }

    public void Add( BreadcrumbItem item )
    {
        items.Add(item);
        Changed?.Invoke();
    }
}
