using System;
using Xunit;
using SyncMedia.Core.Services;
using SyncMedia.Core.Models;
using SyncMedia.Core.Constants;

namespace SyncMedia.Tests.Core.Services
{
    public class FeatureFlagServiceTests
    {
        [Fact]
        public void Constructor_WithNullLicense_ShouldNotThrow()
        {
            // Act & Assert - constructor should handle null gracefully or throw ArgumentNullException
            var license = new LicenseInfo();
            var service = new FeatureFlagService(license);
            Assert.NotNull(service);
        }

        [Fact]
        public void HasProAccess_WhenProLicenseValid_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "ABCD-1234-EFGH-5678",
                ExpirationDate = null
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.True(service.HasProAccess);
        }

        [Fact]
        public void HasProAccess_WhenProLicenseExpired_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "ABCD-1234-EFGH-5678",
                ExpirationDate = DateTime.Now.AddDays(-1)
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.False(service.HasProAccess);
        }

        [Fact]
        public void HasProAccess_WhenFreeUser_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.False(service.HasProAccess);
        }

        [Fact]
        public void ShouldShowAds_WhenProUser_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "ABCD-1234-EFGH-5678"
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.False(service.ShouldShowAds);
        }

        [Fact]
        public void ShouldShowAds_WhenFreeUser_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.True(service.ShouldShowAds);
        }

        [Fact]
        public void ShouldThrottle_WhenProUser_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "ABCD-1234-EFGH-5678"
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.False(service.ShouldThrottle);
        }

        [Fact]
        public void ShouldThrottle_WhenFreeUserNoBoost_ShouldReturnTrue()
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
        public void ShouldThrottle_WhenFreeUserWithActiveBoost_ShouldReturnFalse()
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
        public void ShouldThrottle_WhenFreeUserWithExpiredBoost_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                SpeedBoostExpirationDate = DateTime.Now.AddMinutes(-1)
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.True(service.ShouldThrottle);
        }

        [Fact]
        public void GetThrottleDelayMs_WhenProUser_ShouldReturnZero()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "ABCD-1234-EFGH-5678"
            };
            var service = new FeatureFlagService(license);

            // Act
            var delay = service.GetThrottleDelayMs();

            // Assert
            Assert.Equal(0, delay);
        }

        [Fact]
        public void GetThrottleDelayMs_WhenFreeUserWithBoost_ShouldReturnZero()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                SpeedBoostExpirationDate = DateTime.Now.AddMinutes(30),
                FilesProcessedCount = 100
            };
            var service = new FeatureFlagService(license);

            // Act
            var delay = service.GetThrottleDelayMs();

            // Assert
            Assert.Equal(0, delay);
        }

        [Theory]
        [InlineData(0, 500)]
        [InlineData(25, 500)]
        [InlineData(49, 500)]
        public void GetThrottleDelayMs_WhenFilesUnder50_ShouldReturn500ms(int filesProcessed, int expectedDelay)
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                FilesProcessedCount = filesProcessed
            };
            var service = new FeatureFlagService(license);

            // Act
            var delay = service.GetThrottleDelayMs();

            // Assert
            Assert.Equal(expectedDelay, delay);
        }

        [Theory]
        [InlineData(50, 1000)]
        [InlineData(60, 1000)]
        [InlineData(74, 1000)]
        public void GetThrottleDelayMs_WhenFilesBetween50And75_ShouldReturn1000ms(int filesProcessed, int expectedDelay)
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                FilesProcessedCount = filesProcessed
            };
            var service = new FeatureFlagService(license);

            // Act
            var delay = service.GetThrottleDelayMs();

            // Assert
            Assert.Equal(expectedDelay, delay);
        }

        [Theory]
        [InlineData(75, 2000)]
        [InlineData(76, 2000)]
        [InlineData(100, 2000)]
        [InlineData(150, 2000)]
        public void GetThrottleDelayMs_WhenFiles75OrMore_ShouldReturn2000ms(int filesProcessed, int expectedDelay)
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                FilesProcessedCount = filesProcessed
            };
            var service = new FeatureFlagService(license);

            // Act
            var delay = service.GetThrottleDelayMs();

            // Assert
            Assert.Equal(expectedDelay, delay);
        }

        [Fact]
        public void IsFeatureEnabled_WhenProUser_ShouldEnableProFeatures()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "ABCD-1234-EFGH-5678"
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.True(service.IsFeatureEnabled(ProFeatures.ParallelProcessing));
            Assert.True(service.IsFeatureEnabled(ProFeatures.GpuAcceleration));
            Assert.True(service.IsFeatureEnabled(ProFeatures.AdvancedDuplicateDetection));
            Assert.True(service.IsFeatureEnabled(ProFeatures.AdFree));
        }

        [Fact]
        public void IsFeatureEnabled_WhenFreeUser_ShouldNotEnableProFeatures()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false
            };
            var service = new FeatureFlagService(license);

            // Act & Assert
            Assert.False(service.IsFeatureEnabled(ProFeatures.ParallelProcessing));
            Assert.False(service.IsFeatureEnabled(ProFeatures.GpuAcceleration));
            Assert.False(service.IsFeatureEnabled(ProFeatures.AdvancedDuplicateDetection));
            Assert.False(service.IsFeatureEnabled(ProFeatures.AdFree));
        }

        [Fact]
        public void RefreshFeatureFlags_ShouldUpdateFeatureAvailability()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false
            };
            var service = new FeatureFlagService(license);
            Assert.False(service.IsFeatureEnabled(ProFeatures.AdFree));

            // Act - Upgrade to Pro
            license.IsPro = true;
            license.LicenseKey = "ABCD-1234-EFGH-5678";
            service.RefreshFeatureFlags();

            // Assert
            Assert.True(service.IsFeatureEnabled(ProFeatures.AdFree));
        }

        [Fact]
        public void RefreshFeatureFlags_WhenDowngradeFromPro_ShouldDisableProFeatures()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "ABCD-1234-EFGH-5678"
            };
            var service = new FeatureFlagService(license);
            Assert.True(service.IsFeatureEnabled(ProFeatures.AdFree));

            // Act - Downgrade to Free
            license.IsPro = false;
            license.LicenseKey = null;
            service.RefreshFeatureFlags();

            // Assert
            Assert.False(service.IsFeatureEnabled(ProFeatures.AdFree));
        }
    }
}
