using System.Globalization;

namespace BlazorFurniture.Application.Common.Interfaces;

public interface IRazorHtmlRenderer
{
    Task<string> RenderHtml( object template, IReadOnlyDictionary<string, string> parameters, CultureInfo culture, CancellationToken ct );
    string RenderSubject( object template, IReadOnlyDictionary<string, string> parameters, CultureInfo culture );
    string RenderText( object template, IReadOnlyDictionary<string, string> parameters, CultureInfo culture );
}
