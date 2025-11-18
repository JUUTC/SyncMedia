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
        /// Event raised when video ad completes successfully
        /// </summary>
        event EventHandler<VideoAdCompletedEventArgs> VideoAdCompleted;

        /// <summary>
        /// Event raised when user clicks on an ad
        /// </summary>
        event EventHandler AdClicked;

        /// <summary>
        /// Initialize the advertising service
        /// </summary>
        void Initialize();

        /// <summary>
        /// Show banner advertisements
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

        /// <summary>
        /// Show an interstitial video ad
        /// </summary>
        /// <returns>True if video ad was shown successfully</returns>
        bool ShowInterstitialVideoAd();

        /// <summary>
        /// Check if a video ad is ready to be shown
        /// </summary>
        bool IsVideoAdReady { get; }
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

    /// <summary>
    /// Event args for video ad completion
    /// </summary>
    public class VideoAdCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets whether the video was watched completely
        /// </summary>
        public bool WatchedCompletely { get; set; }

        /// <summary>
        /// Gets the percentage of video watched (0-100)
        /// </summary>
        public int PercentageWatched { get; set; }
    }
}
