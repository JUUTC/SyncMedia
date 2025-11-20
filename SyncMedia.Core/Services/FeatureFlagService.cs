using SyncMedia.Core.Constants;
using SyncMedia.Core.Models;
using System.Collections.Generic;

namespace SyncMedia.Core.Services
{
    /// <summary>
    /// Service for managing feature flags and Pro feature availability
    /// </summary>
    public class FeatureFlagService
    {
        private readonly LicenseInfo _licenseInfo;
        private readonly HashSet<string> _enabledFeatures;

        public FeatureFlagService(LicenseInfo licenseInfo)
        {
            _licenseInfo = licenseInfo;
            _enabledFeatures = new HashSet<string>();
            InitializeFeatureFlags();
        }

        /// <summary>
        /// Initialize feature flags based on license status
        /// </summary>
        private void InitializeFeatureFlags()
        {
            _enabledFeatures.Clear();

            // Free version features (always enabled)
            // Basic sync, file preview, statistics, achievements are always available

            // Pro features (only if Pro license is valid)
            if (_licenseInfo.IsPro && _licenseInfo.IsValid)
            {
                _enabledFeatures.Add(ProFeatures.ParallelProcessing);
                _enabledFeatures.Add(ProFeatures.AdvancedDuplicateDetection);
                _enabledFeatures.Add(ProFeatures.GpuAcceleration);
                _enabledFeatures.Add(ProFeatures.AdvancedAnalytics);
                _enabledFeatures.Add(ProFeatures.PerformanceOptimizations);
                _enabledFeatures.Add(ProFeatures.AdFree);
                _enabledFeatures.Add(ProFeatures.CustomSyncProfiles);
                _enabledFeatures.Add(ProFeatures.PrioritySupport);
            }
        }

        /// <summary>
        /// Check if a feature is enabled
        /// </summary>
        /// <param name="featureName">The feature name from ProFeatures constants</param>
        /// <returns>True if the feature is enabled, false otherwise</returns>
        public bool IsFeatureEnabled(string featureName)
        {
            return _enabledFeatures.Contains(featureName);
        }

        /// <summary>
        /// Check if user has Pro access
        /// </summary>
        public bool HasProAccess => _licenseInfo.IsPro && _licenseInfo.IsValid;

        /// <summary>
        /// Check if ads should be shown (Free version)
        /// </summary>
        public bool ShouldShowAds => !IsFeatureEnabled(ProFeatures.AdFree);

        /// <summary>
        /// Check if user should be throttled based on license and speed boost status
        /// </summary>
        public bool ShouldThrottle => !HasProAccess && !_licenseInfo.HasActiveSpeedBoost;

        /// <summary>
        /// Get the throttle delay in milliseconds based on files processed
        /// Progressive throttling: more files = progressively more delay
        /// Ad interactions reset the counter to 0, removing throttle
        /// </summary>
        public int GetThrottleDelayMs()
        {
            if (!ShouldThrottle) return 0;

            // Progressive throttling formula
            // Delay increases by 100ms for every 10 files processed
            // 0 files: 0ms
            // 10 files: 100ms
            // 50 files: 500ms
            // 100 files: 1000ms (1 second)
            // 200 files: 2000ms (2 seconds)
            // 500 files: 5000ms (5 seconds)
            // Caps at 10 seconds to avoid extreme delays
            var filesProcessed = _licenseInfo.FilesProcessedCount;
            var delay = (filesProcessed / 10) * 100;
            
            // Cap at 10 seconds maximum
            return Math.Min(delay, 10000);
        }

        /// <summary>
        /// Refresh feature flags (call after license status changes)
        /// </summary>
        public void RefreshFeatureFlags()
        {
            InitializeFeatureFlags();
        }
    }
}
