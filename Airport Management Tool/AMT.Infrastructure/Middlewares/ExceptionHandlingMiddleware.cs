using System;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace AMT.Infrastructure.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger logger)
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
            _logger.Error(ex, "An unhandled exception occurred while processing the request.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = new { errors = new[] { "An unexpected error occurred. Please try again later." } };
        context.Response.StatusCode = 500;
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
