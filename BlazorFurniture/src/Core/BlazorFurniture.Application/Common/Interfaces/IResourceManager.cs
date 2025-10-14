using System.Globalization;

namespace BlazorFurniture.Application.Common.Interfaces;

public interface IResourceManager
{
    string? GetString( string key, CultureInfo culture );
}
