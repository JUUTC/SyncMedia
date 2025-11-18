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
        /// Progressive throttling: more files = more delay
        /// </summary>
        public int GetThrottleDelayMs()
        {
            if (!ShouldThrottle) return 0;

            // Progressive throttling formula
            // 0-50 files: 500ms delay
            // 51-75 files: 1000ms delay  
            // 76+ files: 2000ms delay
            var filesProcessed = _licenseInfo.FilesProcessedCount;
            
            if (filesProcessed < 50)
                return 500;
            else if (filesProcessed < 75)
                return 1000;
            else
                return 2000;
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
