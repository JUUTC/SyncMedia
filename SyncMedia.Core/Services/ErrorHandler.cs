using System;
using Microsoft.Extensions.Logging;

namespace SyncMedia.Core.Services
{
    /// <summary>
    /// Centralized error handling service implementation
    /// </summary>
    public class ErrorHandler : IErrorHandler
    {
        private readonly ILogger<ErrorHandler> _logger;
        private string _lastError = string.Empty;

        /// <summary>
        /// Initializes a new instance of the ErrorHandler class
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public ErrorHandler(ILogger<ErrorHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handle an exception and optionally rethrow it
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="context">Context information about where the error occurred</param>
        /// <param name="rethrow">Whether to rethrow the exception after handling</param>
        public void HandleException(Exception exception, string context = "", bool rethrow = false)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var errorMessage = string.IsNullOrEmpty(context)
                ? exception.Message
                : $"{context}: {exception.Message}";

            _lastError = errorMessage;
            _logger.LogError(exception, errorMessage);

            if (rethrow)
            {
                throw exception;
            }
        }

        /// <summary>
        /// Handle an exception with a custom error message
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="customMessage">Custom error message</param>
        /// <param name="context">Context information</param>
        /// <param name="rethrow">Whether to rethrow the exception after handling</param>
        public void HandleException(Exception exception, string customMessage, string context = "", bool rethrow = false)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            var fullMessage = string.IsNullOrEmpty(context)
                ? customMessage
                : $"{context}: {customMessage}";

            _lastError = fullMessage;
            _logger.LogError(exception, fullMessage);

            if (rethrow)
            {
                throw new InvalidOperationException(fullMessage, exception);
            }
        }

        /// <summary>
        /// Log an error without an exception
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="context">Context information</param>
        public void LogError(string message, string context = "")
        {
            var errorMessage = string.IsNullOrEmpty(context)
                ? message
                : $"{context}: {message}";

            _lastError = errorMessage;
            _logger.LogError(errorMessage);
        }

        /// <summary>
        /// Get the last error message
        /// </summary>
        public string GetLastError()
        {
            return _lastError;
        }

        /// <summary>
        /// Clear the last error
        /// </summary>
        public void ClearLastError()
        {
            _lastError = string.Empty;
        }
    }
}
