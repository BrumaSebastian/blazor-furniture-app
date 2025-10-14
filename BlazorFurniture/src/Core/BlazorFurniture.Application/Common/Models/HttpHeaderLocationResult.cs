namespace BlazorFurniture.Application.Common.Models;

/// <summary>
/// Gets the value of the HTTP 'Location' header.
/// </summary>
public sealed class HttpHeaderLocationResult
{
    public Uri? Location { get; set; }

    public HttpHeaderLocationResult()
    {

    }

    public HttpHeaderLocationResult( Uri? location )
    {
        Location = location;
    }
}
