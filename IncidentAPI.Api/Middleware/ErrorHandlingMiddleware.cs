using System.Net;
using System.Text.Json;

namespace IncidentAPI.Api.Middleware;

public class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> logger,
    IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }catch(Exception) when (context.Response.HasStarted)
        {
            throw;
        }
        catch(AppException ex)
        {
            logger.LogWarning(ex, "Business error in {Path}", context.Request.Path);
            await WriteResponseAsync(context, ex.StatusCode, ex.Message);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Unhandled error in {Method} {Path}", 
                context.Request.Method, context.Request.Path);

            await WriteResponseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "A server error occurred.",
                env.IsDevelopment() ? ex.Message : null
            );
        }
    }

    private static Task WriteResponseAsync(
            HttpContext context, int statusCode, string message, string? detail = null
        )
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(new
        {
            status = statusCode,
            message,
            detail
        });
    }
}