using System;
using System.IO;
using Xunit;
using SyncMedia.Core.Models;
using SyncMedia.Core.Services;

namespace SyncMedia.Tests.Services
{
    public class LicenseManagerTests
    {
        // Note: LicenseManager tests are simplified due to file system dependencies
        // Full integration tests would require mocking the file system

        [Fact]
        public void ActivateLicense_WithNullKey_ShouldReturnFalse()
        {
            // Arrange
            var manager = new LicenseManager();

            // Act
            var result = manager.ActivateLicense(null!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ActivateLicense_WithEmptyKey_ShouldReturnFalse()
        {
            // Arrange
            var manager = new LicenseManager();

            // Act
            var result = manager.ActivateLicense(string.Empty);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ActivateLicense_WithWhitespaceKey_ShouldReturnFalse()
        {
            // Arrange
            var manager = new LicenseManager();

            // Act
            var result = manager.ActivateLicense("   ");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ActivateLicense_WithInvalidKeyLength_ShouldReturnFalse()
        {
            // Arrange
            var manager = new LicenseManager();

            // Act - Each part too short (3 chars instead of 4)
            var result = manager.ActivateLicense("ABC-123-DEF-456");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ActivateLicense_WithIncorrectNumberOfParts_ShouldReturnFalse()
        {
            // Arrange
            var manager = new LicenseManager();

            // Act - Only 3 parts instead of 4
            var result = manager.ActivateLicense("ABCD-1234-EFGH");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CurrentLicense_ShouldNotBeNull()
        {
            // Arrange & Act
            var manager = new LicenseManager();

            // Assert
            Assert.NotNull(manager.CurrentLicense);
        }

        [Fact]
        public void DeactivateLicense_ShouldResetLicenseProperties()
        {
            // Arrange
            var manager = new LicenseManager();
            // Use valid key: ABCD-1234-EFGH-E10A
            manager.ActivateLicense("ABCD-1234-EFGH-E10A");

            // Act
            manager.DeactivateLicense();

            // Assert
            Assert.False(manager.CurrentLicense.IsPro);
            Assert.Null(manager.CurrentLicense.LicenseKey);
        }

        [Fact]
        public void ActivateLicense_WithValidKey_ShouldSetProFlag()
        {
            // Arrange
            var manager = new LicenseManager();

            // Act - Valid key with correct checksum
            var result = manager.ActivateLicense("ABCD-1234-EFGH-E10A");

            // Assert
            Assert.True(result);
            Assert.True(manager.CurrentLicense.IsPro);
        }

        [Fact]
        public void ActivateLicense_WithValidKey_ShouldSetActivationDate()
        {
            // Arrange
            var manager = new LicenseManager();

            // Act
            manager.ActivateLicense("TEST-1234-WXYZ-9149");

            // Assert
            Assert.NotNull(manager.CurrentLicense.ActivationDate);
        }

        [Fact]
        public void ActivateLicense_ShouldSetLifetimeLicense()
        {
            // Arrange
            var manager = new LicenseManager();

            // Act
            manager.ActivateLicense("DEMO-5678-PROD-A1A4");

            // Assert
            Assert.Null(manager.CurrentLicense.ExpirationDate); // Lifetime
        }

        [Fact]
        public void DeactivateLicense_MultipleTimes_ShouldNotThrow()
        {
            // Arrange
            var manager = new LicenseManager();

            // Act & Assert - Should not throw
            manager.DeactivateLicense();
            manager.DeactivateLicense();
            manager.DeactivateLicense();
        }
    }
}
