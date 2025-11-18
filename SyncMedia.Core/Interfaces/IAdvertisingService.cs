using System;

namespace SyncMedia.Core.Interfaces
{
    /// <summary>
    /// Service interface for managing advertisements in the free version
    /// </summary>
    public interface IAdvertisingService
    {
        /// <summary>
        /// Gets whether ads are currently enabled
        /// </summary>
        bool AdsEnabled { get; }

        /// <summary>
        /// Gets whether ads are being blocked (by ad-blocker or network issues)
        /// </summary>
        bool AdsBlocked { get; }

        /// <summary>
        /// Event raised when ad blocking is detected
        /// </summary>
        event EventHandler<AdBlockedEventArgs> AdBlockingDetected;

        /// <summary>
        /// Event raised when ad successfully loads
        /// </summary>
        event EventHandler AdLoaded;

        /// <summary>
        /// Event raised when ad fails to load
        /// </summary>
        event EventHandler<AdErrorEventArgs> AdFailed;

        /// <summary>
        /// Initialize the advertising service
        /// </summary>
        void Initialize();

        /// <summary>
        /// Show advertisements
        /// </summary>
        void ShowAds();

        /// <summary>
        /// Hide advertisements
        /// </summary>
        void HideAds();

        /// <summary>
        /// Update ad visibility based on license status
        /// </summary>
        /// <param name="shouldShowAds">Whether ads should be shown</param>
        void UpdateAdVisibility(bool shouldShowAds);
    }

    /// <summary>
    /// Event args for ad blocking detection
    /// </summary>
    public class AdBlockedEventArgs : EventArgs
    {
        public string Reason { get; set; }
        public bool IsNetworkIssue { get; set; }
    }

    /// <summary>
    /// Event args for ad errors
    /// </summary>
    public class AdErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }
    }
}
