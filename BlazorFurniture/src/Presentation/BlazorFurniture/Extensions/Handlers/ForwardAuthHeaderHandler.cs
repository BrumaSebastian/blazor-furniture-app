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

        if (context.Request.Headers.TryGetValue(HeaderNames.Cookie, out StringValues cookie)
            && !StringValues.IsNullOrEmpty(cookie))
        {
            var cookieHeader = string.Join("; ", cookie!);
            request.Headers.Remove(HeaderNames.Cookie);
            request.Headers.TryAddWithoutValidation(HeaderNames.Cookie, cookieHeader);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
