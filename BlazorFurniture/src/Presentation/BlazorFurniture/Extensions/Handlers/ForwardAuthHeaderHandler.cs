using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace BlazorFurniture.Extensions.Handlers;

public class ForwardAuthHeaderHandler( IHttpContextAccessor httpContextAccessor ) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken )
    {
        var context = httpContextAccessor.HttpContext;

        if (context is null)
        {
            return base.SendAsync(request, cancellationToken);
        }

        if (context.Request.Headers.TryGetValue(HeaderNames.Authorization, out var auth))
        {
            request.Headers.Remove(HeaderNames.Authorization);
            request.Headers.TryAddWithoutValidation(HeaderNames.Authorization, auth[0]);
        }
        else if (context.Request.Headers.TryGetValue(HeaderNames.Cookie, out StringValues cookie) && !StringValues.IsNullOrEmpty(cookie))
        {
            // Join multiple cookie header values into a single header value
            var cookieHeader = string.Join("; ", cookie!);
            request.Headers.Remove(HeaderNames.Cookie);
            request.Headers.TryAddWithoutValidation(HeaderNames.Cookie, cookieHeader);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
