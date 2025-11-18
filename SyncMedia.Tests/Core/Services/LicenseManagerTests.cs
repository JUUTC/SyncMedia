using System;
using System.IO;
using Xunit;
using SyncMedia.Core.Services;
using SyncMedia.Core.Models;

namespace SyncMedia.Tests.Core.Services
{
    public class LicenseManagerTests : IDisposable
    {
        private readonly string _testLicensePath;
        private readonly LicenseManager _licenseManager;

        public LicenseManagerTests()
        {
            // Create a temporary directory for test license files
            _testLicensePath = Path.Combine(Path.GetTempPath(), "SyncMediaTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testLicensePath);
            
            // Set environment variable to use test path
            Environment.SetEnvironmentVariable("LOCALAPPDATA", _testLicensePath);
            
            _licenseManager = new LicenseManager();
        }

        public void Dispose()
        {
            // Clean up test directory
            if (Directory.Exists(_testLicensePath))
            {
                Directory.Delete(_testLicensePath, true);
            }
        }

        [Fact]
        public void Constructor_ShouldInitializeWithFreeLicense()
        {
            // Assert
            Assert.NotNull(_licenseManager.CurrentLicense);
            Assert.False(_licenseManager.CurrentLicense.IsPro);
            Assert.Equal(0, _licenseManager.CurrentLicense.FilesProcessedCount);
            Assert.NotNull(_licenseManager.CurrentLicense.PeriodStartDate);
        }

        [Fact]
        public void IncrementFilesProcessed_ShouldIncreaseCount()
        {
            // Arrange
            var initialCount = _licenseManager.CurrentLicense.FilesProcessedCount;

            // Act
            _licenseManager.IncrementFilesProcessed(5);

            // Assert
            Assert.Equal(initialCount + 5, _licenseManager.CurrentLicense.FilesProcessedCount);
        }

        [Fact]
        public void IncrementFilesProcessed_WithDefaultParam_ShouldIncrementByOne()
        {
            // Arrange
            var initialCount = _licenseManager.CurrentLicense.FilesProcessedCount;

            // Act
            _licenseManager.IncrementFilesProcessed();

            // Assert
            Assert.Equal(initialCount + 1, _licenseManager.CurrentLicense.FilesProcessedCount);
        }

        [Fact]
        public void ResetFilesProcessedCounter_ShouldResetToZero()
        {
            // Arrange
            _licenseManager.IncrementFilesProcessed(50);
            Assert.Equal(50, _licenseManager.CurrentLicense.FilesProcessedCount);

            // Act
            _licenseManager.ResetFilesProcessedCounter();

            // Assert
            Assert.Equal(0, _licenseManager.CurrentLicense.FilesProcessedCount);
        }

        [Fact]
        public void ActivateSpeedBoost_WithDefaultDuration_ShouldSet60Minutes()
        {
            // Act
            _licenseManager.ActivateSpeedBoost();

            // Assert
            Assert.NotNull(_licenseManager.CurrentLicense.SpeedBoostExpirationDate);
            var expirationTime = _licenseManager.CurrentLicense.SpeedBoostExpirationDate.Value;
            var now = DateTime.Now;
            var diff = (expirationTime - now).TotalMinutes;
            
            // Allow some variance due to timing
            Assert.True(diff >= 59 && diff <= 61, $"Expected ~60 minutes, got {diff}");
        }

        [Fact]
        public void ActivateSpeedBoost_WithCustomDuration_ShouldSetCorrectMinutes()
        {
            // Act
            _licenseManager.ActivateSpeedBoost(30);

            // Assert
            Assert.NotNull(_licenseManager.CurrentLicense.SpeedBoostExpirationDate);
            var expirationTime = _licenseManager.CurrentLicense.SpeedBoostExpirationDate.Value;
            var now = DateTime.Now;
            var diff = (expirationTime - now).TotalMinutes;
            
            // Allow some variance due to timing
            Assert.True(diff >= 29 && diff <= 31, $"Expected ~30 minutes, got {diff}");
        }

        [Fact]
        public void ActivateLicense_WithValidKey_ShouldActivate()
        {
            // Act
            var result = _licenseManager.ActivateLicense("ABCD-EFGH-IJKL-D6F9");

            // Assert
            Assert.True(result);
            Assert.True(_licenseManager.CurrentLicense.IsPro);
            Assert.NotNull(_licenseManager.CurrentLicense.LicenseKey);
            Assert.NotNull(_licenseManager.CurrentLicense.ActivationDate);
        }

        [Fact]
        public void ActivateLicense_WithInvalidFormat_ShouldReturnFalse()
        {
            // Act
            var result = _licenseManager.ActivateLicense("INVALID");

            // Assert
            Assert.False(result);
            Assert.False(_licenseManager.CurrentLicense.IsPro);
        }

        [Fact]
        public void ActivateLicense_WithNullKey_ShouldReturnFalse()
        {
            // Act
            var result = _licenseManager.ActivateLicense(null);

            // Assert
            Assert.False(result);
            Assert.False(_licenseManager.CurrentLicense.IsPro);
        }

        [Fact]
        public void ActivateLicense_WithEmptyKey_ShouldReturnFalse()
        {
            // Act
            var result = _licenseManager.ActivateLicense("");

            // Assert
            Assert.False(result);
            Assert.False(_licenseManager.CurrentLicense.IsPro);
        }

        [Fact]
        public void DeactivateLicense_ShouldResetToFreeLicense()
        {
            // Arrange
            _licenseManager.ActivateLicense("ABCD-EFGH-IJKL-D6F9");
            Assert.True(_licenseManager.CurrentLicense.IsPro);

            // Act
            _licenseManager.DeactivateLicense();

            // Assert
            Assert.False(_licenseManager.CurrentLicense.IsPro);
            Assert.Null(_licenseManager.CurrentLicense.LicenseKey);
            Assert.Null(_licenseManager.CurrentLicense.ActivationDate);
        }

        [Fact]
        public void GenerateLicenseKey_ShouldReturnValidFormat()
        {
            // Act
            var key = LicenseManager.GenerateLicenseKey();

            // Assert
            Assert.NotNull(key);
            Assert.Equal(19, key.Length); // XXXX-XXXX-XXXX-XXXX
            Assert.Equal(3, key.Count(c => c == '-'));
            
            var parts = key.Split('-');
            Assert.Equal(4, parts.Length);
            foreach (var part in parts)
            {
                Assert.Equal(4, part.Length);
            }
        }

        [Fact]
        public void GenerateLicenseKey_ShouldGenerateUniqueyKeys()
        {
            // Act
            var key1 = LicenseManager.GenerateLicenseKey();
            var key2 = LicenseManager.GenerateLicenseKey();

            // Assert
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void LicensePersistence_ShouldSaveAndLoad()
        {
            // Arrange
            _licenseManager.IncrementFilesProcessed(25);
            _licenseManager.ActivateSpeedBoost(45);

            // Act - Create new manager to test loading
            var newManager = new LicenseManager();

            // Assert
            Assert.Equal(25, newManager.CurrentLicense.FilesProcessedCount);
            Assert.NotNull(newManager.CurrentLicense.SpeedBoostExpirationDate);
            Assert.NotNull(newManager.CurrentLicense.PeriodStartDate);
        }
    }
}
