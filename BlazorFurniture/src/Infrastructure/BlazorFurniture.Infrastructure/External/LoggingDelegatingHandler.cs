using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BlazorFurniture.Infrastructure.External;

internal sealed class LoggingDelegatingHandler( ILogger<LoggingDelegatingHandler> logger ) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken )
    {
        var stopwatch = Stopwatch.StartNew();

        // Log request
        logger.LogInformation(
            "HTTP {Method} {Uri} - Starting request",
            request.Method,
            request.RequestUri);

        if (logger.IsEnabled(LogLevel.Debug) && request.Content is not null)
        {
            var requestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            logger.LogDebug(
                "HTTP {Method} {Uri} - Request Body: {Body}",
                request.Method,
                request.RequestUri,
                requestBody);
        }

        HttpResponseMessage? response = null;
        try
        {
            response = await base.SendAsync(request, cancellationToken);

            stopwatch.Stop();

            // Log response
            logger.LogInformation(
                "HTTP {Method} {Uri} - {StatusCode} {StatusCodeName} in {ElapsedMs}ms",
                request.Method,
                request.RequestUri,
                (int)response.StatusCode,
                response.StatusCode,
                stopwatch.ElapsedMilliseconds);

            if (logger.IsEnabled(LogLevel.Debug) && response.Content is not null)
            {
                var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogDebug(
                    "HTTP {Method} {Uri} - Response Body: {Body}",
                    request.Method,
                    request.RequestUri,
                    responseBody);
            }

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            logger.LogError(
                ex,
                "HTTP {Method} {Uri} - Request failed after {ElapsedMs}ms",
                request.Method,
                request.RequestUri,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
