using System;

namespace SyncMedia.Core.Services
{
    /// <summary>
    /// Centralized error handling service interface
    /// </summary>
    public interface IErrorHandler
    {
        /// <summary>
        /// Handle an exception and optionally rethrow it
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="context">Context information about where the error occurred</param>
        /// <param name="rethrow">Whether to rethrow the exception after handling</param>
        void HandleException(Exception exception, string context = "", bool rethrow = false);

        /// <summary>
        /// Handle an exception with a custom error message
        /// </summary>
        /// <param name="exception">The exception to handle</param>
        /// <param name="customMessage">Custom error message</param>
        /// <param name="context">Context information</param>
        /// <param name="rethrow">Whether to rethrow the exception after handling</param>
        void HandleException(Exception exception, string customMessage, string context = "", bool rethrow = false);

        /// <summary>
        /// Log an error without an exception
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="context">Context information</param>
        void LogError(string message, string context = "");

        /// <summary>
        /// Get the last error message
        /// </summary>
        string GetLastError();

        /// <summary>
        /// Clear the last error
        /// </summary>
        void ClearLastError();
    }
}
