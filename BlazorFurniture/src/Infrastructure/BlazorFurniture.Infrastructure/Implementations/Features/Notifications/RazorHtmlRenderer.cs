using BlazorFurniture.Application.Common.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlazorFurniture.Infrastructure.Implementations.Features.Notifications;

internal class RazorHtmlRenderer( IServiceScopeFactory scopeFactory ) : IRazorHtmlRenderer
{
    public async Task<string> RenderAsync<TComponent>( object? parameters = null, CancellationToken ct = default )
        => await RenderAsync(typeof(TComponent), ToDictionary(parameters), ct);

    public async Task<string> RenderAsync( Type componentType, IDictionary<string, object?>? parameters = null, CancellationToken ct = default )
    {
        ArgumentNullException.ThrowIfNull(componentType);

        // Use IServiceScopeFactory to create a new scope - this is always available
        using var scope = scopeFactory.CreateScope();
        await using var htmlRenderer = new HtmlRenderer(scope.ServiceProvider, NullLoggerFactory.Instance);

        var paramView = parameters is null
            ? ParameterView.Empty
            : ParameterView.FromDictionary(parameters);

        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            ct.ThrowIfCancellationRequested();
            var fragment = await htmlRenderer.RenderComponentAsync(componentType, paramView);
            return fragment.ToHtmlString();
        });

        return html;
    }

    private static Dictionary<string, object?>? ToDictionary( object? obj )
    {
        if (obj is null) return null;
        return obj.GetType()
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(obj));
    }
}
