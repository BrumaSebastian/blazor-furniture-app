namespace BlazorFurniture.Client.Services.Interfaces;

public interface ISearchService
{
    string SearchTerm { get; set; }
    event Action? OnSearchChanged;
}
