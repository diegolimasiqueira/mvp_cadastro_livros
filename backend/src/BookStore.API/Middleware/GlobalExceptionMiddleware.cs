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
            // Log estruturado com contexto completo
            _logger.LogError(ex, 
                "❌ Exceção não tratada capturada | Tipo: {ExceptionType} | Mensagem: {Message} | Path: {RequestPath} | Method: {RequestMethod} | IP: {RemoteIP} | UserAgent: {UserAgent}",
                ex.GetType().Name,
                ex.Message,
                context.Request.Path,
                context.Request.Method,
                context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                context.Request.Headers["User-Agent"].ToString());
            
            await HandleExceptionAsync(context, ex, _logger);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<GlobalExceptionMiddleware> logger)
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
                logger.LogWarning("⚠️ Recurso não encontrado: {Message}", notFoundEx.Message);
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
                logger.LogWarning("⚠️ Erro de validação: {Message} | Erros: {@Errors}", validationEx.Message, validationEx.Errors);
                break;

            case UnauthorizedException unauthorizedEx:
                statusCode = (int)HttpStatusCode.Unauthorized;
                response = new
                {
                    statusCode,
                    message = unauthorizedEx.Message,
                    type = "Unauthorized"
                };
                logger.LogWarning("⚠️ Tentativa de acesso não autorizado: {Message}", unauthorizedEx.Message);
                break;

            case BusinessException businessEx:
                statusCode = (int)HttpStatusCode.BadRequest;
                response = new
                {
                    statusCode,
                    message = businessEx.Message,
                    type = "Business"
                };
                logger.LogWarning("⚠️ Erro de regra de negócio: {Message}", businessEx.Message);
                break;

            default:
                statusCode = (int)HttpStatusCode.InternalServerError;
                response = new
                {
                    statusCode,
                    message = "An internal server error occurred",
                    type = "InternalError"
                };
                logger.LogError("🔥 Erro interno do servidor: {Message} | StackTrace: {StackTrace}", 
                    exception.Message, 
                    exception.StackTrace);
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

