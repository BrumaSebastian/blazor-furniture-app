namespace BlazorFurniture.Application.Common.Interfaces;

public interface IRazorHtmlRenderer
{
    Task<string> RenderAsync<TComponent>( object? parameters = null, CancellationToken ct = default );
    Task<string> RenderAsync( Type componentType, IDictionary<string, object?>? parameters = null, CancellationToken ct = default );
}
