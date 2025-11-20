using SyncMedia.Core.Common;

namespace SyncMedia.Core.Interfaces
{
    /// <summary>
    /// Provides centralized error handling and result management
    /// </summary>
    public interface IErrorHandler
    {
        /// <summary>
        /// Executes an async operation with error handling
        /// </summary>
        Task<Result<T>> ExecuteAsync<T>(Func<Task<T>> action, string operationName);
        
        /// <summary>
        /// Logs an error with context
        /// </summary>
        void LogError(Exception ex, string context);
        
        /// <summary>
        /// Handles and logs an exception, returning a failure result
        /// </summary>
        Result<T> HandleError<T>(Exception ex, string operationName);
    }
}