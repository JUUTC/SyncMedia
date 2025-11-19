using Xunit;
using SyncMedia.Core.Constants;
using SyncMedia.Core.Models;
using SyncMedia.Core.Services;
using System;

namespace SyncMedia.Tests.Services
{
    public class FeatureFlagServiceTests
    {
        [Fact]
        public void Constructor_WithValidLicense_ShouldInitializeProFeatures()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "VALID-KEY"
            };
            
            // Act
            var service = new FeatureFlagService(license);
            
            // Assert
            Assert.True(service.HasProAccess);
            Assert.True(service.IsFeatureEnabled(ProFeatures.ParallelProcessing));
            Assert.True(service.IsFeatureEnabled(ProFeatures.AdvancedDuplicateDetection));
            Assert.True(service.IsFeatureEnabled(ProFeatures.GpuAcceleration));
            Assert.True(service.IsFeatureEnabled(ProFeatures.AdvancedAnalytics));
        }

        [Fact]
        public void Constructor_WithTrialLicense_ShouldEnableProFeatures()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                TrialExpirationDate = DateTime.Now.AddDays(7)
            };
            
            // Act
            var service = new FeatureFlagService(license);
            
            // Assert
            Assert.True(service.HasProAccess);
            Assert.True(service.IsFeatureEnabled(ProFeatures.ParallelProcessing));
            Assert.True(service.IsFeatureEnabled(ProFeatures.AdFree));
        }

        [Fact]
        public void Constructor_WithFreeLicense_ShouldDisableProFeatures()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                LicenseKey = string.Empty
            };
            
            // Act
            var service = new FeatureFlagService(license);
            
            // Assert
            Assert.False(service.HasProAccess);
            Assert.False(service.IsFeatureEnabled(ProFeatures.ParallelProcessing));
            Assert.False(service.IsFeatureEnabled(ProFeatures.AdvancedDuplicateDetection));
            Assert.False(service.IsFeatureEnabled(ProFeatures.GpuAcceleration));
        }

        [Fact]
        public void IsFeatureEnabled_WithEnabledFeature_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "VALID-KEY"
            };
            var service = new FeatureFlagService(license);
            
            // Act
            var isEnabled = service.IsFeatureEnabled(ProFeatures.ParallelProcessing);
            
            // Assert
            Assert.True(isEnabled);
        }

        [Fact]
        public void IsFeatureEnabled_WithDisabledFeature_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                LicenseKey = string.Empty
            };
            var service = new FeatureFlagService(license);
            
            // Act
            var isEnabled = service.IsFeatureEnabled(ProFeatures.ParallelProcessing);
            
            // Assert
            Assert.False(isEnabled);
        }

        [Fact]
        public void IsFeatureEnabled_WithNonExistentFeature_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "VALID-KEY"
            };
            var service = new FeatureFlagService(license);
            
            // Act
            var isEnabled = service.IsFeatureEnabled("NonExistentFeature");
            
            // Assert
            Assert.False(isEnabled);
        }

        [Fact]
        public void HasProAccess_WithValidProLicense_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "VALID-KEY"
            };
            var service = new FeatureFlagService(license);
            
            // Act & Assert
            Assert.True(service.HasProAccess);
        }

        [Fact]
        public void HasProAccess_WithActiveTrial_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                TrialExpirationDate = DateTime.Now.AddDays(7)
            };
            var service = new FeatureFlagService(license);
            
            // Act & Assert
            Assert.True(service.HasProAccess);
        }

        [Fact]
        public void HasProAccess_WithFreeLicense_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                LicenseKey = string.Empty
            };
            var service = new FeatureFlagService(license);
            
            // Act & Assert
            Assert.False(service.HasProAccess);
        }

        [Fact]
        public void ShouldShowAds_WithFreeLicense_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                LicenseKey = string.Empty
            };
            var service = new FeatureFlagService(license);
            
            // Act & Assert
            Assert.True(service.ShouldShowAds);
        }

        [Fact]
        public void ShouldShowAds_WithProLicense_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "VALID-KEY"
            };
            var service = new FeatureFlagService(license);
            
            // Act & Assert
            Assert.False(service.ShouldShowAds);
        }

        [Fact]
        public void ShouldShowAds_WithActiveTrial_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                TrialExpirationDate = DateTime.Now.AddDays(7)
            };
            var service = new FeatureFlagService(license);
            
            // Act & Assert
            Assert.False(service.ShouldShowAds);
        }

        [Fact]
        public void RefreshFeatureFlags_AfterLicenseUpgrade_ShouldEnableProFeatures()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                LicenseKey = string.Empty
            };
            var service = new FeatureFlagService(license);
            
            // Verify initially no Pro access
            Assert.False(service.HasProAccess);
            Assert.False(service.IsFeatureEnabled(ProFeatures.ParallelProcessing));
            
            // Act - Upgrade to Pro
            license.IsPro = true;
            license.LicenseKey = "NEW-PRO-KEY";
            service.RefreshFeatureFlags();
            
            // Assert
            Assert.True(service.HasProAccess);
            Assert.True(service.IsFeatureEnabled(ProFeatures.ParallelProcessing));
        }

        [Fact]
        public void AllProFeatures_ShouldBeEnabledForProUser()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "VALID-KEY"
            };
            var service = new FeatureFlagService(license);
            
            // Act & Assert - Check all Pro features
            Assert.True(service.IsFeatureEnabled(ProFeatures.ParallelProcessing));
            Assert.True(service.IsFeatureEnabled(ProFeatures.AdvancedDuplicateDetection));
            Assert.True(service.IsFeatureEnabled(ProFeatures.GpuAcceleration));
            Assert.True(service.IsFeatureEnabled(ProFeatures.AdvancedAnalytics));
            Assert.True(service.IsFeatureEnabled(ProFeatures.PerformanceOptimizations));
            Assert.True(service.IsFeatureEnabled(ProFeatures.AdFree));
            Assert.True(service.IsFeatureEnabled(ProFeatures.CustomSyncProfiles));
            Assert.True(service.IsFeatureEnabled(ProFeatures.PrioritySupport));
        }

        [Fact]
        public void AllProFeatures_ShouldBeDisabledForFreeUser()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                LicenseKey = string.Empty
            };
            var service = new FeatureFlagService(license);
            
            // Act & Assert - Check all Pro features are disabled
            Assert.False(service.IsFeatureEnabled(ProFeatures.ParallelProcessing));
            Assert.False(service.IsFeatureEnabled(ProFeatures.AdvancedDuplicateDetection));
            Assert.False(service.IsFeatureEnabled(ProFeatures.GpuAcceleration));
            Assert.False(service.IsFeatureEnabled(ProFeatures.AdvancedAnalytics));
            Assert.False(service.IsFeatureEnabled(ProFeatures.PerformanceOptimizations));
            Assert.False(service.IsFeatureEnabled(ProFeatures.AdFree));
            Assert.False(service.IsFeatureEnabled(ProFeatures.CustomSyncProfiles));
            Assert.False(service.IsFeatureEnabled(ProFeatures.PrioritySupport));
        }
    }
}
