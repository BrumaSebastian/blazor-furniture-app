using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Templates.Resources;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace BlazorFurniture.Infrastructure.Implementations.Features.Notifications;

internal class EmailResourceManager( ILogger<EmailResourceManager> logger ) : IResourceManager
{
    public string? GetString( string key, CultureInfo culture )
    {
        var text = EmailResource.ResourceManager.GetString(key, culture);

        if (text is null)
        {
            logger.LogWarning("Email resource with key {Key} not found for culture {Culture}", key, culture);
        }

        return text;
    }
}
