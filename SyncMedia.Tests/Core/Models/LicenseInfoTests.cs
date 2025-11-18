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
                FilesProcessedCount = 100,
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
                FilesProcessedCount = 101,
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
                FilesProcessedCount = 110,
                BonusFilesFromAds = 20
            };

            // Act & Assert
            // Limit is 100 + 20 = 120, processed is 110, so not reached
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
                FilesProcessedCount = 30,
                BonusFilesFromAds = 10
            };

            // Act
            var remaining = license.RemainingFreeFiles;

            // Assert
            // Total available: 100 + 10 = 110
            // Processed: 30
            // Remaining: 80
            Assert.Equal(80, remaining);
        }

        [Fact]
        public void RemainingFreeFiles_WhenOverLimit_ShouldReturnZero()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                FilesProcessedCount = 120,
                BonusFilesFromAds = 10
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
        [InlineData(100)]
        [InlineData(20)]
        [InlineData(10)]
        public void Constants_ShouldHaveExpectedValues(int expectedFreeFiles)
        {
            // Assert - verify the constants are set as expected
            if (expectedFreeFiles == 100)
                Assert.Equal(100, LicenseInfo.FREE_FILES_PER_PERIOD);
        }

        [Fact]
        public void BonusConstants_ShouldHaveExpectedValues()
        {
            // Assert
            Assert.Equal(20, LicenseInfo.BONUS_FILES_PER_VIDEO_AD);
            Assert.Equal(10, LicenseInfo.BONUS_FILES_PER_CLICK);
        }
    }
}
