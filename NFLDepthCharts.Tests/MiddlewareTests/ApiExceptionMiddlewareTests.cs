using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NFLDepthCharts.API.Exceptions;
using NFLDepthCharts.API.Middleware;
using System.Text.Json;

namespace NFLDepthCharts.Tests.MiddlewareTests
{
    [TestFixture]
    public class ApiExceptionMiddlewareTests
    {
        private Mock<ILogger<ApiExceptionMiddleware>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<ApiExceptionMiddleware>>();
        }

        [Test]
        public async Task InvokeAsync_HandlesValidationException()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var middleware = new ApiExceptionMiddleware(
                next: (innerHttpContext) => throw new ValidationException("Validation failed"),
                _mockLogger.Object
            );

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var responseBody = await reader.ReadToEndAsync();
            var responseObject = JsonSerializer.Deserialize<ErrorResponse>(responseBody);

            Assert.Multiple(() =>
            {
                Assert.That(context.Response.StatusCode, Is.EqualTo(400));
                Assert.That(context.Response.ContentType, Is.EqualTo("application/json"));
                Assert.That(responseObject.Success, Is.False);
                Assert.That(responseObject.Message, Is.EqualTo("Validation failed"));
            });
        }

        [Test]
        public async Task InvokeAsync_HandlesGenericException()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var middleware = new ApiExceptionMiddleware(
                next: (innerHttpContext) => throw new Exception("Something went wrong"),
                _mockLogger.Object
            );

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var responseBody = await reader.ReadToEndAsync();
            var responseObject = JsonSerializer.Deserialize<ErrorResponse>(responseBody);

            Assert.Multiple(() =>
            {
                Assert.That(context.Response.StatusCode, Is.EqualTo(500));
                Assert.That(context.Response.ContentType, Is.EqualTo("application/json"));
                Assert.That(responseObject.Success, Is.False);
                Assert.That(responseObject.Message, Is.EqualTo("Internal server error. Please retry later."));
            });
        }

        [Test]
        public async Task InvokeAsync_DoesNotCatchExceptionWhenNoExceptionIsThrown()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var middlewareInvoked = false;

            var middleware = new ApiExceptionMiddleware(
                next: (innerHttpContext) =>
                {
                    middlewareInvoked = true;
                    return Task.CompletedTask;
                },
                _mockLogger.Object
            );

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.That(middlewareInvoked, Is.True);
            Assert.That(context.Response.StatusCode, Is.EqualTo(200)); // Default status code
        }

        [Test]
        public async Task InvokeAsync_LogsException()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var middleware = new ApiExceptionMiddleware(
                next: (innerHttpContext) => throw new Exception("Test exception"),
                _mockLogger.Object
            );

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Test exception")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)
                ),
                Times.Once
            );
        }
    }
}
