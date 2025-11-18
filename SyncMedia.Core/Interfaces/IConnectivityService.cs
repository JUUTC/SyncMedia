using System;

namespace SyncMedia.Core.Interfaces
{
    /// <summary>
    /// Service interface for monitoring network connectivity
    /// </summary>
    public interface IConnectivityService
    {
        /// <summary>
        /// Gets whether the device currently has internet connectivity
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Event raised when connectivity status changes
        /// </summary>
        event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;

        /// <summary>
        /// Check connectivity status
        /// </summary>
        /// <returns>True if connected, false otherwise</returns>
        bool CheckConnectivity();

        /// <summary>
        /// Start monitoring connectivity changes
        /// </summary>
        void StartMonitoring();

        /// <summary>
        /// Stop monitoring connectivity changes
        /// </summary>
        void StopMonitoring();
    }

    /// <summary>
    /// Event args for connectivity change events
    /// </summary>
    public class ConnectivityChangedEventArgs : EventArgs
    {
        public bool IsConnected { get; set; }
        public string Reason { get; set; }
    }
}
