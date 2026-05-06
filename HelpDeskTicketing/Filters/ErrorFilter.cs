using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HelpDeskTicketing.Filters;

public class ErrorFilter :IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var statusCode = context.Exception switch
        {
            ArgumentException => 400,
            UnauthorizedAccessException => 401,
            NullReferenceException => 404,
            _ => 500
        };

        context.Result = new ContentResult()
        {
            Content = context.Exception.Message,
            StatusCode = statusCode
        };
    }
}