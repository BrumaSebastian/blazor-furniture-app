using BlazorFurniture.Client.Services.Interfaces;

namespace BlazorFurniture.Client.Services;

public sealed class SearchService : ISearchService
{
    public string SearchTerm
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnSearchChanged?.Invoke();
            }
        }
    } = string.Empty;

    public event Action? OnSearchChanged;
}
