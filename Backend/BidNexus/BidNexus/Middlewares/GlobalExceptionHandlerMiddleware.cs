using System.Net;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace API.Middlewares;

public class GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger,
    IHostEnvironment environment)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;
    private readonly IHostEnvironment _environment = environment;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing request {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("The response has already started, cannot write exception response.");
            return;
        }

        context.Response.ContentType = "application/json";

        var (statusCode, responseBody) = exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                (object)new
                {
                    message = "Validation failed.",
                    errors = validationEx.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray())
                }),

            UnauthorizedAccessException unauthEx => (
                StatusCodes.Status403Forbidden,
                new { message = string.IsNullOrWhiteSpace(unauthEx.Message) ? "Forbidden." : unauthEx.Message }),

            KeyNotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                new { message = notFoundEx.Message }),

            ArgumentException argEx => (
                StatusCodes.Status400BadRequest,
                new { message = argEx.Message }),

            InvalidOperationException opEx => (
                StatusCodes.Status409Conflict,
                new { message = opEx.Message }),

            DbUpdateConcurrencyException => (
                StatusCodes.Status409Conflict,
                new { message = "The record was modified or deleted by another concurrent operation." }),

            DbUpdateException => (
                StatusCodes.Status409Conflict,
                new { message = "A database constraint or conflict occurred." }),

            _ => (
                StatusCodes.Status500InternalServerError,
                new
                {
                    message = _environment.IsDevelopment()
                        ? exception.Message
                        : "An internal server error occurred. Please try again later.",
                    details = _environment.IsDevelopment()
                        ? exception.ToString()
                        : (string?)null
                })
        };

        context.Response.StatusCode = statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(responseBody, jsonOptions));
    }
}
