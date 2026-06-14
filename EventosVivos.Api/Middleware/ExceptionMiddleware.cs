using EventosVivos.Domain.Exceptions;
using FluentValidation;
using System.Net;
using System.Text.Json;

namespace EventosVivos.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => e.ErrorMessage);
            await WriteResponse(context, HttpStatusCode.BadRequest, new { errors });
        }
        catch (DomainException ex)
        {
            await WriteResponse(context, HttpStatusCode.UnprocessableEntity,
                new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado");
            await WriteResponse(context, HttpStatusCode.InternalServerError,
                new { error = "Ocurrió un error interno." });
        }
    }

    private static async Task WriteResponse(HttpContext context, HttpStatusCode code, object body)
    {
        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(body));
    }
}