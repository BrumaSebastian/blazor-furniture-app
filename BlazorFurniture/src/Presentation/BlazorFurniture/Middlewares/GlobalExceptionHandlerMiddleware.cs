using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BlazorFurniture.Middlewares;

public class GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger,
    IWebHostEnvironment webHostEnvironment )
{
    public async Task Invoke( HttpContext context )
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            LogError(context, ex);
            await HandlException(context, ex);
        }
    }

    private void LogError( HttpContext context, Exception ex )
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        switch (ex)
        {
            default:
                logger.LogError(ex, "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
                    context.Request.Method, context.Request.Path, traceId);
                break;
        }
    }

    private Task HandlException( HttpContext context, Exception ex )
    {
        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = MapStatusCode(ex);
        var problemDetails = CreateProblemDetails(ex, context.Response.StatusCode);

        return context.Response.WriteAsJsonAsync(problemDetails, context.RequestAborted);
    }

    private ProblemDetails CreateProblemDetails( Exception ex, int statusCode )
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = "Unexpected Error",
            Detail = MapDetails(ex)
        };

        if (webHostEnvironment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = ex.GetType().FullName;
            problemDetails.Extensions["exceptionMessage"] = ex.Message;
            problemDetails.Extensions["stackTrace"] = ex.StackTrace;
            problemDetails.Extensions["innerException"] = ex.InnerException?.ToString();
        }

        return problemDetails;
    }

    private static string MapDetails( Exception ex )
    {
        return "Not yet implemented";
    }

    private static int MapStatusCode( Exception ex )
    {
        return ex switch
        {
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
