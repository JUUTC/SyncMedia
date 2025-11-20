using System;
using Xunit;
using SyncMedia.Core.Models;

namespace SyncMedia.Tests.Core.Models
{
    public class LicenseInfoTests
    {
        [Fact]
        public void HasActiveSpeedBoost_WhenNotSet_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                SpeedBoostExpirationDate = null
            };

            // Act & Assert
            Assert.False(license.HasActiveSpeedBoost);
        }

        [Fact]
        public void HasActiveSpeedBoost_WhenExpired_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                SpeedBoostExpirationDate = DateTime.Now.AddMinutes(-10)
            };

            // Act & Assert
            Assert.False(license.HasActiveSpeedBoost);
        }

        [Fact]
        public void HasActiveSpeedBoost_WhenActive_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                SpeedBoostExpirationDate = DateTime.Now.AddMinutes(30)
            };

            // Act & Assert
            Assert.True(license.HasActiveSpeedBoost);
        }

        [Fact]
        public void CheckAndResetPeriod_WhenNoPeriodStart_ShouldInitializePeriod()
        {
            // Arrange
            var license = new LicenseInfo
            {
                FilesProcessedCount = 50,
                PeriodStartDate = null
            };

            // Act
            license.CheckAndResetPeriod();

            // Assert
            Assert.NotNull(license.PeriodStartDate);
            Assert.Equal(0, license.FilesProcessedCount);
        }

        [Fact]
        public void CheckAndResetPeriod_WhenWithin30Days_ShouldNotReset()
        {
            // Arrange
            var license = new LicenseInfo
            {
                FilesProcessedCount = 50,
                PeriodStartDate = DateTime.Now.AddDays(-15)
            };

            // Act
            license.CheckAndResetPeriod();

            // Assert
            Assert.Equal(50, license.FilesProcessedCount);
        }

        [Fact]
        public void CheckAndResetPeriod_WhenOver30Days_ShouldReset()
        {
            // Arrange
            var license = new LicenseInfo
            {
                FilesProcessedCount = 100,
                PeriodStartDate = DateTime.Now.AddDays(-31)
            };

            // Act
            license.CheckAndResetPeriod();

            // Assert
            Assert.Equal(0, license.FilesProcessedCount);
            Assert.True((DateTime.Now - license.PeriodStartDate.Value).TotalDays < 1);
        }

        [Fact]
        public void IsValid_WhenProWithValidLicense_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "XXXX-XXXX-XXXX-XXXX",
                ExpirationDate = null
            };

            // Act & Assert
            Assert.True(license.IsValid);
        }

        [Fact]
        public void IsValid_WhenProWithExpiredLicense_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "XXXX-XXXX-XXXX-XXXX",
                ExpirationDate = DateTime.Now.AddDays(-1)
            };

            // Act & Assert
            Assert.False(license.IsValid);
        }

        [Fact]
        public void IsValid_WhenProWithNoKey_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = null
            };

            // Act & Assert
            Assert.False(license.IsValid);
        }

        [Fact]
        public void IsValid_WhenFree_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                LicenseKey = null
            };

            // Act & Assert
            Assert.False(license.IsValid);
        }
    }
}
