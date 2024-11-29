using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace ProductControl.Infrastracture.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex); 
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var env = context.RequestServices.GetService<IWebHostEnvironment>();
        var showDetails = env?.IsDevelopment() ?? false;

        var problemDetails = new ProblemDetails
        {
            Status = (int)HttpStatusCode.InternalServerError,
            Title = "An error occurred.",
            Detail = showDetails ? exception.Message : null,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}