using BioWings.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BioWings.UI.Exceptions;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "An unhandled exception occurred: {ExceptionType} - {Message}", exception.GetType().Name, exception.Message);
        
        // WebUI için özel hata yönlendirmeleri
        switch (exception)
        {
            case ValidationException:
                httpContext.Response.Redirect("/Error/ValidationError");
                break;
            case NotFoundException:
                httpContext.Response.Redirect("/Error/NotFoundd");
                break;
            case NullEntityException:
                httpContext.Response.Redirect("/Error/BadRequest");
                break;
            case UnauthorizedAccessException:
                httpContext.Response.Redirect("/Error/Unauthorized");
                break;
            default:
                httpContext.Response.Redirect("/Error/InternalServerError");
                break;
        }
        
        return true;
    }
} 