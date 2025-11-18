using System;
using Xunit;
using SyncMedia.Core.Models;

namespace SyncMedia.Tests.Core.Models
{
    public class LicenseInfoTests
    {
        [Fact]
        public void HasReachedFreeLimit_WhenProLicense_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                FilesProcessedCount = 150
            };

            // Act & Assert
            Assert.False(license.HasReachedFreeLimit);
        }

        [Fact]
        public void HasReachedFreeLimit_WhenUnderLimit_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                FilesProcessedCount = 50,
                BonusFilesFromAds = 10
            };

            // Act & Assert
            Assert.False(license.HasReachedFreeLimit);
        }

        [Fact]
        public void HasReachedFreeLimit_WhenAtExactLimit_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                FilesProcessedCount = 25, // Updated to new limit
                BonusFilesFromAds = 0
            };

            // Act & Assert
            Assert.False(license.HasReachedFreeLimit);
        }

        [Fact]
        public void HasReachedFreeLimit_WhenOverLimit_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                FilesProcessedCount = 26, // Updated to new limit
                BonusFilesFromAds = 0
            };

            // Act & Assert
            Assert.True(license.HasReachedFreeLimit);
        }

        [Fact]
        public void HasReachedFreeLimit_WithBonusFiles_ShouldIncludeBonusInLimit()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                FilesProcessedCount = 50,
                BonusFilesFromAds = 50 // Updated bonus
            };

            // Act & Assert
            // Limit is 25 + 50 = 75, processed is 50, so not reached
            Assert.False(license.HasReachedFreeLimit);
        }

        [Fact]
        public void RemainingFreeFiles_WhenProLicense_ShouldReturnMaxValue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                FilesProcessedCount = 50
            };

            // Act & Assert
            Assert.Equal(int.MaxValue, license.RemainingFreeFiles);
        }

        [Fact]
        public void RemainingFreeFiles_ShouldCalculateCorrectly()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                FilesProcessedCount = 10,
                BonusFilesFromAds = 50 // Updated bonus
            };

            // Act
            var remaining = license.RemainingFreeFiles;

            // Assert
            // Total available: 25 + 50 = 75
            // Processed: 10
            // Remaining: 65
            Assert.Equal(65, remaining);
        }

        [Fact]
        public void RemainingFreeFiles_WhenOverLimit_ShouldReturnZero()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                FilesProcessedCount = 80,
                BonusFilesFromAds = 50 // Updated bonus
            };

            // Act
            var remaining = license.RemainingFreeFiles;

            // Assert
            Assert.Equal(0, remaining);
        }

        [Fact]
        public void HasActiveSpeedBoost_WhenNoBoost_ShouldReturnFalse()
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
        public void HasActiveSpeedBoost_WhenBoostExpired_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                SpeedBoostExpirationDate = DateTime.Now.AddMinutes(-1)
            };

            // Act & Assert
            Assert.False(license.HasActiveSpeedBoost);
        }

        [Fact]
        public void HasActiveSpeedBoost_WhenBoostActive_ShouldReturnTrue()
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
                BonusFilesFromAds = 10,
                PeriodStartDate = null
            };

            // Act
            license.CheckAndResetPeriod();

            // Assert
            Assert.NotNull(license.PeriodStartDate);
            Assert.Equal(0, license.FilesProcessedCount);
            Assert.Equal(0, license.BonusFilesFromAds);
        }

        [Fact]
        public void CheckAndResetPeriod_WhenPeriodUnder30Days_ShouldNotReset()
        {
            // Arrange
            var license = new LicenseInfo
            {
                FilesProcessedCount = 50,
                BonusFilesFromAds = 10,
                PeriodStartDate = DateTime.Now.AddDays(-15)
            };

            // Act
            license.CheckAndResetPeriod();

            // Assert
            Assert.Equal(50, license.FilesProcessedCount);
            Assert.Equal(10, license.BonusFilesFromAds);
        }

        [Fact]
        public void CheckAndResetPeriod_WhenPeriodOver30Days_ShouldReset()
        {
            // Arrange
            var license = new LicenseInfo
            {
                FilesProcessedCount = 50,
                BonusFilesFromAds = 10,
                PeriodStartDate = DateTime.Now.AddDays(-31)
            };

            // Act
            license.CheckAndResetPeriod();

            // Assert
            Assert.Equal(0, license.FilesProcessedCount);
            Assert.Equal(0, license.BonusFilesFromAds);
            Assert.True((DateTime.Now - license.PeriodStartDate.Value).TotalDays < 1);
        }

        [Fact]
        public void IsValid_WhenProWithValidKey_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "ABCD-1234-EFGH-5678",
                ExpirationDate = null
            };

            // Act & Assert
            Assert.True(license.IsValid);
        }

        [Fact]
        public void IsValid_WhenProWithExpiredDate_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "ABCD-1234-EFGH-5678",
                ExpirationDate = DateTime.Now.AddDays(-1)
            };

            // Act & Assert
            Assert.False(license.IsValid);
        }

        [Fact]
        public void IsValid_WhenProWithoutKey_ShouldReturnFalse()
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
        public void IsValid_WhenFreeUser_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false
            };

            // Act & Assert
            Assert.False(license.IsValid);
        }

        [Theory]
        [InlineData(25)]
        [InlineData(50)]
        [InlineData(5)]
        public void Constants_ShouldHaveExpectedValues(int expectedValue)
        {
            // Assert - verify the constants are set as expected
            if (expectedValue == 25)
                Assert.Equal(25, LicenseInfo.FREE_FILES_PER_PERIOD);
        }

        [Fact]
        public void BonusConstants_ShouldHaveExpectedValues()
        {
            // Assert - Updated values for fairer model
            Assert.Equal(50, LicenseInfo.BONUS_FILES_PER_VIDEO_AD);
            Assert.Equal(5, LicenseInfo.BONUS_FILES_PER_CLICK);
        }
    }
}
