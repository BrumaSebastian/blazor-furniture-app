namespace BlazorFurniture.Core.Shared.Models.Errors;

public abstract class BasicError( string title, string description )
{
    public string Title { get; } = title;

    public string Description { get; } = description;
}
