using Microsoft.AspNetCore.Mvc.Filters;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;

namespace WalletService.Api.Controllers.Filters;

public class ExceptionFilter<TController>(ILogger<TController> logger) : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var exceptionType = context.Exception.GetBaseException().GetType();
        var isTransientDatabaseError = (context.Exception as DbException)?.IsTransient;

        var status = exceptionType switch
        {
            _ when exceptionType == typeof(ArgumentException) ||
                   exceptionType == typeof(BadHttpRequestException) ||
                   exceptionType == typeof(ArgumentNullException) ||
                   exceptionType == typeof(InvalidOperationException) ||
                   exceptionType == typeof(ArgumentOutOfRangeException)
                   => HttpStatusCode.BadRequest,
            _ when exceptionType == typeof(SocketException) ||
                   isTransientDatabaseError == true ||
                   exceptionType == typeof(HostAbortedException) => HttpStatusCode.ServiceUnavailable,
            _ => HttpStatusCode.InternalServerError
        };

        var message = context.Exception.Message;
        context.ExceptionHandled = true;

        var response = context.HttpContext.Response;
        response.StatusCode = (int)status;
        logger.LogError(context.Exception, $"An error occurred {message} {context.Exception.StackTrace}");
        response.WriteAsync(message);
    }
}
