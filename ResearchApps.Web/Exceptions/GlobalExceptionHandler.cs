using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Common.Exceptions;

namespace ResearchApps.Web.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, logLevel) = exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, LogLevel.Warning),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, LogLevel.Warning),
            KeyNotFoundException => (StatusCodes.Status404NotFound, LogLevel.Warning),
            RepoException => (StatusCodes.Status400BadRequest, LogLevel.Error),
            _ => (StatusCodes.Status500InternalServerError, LogLevel.Error)
        };
        
        _logger.Log(
            logLevel, 
            exception, 
            "{ExceptionType} occurred. Path: {Path}, User: {User}, TraceId: {TraceId}",
            exception.GetType().Name,
            httpContext.Request.Path,
            httpContext.User.Identity?.Name ?? "Anonymous",
            httpContext.TraceIdentifier);

        httpContext.Response.StatusCode = statusCode;
        
        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Type = exception.GetType().Name,
                Title = GetTitle(exception),
                Detail = httpContext.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment() 
                    ? exception.Message 
                    : "An error occurred processing your request.",
                Instance = httpContext.Request.Path
            }
        });
    }

    private static string GetTitle(Exception exception) => exception switch
    {
        ArgumentException => "Invalid Request",
        UnauthorizedAccessException => "Unauthorized",
        KeyNotFoundException => "Resource Not Found",
        RepoException => "Database Operation Failed",
        _ => MessageConstants.UnhandledException
    };
}