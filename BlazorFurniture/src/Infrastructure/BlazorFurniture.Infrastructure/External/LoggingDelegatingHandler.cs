using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BlazorFurniture.Infrastructure.External;

internal sealed class LoggingDelegatingHandler( ILogger<LoggingDelegatingHandler> logger ) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken )
    {
        var sw = Stopwatch.StartNew();
        try
        {
            logger.LogInformation("HTTP {Method} {Uri} - Starting request", request.Method, request.RequestUri);

            var response = await base.SendAsync(request, cancellationToken);

            sw.Stop();
            logger.LogInformation("HTTP {Method} {Uri} - {StatusCode} {Reason} in {Elapsed}ms",
                request.Method, request.RequestUri, (int)response.StatusCode, response.ReasonPhrase, sw.ElapsedMilliseconds);

            return response;
        }
        catch (OperationCanceledException oce) when (cancellationToken.IsCancellationRequested)
        {
            sw.Stop();
            // Request was intentionally canceled (e.g., client navigated away)
            logger.LogDebug(oce, "HTTP {Method} {Uri} - Canceled after {Elapsed}ms",
                request.Method, request.RequestUri, sw.ElapsedMilliseconds);
            throw;
        }
        catch (TaskCanceledException tce)
        {
            sw.Stop();
            // Likely a timeout (token not canceled)
            logger.LogWarning(tce, "HTTP {Method} {Uri} - Timed out after {Elapsed}ms",
                request.Method, request.RequestUri, sw.ElapsedMilliseconds);
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(ex, "HTTP {Method} {Uri} - Request failed after {Elapsed}ms",
                request.Method, request.RequestUri, sw.ElapsedMilliseconds);
            throw;
        }
    }
}
