using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ResearchApps.Common.Constants;
using ResearchApps.Common.Exceptions;

namespace ResearchApps.Web.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        //_logger.LogError(exception, MessageConstants.UnhandledException);

        httpContext.Response.StatusCode = exception switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            RepoException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
        
        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Title = MessageConstants.UnhandledException,
                Detail = exception.Message,
            }
        });
    }
}