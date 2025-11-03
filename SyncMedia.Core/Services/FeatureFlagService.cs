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

            // Pro features (only if Pro license is valid or in trial)
            if (_licenseInfo.IsPro && _licenseInfo.IsValid || _licenseInfo.IsInTrial)
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
        /// Check if user has Pro access (either valid license or active trial)
        /// </summary>
        public bool HasProAccess => (_licenseInfo.IsPro && _licenseInfo.IsValid) || _licenseInfo.IsInTrial;

        /// <summary>
        /// Check if ads should be shown (Free version and not in trial)
        /// </summary>
        public bool ShouldShowAds => !IsFeatureEnabled(ProFeatures.AdFree);

        /// <summary>
        /// Refresh feature flags (call after license status changes)
        /// </summary>
        public void RefreshFeatureFlags()
        {
            InitializeFeatureFlags();
        }
    }
}
