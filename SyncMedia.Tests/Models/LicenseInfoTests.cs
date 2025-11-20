using System;
using Xunit;
using SyncMedia.Core.Models;

namespace SyncMedia.Tests.Models
{
    public class LicenseInfoTests
    {
        [Fact]
        public void IsPro_WithLicenseKey_ShouldBeValid()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "TEST-KEY-1234",
                ActivationDate = DateTime.Now
            };
            
            // Act & Assert
            Assert.True(license.IsValid);
        }

        [Fact]
        public void IsPro_WithExpiredLicense_ShouldBeInvalid()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "TEST-KEY-1234",
                ActivationDate = DateTime.Now.AddDays(-365),
                ExpirationDate = DateTime.Now.AddDays(-1)
            };
            
            // Act & Assert
            Assert.False(license.IsValid);
        }

        [Fact]
        public void IsPro_WithoutLicenseKey_ShouldBeInvalid()
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
        public void IsValid_WithLifetimeLicense_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "LIFETIME-KEY-1234",
                ActivationDate = DateTime.Now,
                ExpirationDate = null // Lifetime license
            };
            
            // Act & Assert
            Assert.True(license.IsValid);
        }

        [Fact]
        public void HasActiveSpeedBoost_WithValidBoost_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                SpeedBoostExpirationDate = DateTime.Now.AddHours(1)
            };
            
            // Act & Assert
            Assert.True(license.HasActiveSpeedBoost);
        }

        [Fact]
        public void HasActiveSpeedBoost_WithExpiredBoost_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                SpeedBoostExpirationDate = DateTime.Now.AddHours(-1)
            };
            
            // Act & Assert
            Assert.False(license.HasActiveSpeedBoost);
        }

        [Fact]
        public void HasActiveSpeedBoost_WithNoBoost_ShouldReturnFalse()
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
        public void CheckAndResetPeriod_After30Days_ShouldResetCount()
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
        public void CheckAndResetPeriod_Within30Days_ShouldNotReset()
        {
            // Arrange
            var license = new LicenseInfo
            {
                FilesProcessedCount = 50,
                PeriodStartDate = DateTime.Now.AddDays(-15)
            };
            var originalCount = license.FilesProcessedCount;
            var originalDate = license.PeriodStartDate;
            
            // Act
            license.CheckAndResetPeriod();
            
            // Assert
            Assert.Equal(originalCount, license.FilesProcessedCount);
            Assert.Equal(originalDate, license.PeriodStartDate);
        }

        [Fact]
        public void CheckAndResetPeriod_WithNoPeriodStart_ShouldInitialize()
        {
            // Arrange
            var license = new LicenseInfo
            {
                FilesProcessedCount = 10,
                PeriodStartDate = null
            };
            
            // Act
            license.CheckAndResetPeriod();
            
            // Assert
            Assert.Equal(0, license.FilesProcessedCount);
            Assert.NotNull(license.PeriodStartDate);
            Assert.True((DateTime.Now - license.PeriodStartDate.Value).TotalSeconds < 5);
        }

        [Fact]
        public void IsStoreLicense_ShouldTrackStorePurchases()
        {
            // Arrange & Act
            var storeLicense = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "STORE-KEY-1234",
                IsStoreLicense = true
            };
            
            // Assert
            Assert.True(storeLicense.IsStoreLicense);
        }

        [Fact]
        public void MachineId_ShouldBeSetForHardwareBoundLicenses()
        {
            // Arrange & Act
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "HARDWARE-KEY-1234",
                MachineId = "MACHINE-ABC-123"
            };
            
            // Assert
            Assert.Equal("MACHINE-ABC-123", license.MachineId);
        }
    }
}
