using System;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SyncMedia.Core.Services;

namespace SyncMedia.Tests.Services
{
    public class ErrorHandlerTests
    {
        private readonly Mock<ILogger<ErrorHandler>> _mockLogger;
        private readonly ErrorHandler _errorHandler;

        public ErrorHandlerTests()
        {
            _mockLogger = new Mock<ILogger<ErrorHandler>>();
            _errorHandler = new ErrorHandler(_mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ErrorHandler(null!));
        }

        [Fact]
        public void HandleException_WithNullException_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _errorHandler.HandleException(null!));
        }

        [Fact]
        public void HandleException_ShouldLogError()
        {
            // Arrange
            var exception = new InvalidOperationException("Test error");
            
            // Act
            _errorHandler.HandleException(exception);
            
            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    exception,
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public void HandleException_WithContext_ShouldIncludeContextInMessage()
        {
            // Arrange
            var exception = new InvalidOperationException("Test error");
            var context = "TestContext";
            
            // Act
            _errorHandler.HandleException(exception, context: context);
            
            // Assert
            var lastError = _errorHandler.GetLastError();
            Assert.Contains(context, lastError);
            Assert.Contains(exception.Message, lastError);
        }

        [Fact]
        public void HandleException_ShouldStoreLastError()
        {
            // Arrange
            var exception = new InvalidOperationException("Test error");
            
            // Act
            _errorHandler.HandleException(exception);
            
            // Assert
            Assert.Equal(exception.Message, _errorHandler.GetLastError());
        }

        [Fact]
        public void HandleException_WithRethrow_ShouldRethrowException()
        {
            // Arrange
            var exception = new InvalidOperationException("Test error");
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => 
                _errorHandler.HandleException(exception, rethrow: true));
        }

        [Fact]
        public void HandleException_WithCustomMessage_ShouldUseCustomMessage()
        {
            // Arrange
            var exception = new InvalidOperationException("Original error");
            var customMessage = "Custom error message";
            
            // Act
            _errorHandler.HandleException(exception, customMessage: customMessage);
            
            // Assert
            Assert.Equal(customMessage, _errorHandler.GetLastError());
        }

        [Fact]
        public void HandleException_WithCustomMessageAndRethrow_ShouldThrowWithCustomMessage()
        {
            // Arrange
            var exception = new InvalidOperationException("Original error");
            var customMessage = "Custom error message";
            
            // Act & Assert
            var thrown = Assert.Throws<InvalidOperationException>(() => 
                _errorHandler.HandleException(exception, customMessage: customMessage, rethrow: true));
            
            Assert.Contains(customMessage, thrown.Message);
            Assert.Equal(exception, thrown.InnerException);
        }

        [Fact]
        public void LogError_ShouldLogErrorMessage()
        {
            // Arrange
            var message = "Test error message";
            
            // Act
            _errorHandler.LogError(message);
            
            // Assert
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
                    null,
                    It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public void LogError_ShouldStoreLastError()
        {
            // Arrange
            var message = "Test error message";
            
            // Act
            _errorHandler.LogError(message);
            
            // Assert
            Assert.Equal(message, _errorHandler.GetLastError());
        }

        [Fact]
        public void LogError_WithContext_ShouldIncludeContext()
        {
            // Arrange
            var message = "Test error message";
            var context = "TestContext";
            
            // Act
            _errorHandler.LogError(message, context);
            
            // Assert
            var lastError = _errorHandler.GetLastError();
            Assert.Contains(context, lastError);
            Assert.Contains(message, lastError);
        }

        [Fact]
        public void ClearLastError_ShouldClearStoredError()
        {
            // Arrange
            _errorHandler.LogError("Test error");
            
            // Act
            _errorHandler.ClearLastError();
            
            // Assert
            Assert.Empty(_errorHandler.GetLastError());
        }

        [Fact]
        public void GetLastError_WhenNoError_ShouldReturnEmptyString()
        {
            // Act
            var lastError = _errorHandler.GetLastError();
            
            // Assert
            Assert.Empty(lastError);
        }

        [Fact]
        public void HandleException_MultipleErrors_ShouldKeepLatestError()
        {
            // Arrange
            var firstError = new InvalidOperationException("First error");
            var secondError = new ArgumentException("Second error");
            
            // Act
            _errorHandler.HandleException(firstError);
            _errorHandler.HandleException(secondError);
            
            // Assert
            Assert.Equal(secondError.Message, _errorHandler.GetLastError());
        }
    }
}
