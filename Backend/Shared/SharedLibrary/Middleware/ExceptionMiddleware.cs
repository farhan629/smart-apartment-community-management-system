using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Shared.SharedLibrary.Exceptions;

namespace Shared.SharedLibrary.Middleware;

/// <summary>
/// Middleware for handling exceptions globally and returning standardized error responses.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
    /// </summary>
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to handle the request and catch exceptions.
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning(
                ex,
                "API exception occurred while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path
            );

            await HandleApiException(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception occurred while processing {Method} {Path}",
                context.Request.Method,
                context.Request.Path
            );

            await HandleUnknownException(context);
        }
    }

    /// <summary>
    /// Handles API exceptions and returns a formatted error response.
    /// </summary>
    private static Task HandleApiException(HttpContext context, ApiException ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = ex.StatusCode;

        var result = JsonSerializer.Serialize(new { status = ex.StatusCode, error = ex.Message });

        return context.Response.WriteAsync(result);
    }

    /// <summary>
    /// Handles unknown exceptions and returns a generic internal server error response.
    /// </summary>
    private static Task HandleUnknownException(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;

        var result = JsonSerializer.Serialize(new { status = 500, error = "Something went wrong" });

        return context.Response.WriteAsync(result);
    }
}

/// <summary>
/// Extension methods for registering the exception middleware.
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    /// <summary>
    /// Adds the global exception middleware to the application pipeline.
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}
