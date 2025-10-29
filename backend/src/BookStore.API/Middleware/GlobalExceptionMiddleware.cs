using System.Net;
using System.Text.Json;
using BookStore.Application.Exceptions;

namespace BookStore.API.Middleware;

/// <summary>
/// Middleware global para tratamento de exceções
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        int statusCode;
        object response;

        switch (exception)
        {
            case NotFoundException notFoundEx:
                statusCode = (int)HttpStatusCode.NotFound;
                response = new
                {
                    statusCode,
                    message = notFoundEx.Message,
                    type = "NotFound"
                };
                break;

            case ValidationException validationEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    statusCode,
                    message = validationEx.Message,
                    errors = validationEx.Errors,
                    type = "Validation"
                };
                break;

            case UnauthorizedException unauthorizedEx:
                statusCode = (int)HttpStatusCode.Unauthorized;
                response = new
                {
                    statusCode,
                    message = unauthorizedEx.Message,
                    type = "Unauthorized"
                };
                break;

            case BusinessException businessEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    statusCode,
                    message = businessEx.Message,
                    type = "Business"
                };
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    statusCode,
                    message = "An internal server error occurred",
                    type = "InternalError"
                };
                break;
        }

        context.Response.StatusCode = statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        await context.Response.WriteAsync(json);
    }
}

