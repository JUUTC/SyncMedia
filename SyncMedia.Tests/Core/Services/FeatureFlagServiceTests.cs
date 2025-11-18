using System;
using Xunit;
using SyncMedia.Core.Services;
using SyncMedia.Core.Models;

namespace SyncMedia.Tests.Core.Services
{
    public class FeatureFlagServiceTests
    {
        [Fact]
        public void HasProAccess_WithValidProLicense_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "TEST-KEY",
                ExpirationDate = null
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.True(service.HasProAccess);
        }

        [Fact]
        public void HasProAccess_WithExpiredProLicense_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "TEST-KEY",
                ExpirationDate = DateTime.Now.AddDays(-1)
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.False(service.HasProAccess);
        }

        [Fact]
        public void HasProAccess_WithFreeLicense_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo { IsPro = false };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.False(service.HasProAccess);
        }

        [Fact]
        public void ShouldShowAds_ForFreeLicense_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo { IsPro = false };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.True(service.ShouldShowAds);
        }

        [Fact]
        public void ShouldShowAds_ForProLicense_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "TEST-KEY"
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.False(service.ShouldShowAds);
        }

        [Fact]
        public void ShouldThrottle_ForFreeLicenseWithoutBoost_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                SpeedBoostExpirationDate = null
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.True(service.ShouldThrottle);
        }

        [Fact]
        public void ShouldThrottle_ForFreeLicenseWithActiveBoost_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                SpeedBoostExpirationDate = DateTime.Now.AddMinutes(30)
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.False(service.ShouldThrottle);
        }

        [Fact]
        public void ShouldThrottle_ForProLicense_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "TEST-KEY"
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.False(service.ShouldThrottle);
        }

        [Theory]
        [InlineData(0, 0)]        // 0 files = 0ms
        [InlineData(10, 100)]     // 10 files = 100ms
        [InlineData(20, 200)]     // 20 files = 200ms
        [InlineData(50, 500)]     // 50 files = 500ms
        [InlineData(100, 1000)]   // 100 files = 1000ms (1 second)
        [InlineData(200, 2000)]   // 200 files = 2000ms (2 seconds)
        [InlineData(500, 5000)]   // 500 files = 5000ms (5 seconds)
        [InlineData(1000, 10000)] // 1000 files = 10000ms (capped at 10 seconds)
        [InlineData(2000, 10000)] // 2000 files = 10000ms (capped at 10 seconds)
        public void GetThrottleDelayMs_ShouldReturnCorrectProgressiveDelay(int filesProcessed, int expectedDelay)
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                FilesProcessedCount = filesProcessed,
                SpeedBoostExpirationDate = null
            };
            var service = new FeatureFlagService(license);

            // Act
            var delay = service.GetThrottleDelayMs();

            // Assert
            Assert.Equal(expectedDelay, delay);
        }

        [Fact]
        public void GetThrottleDelayMs_WithSpeedBoost_ShouldReturnZero()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                FilesProcessedCount = 200, // Would normally be 2000ms
                SpeedBoostExpirationDate = DateTime.Now.AddMinutes(30)
            };
            var service = new FeatureFlagService(license);

            // Act
            var delay = service.GetThrottleDelayMs();

            // Assert
            Assert.Equal(0, delay);
        }

        [Fact]
        public void GetThrottleDelayMs_ForProLicense_ShouldAlwaysReturnZero()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "TEST-KEY",
                FilesProcessedCount = 1000 // Would be 10000ms for free user
            };
            var service = new FeatureFlagService(license);

            // Act
            var delay = service.GetThrottleDelayMs();

            // Assert
            Assert.Equal(0, delay);
        }

        [Fact]
        public void IsFeatureEnabled_ForProFeature_WithProLicense_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "TEST-KEY"
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.True(service.IsFeatureEnabled(SyncMedia.Core.Constants.ProFeatures.ParallelProcessing));
            Assert.True(service.IsFeatureEnabled(SyncMedia.Core.Constants.ProFeatures.AdvancedDuplicateDetection));
        }

        [Fact]
        public void IsFeatureEnabled_ForProFeature_WithFreeLicense_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo { IsPro = false };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.False(service.IsFeatureEnabled(SyncMedia.Core.Constants.ProFeatures.ParallelProcessing));
            Assert.False(service.IsFeatureEnabled(SyncMedia.Core.Constants.ProFeatures.AdvancedDuplicateDetection));
        }

        [Fact]
        public void RefreshFeatureFlags_AfterLicenseChange_ShouldUpdateFlags()
        {
            // Arrange
            var license = new LicenseInfo { IsPro = false };
            var service = new FeatureFlagService(license);
            Assert.False(service.IsFeatureEnabled(SyncMedia.Core.Constants.ProFeatures.ParallelProcessing));

            // Act - Change license status
            license.IsPro = true;
            license.LicenseKey = "TEST-KEY";
            service.RefreshFeatureFlags();

            // Assert
            Assert.True(service.IsFeatureEnabled(SyncMedia.Core.Constants.ProFeatures.ParallelProcessing));
        }
    }
}
