namespace BlazorFurniture.Application.Common.Models;

/// <summary>
/// Gets the value of the HTTP 'Location' header.
/// </summary>
public sealed class HttpHeaderLocationResult
{
    public string Location { get; set; } = string.Empty;
}
