using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SampleWebApi.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace SampleWebApi.Exceptions
{
    public class BusinessRuleException : Exception
    {
        public int StatusCode { get; }

        public BusinessRuleException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

}

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
        : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            BusinessRuleException br => br.StatusCode,
            _ => StatusCodes.Status500InternalServerError
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Title = "Error has occured",
                Detail = exception.Message
            }
        });
    }
}




