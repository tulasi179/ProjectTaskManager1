using System.Net;
using System.Text.Json;

namespace Projecttaskmanager.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            KeyNotFoundException ex => new ErrorResponse(
                (int)HttpStatusCode.NotFound,
                ex.Message),

            UnauthorizedAccessException ex => new ErrorResponse(
                (int)HttpStatusCode.Unauthorized,
                ex.Message),

            ArgumentException ex => new ErrorResponse(
                (int)HttpStatusCode.BadRequest,
                ex.Message),

            _ => new ErrorResponse(
                (int)HttpStatusCode.InternalServerError,
                "Something went wrong. Please try again later.")
        };

        context.Response.StatusCode = response.StatusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

public record ErrorResponse(int StatusCode, string Message);