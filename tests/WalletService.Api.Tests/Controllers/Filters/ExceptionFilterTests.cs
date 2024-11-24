using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WalletService.Api.Controllers;
using Moq;
using WalletService.Api.Controllers.Filters;
using System.Data.Common;
using System.Net.Sockets;
using Microsoft.AspNetCore.Routing;
namespace WalletService.Api.Tests.Controllers.Filters;

public class ExceptionFilterTests
{
    [Theory]
    [MemberData(nameof(ExceptionsWithErrorCodes))]
    public void ServiceExceptionFilterAttribute_OnException_ReturnsAppropriateHttpStatusCode(Exception exception,
        int expectedStatusCode)
    {
        // Arrange
        var actionContext = new ActionContext
        {
            HttpContext = new DefaultHttpContext(),
            ActionDescriptor = new ActionDescriptor(),
            RouteData = new RouteData()
        };

        var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = exception
        };

        var logger = new Mock<ILogger<WalletController>>();
        var exceptionFilter = new ExceptionFilter<WalletController>(logger.Object);

        // Act
        exceptionFilter.OnException(exceptionContext);

        // Assert
        Assert.True(exceptionContext.ExceptionHandled);
        Assert.Equal(expectedStatusCode, actionContext.HttpContext.Response.StatusCode);
    }

    #region Helpers

    private class TestDbException(bool isTransientSetting) : DbException
    {
        public override bool IsTransient => isTransientSetting;
    }

    public static TheoryData<Exception, int> ExceptionsWithErrorCodes => new()
        {
            { new Exception(), StatusCodes.Status500InternalServerError },
            { new SocketException(), StatusCodes.Status503ServiceUnavailable },
            { new ArgumentOutOfRangeException(), StatusCodes.Status400BadRequest },
            { new ArgumentNullException(), StatusCodes.Status400BadRequest },
            { new ArgumentException(), StatusCodes.Status400BadRequest },
            { new TestDbException(false), StatusCodes.Status500InternalServerError },
            { new TestDbException(true), StatusCodes.Status503ServiceUnavailable }
        };

    #endregion
}
