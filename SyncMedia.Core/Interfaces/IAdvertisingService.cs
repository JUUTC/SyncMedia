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
}
