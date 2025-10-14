using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Infrastructure.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using System.Globalization;
using System.Reflection;

namespace BlazorFurniture.Infrastructure.Implementations.Features.Notifications;

internal class RazorHtmlRenderer(
    IServiceScopeFactory scopeFactory,
    [FromKeyedServices(KeyedServices.EMAIL_RESOURCE_MANAGER)] IResourceManager resourceManager )
    : IRazorHtmlRenderer
{
    public async Task<string> RenderHtml( object template, IReadOnlyDictionary<string, string> parameters, CultureInfo culture, CancellationToken ct )
    {
        ArgumentNullException.ThrowIfNull(template);

        if (template is not Type type || !typeof(IComponent).IsAssignableFrom(type))
            throw new ArgumentException($"Type parameter {template.GetType().Name} must implement IComponent.");

        ct.ThrowIfCancellationRequested();
        using var scope = scopeFactory.CreateScope();
        await using var htmlRenderer = new HtmlRenderer(scope.ServiceProvider, NullLoggerFactory.Instance);

        var parmetersObj = parameters.ToDictionary(kvp => kvp.Key, kvp => (object?)kvp.Value) ?? [];
        var parameterView = parmetersObj.Count == 0
            ? ParameterView.Empty
            : ParameterView.FromDictionary(parmetersObj);

        var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            ct.ThrowIfCancellationRequested();
            var prevCulture = CultureInfo.CurrentCulture;
            var prevUICulture = CultureInfo.CurrentUICulture;

            try
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
                ct.ThrowIfCancellationRequested();
                var componentType = type;
                var layoutFragment = CreateLayoutFragment(componentType, parmetersObj);
                var fragment = await htmlRenderer.RenderComponentAsync<LayoutView>(
                    ParameterView.FromDictionary(new Dictionary<string, object?>
                    {
                        ["Layout"] = componentType.GetCustomAttribute<LayoutAttribute>()?.LayoutType,
                        ["ChildContent"] = layoutFragment
                    }));
                return fragment.ToHtmlString();
            }
            finally
            {
                CultureInfo.CurrentCulture = prevCulture;
                CultureInfo.CurrentUICulture = prevUICulture;
            }
        });

        return html;
    }

    public string RenderSubject( object template, IReadOnlyDictionary<string, string> parameters, CultureInfo culture )
    {
        ArgumentNullException.ThrowIfNull(template);

        if (template is not Type type || !typeof(IComponent).IsAssignableFrom(type))
            throw new ArgumentException($"Type parameter {template.GetType().Name} must implement IComponent.");

        var key = $"{type.Name}_subject";

        return FormatTextWithParameters(GetTranslatedText(key, culture), parameters);
    }

    public string RenderText( object template, IReadOnlyDictionary<string, string> parameters, CultureInfo culture )
    {
        ArgumentNullException.ThrowIfNull(template);

        if (template is not Type type || !typeof(IComponent).IsAssignableFrom(type))
            throw new ArgumentException($"Type parameter {template.GetType().Name} must implement IComponent.");

        var key = $"{type.Name}_text";

        return FormatTextWithParameters(GetTranslatedText(key, culture), parameters);
    }

    private string GetTranslatedText( string key, CultureInfo culture )
    {
        return resourceManager.GetString(key, culture) ?? key;
    }

    private static string FormatTextWithParameters( string text, IReadOnlyDictionary<string, string> parameters )
    {
        if (parameters is null) return text;

        return string.Format(text, [.. parameters.Values]);
    }

    private static RenderFragment CreateLayoutFragment( Type componentType, Dictionary<string, object?> parameters )
    {
        return builder =>
        {
            builder.OpenComponent<DynamicComponent>(0);
            builder.AddAttribute(1, nameof(Type), componentType);
            builder.AddAttribute(2, "Parameters", parameters);
            builder.CloseComponent();
        };
    }
}
