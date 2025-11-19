using System;
using Xunit;
using SyncMedia.Core.Models;

namespace SyncMedia.Tests.Models
{
    public class LicenseInfoTests
    {
        [Fact]
        public void IsInTrial_WithValidTrialDate_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                TrialExpirationDate = DateTime.Now.AddDays(7)
            };
            
            // Act & Assert
            Assert.True(license.IsInTrial);
        }

        [Fact]
        public void IsInTrial_WithExpiredTrialDate_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                TrialExpirationDate = DateTime.Now.AddDays(-1)
            };
            
            // Act & Assert
            Assert.False(license.IsInTrial);
        }

        [Fact]
        public void IsInTrial_WithNoTrialDate_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                TrialExpirationDate = null
            };
            
            // Act & Assert
            Assert.False(license.IsInTrial);
        }

        [Fact]
        public void IsTrialExpired_WithExpiredDate_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                TrialExpirationDate = DateTime.Now.AddDays(-1)
            };
            
            // Act & Assert
            Assert.True(license.IsTrialExpired);
        }

        [Fact]
        public void IsTrialExpired_WithFutureDate_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                TrialExpirationDate = DateTime.Now.AddDays(7)
            };
            
            // Act & Assert
            Assert.False(license.IsTrialExpired);
        }

        [Fact]
        public void IsTrialExpired_WithNoTrialDate_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                TrialExpirationDate = null
            };
            
            // Act & Assert
            Assert.False(license.IsTrialExpired);
        }

        [Fact]
        public void TrialDaysRemaining_WithValidTrial_ShouldReturnCorrectDays()
        {
            // Arrange
            var license = new LicenseInfo
            {
                TrialExpirationDate = DateTime.Now.AddDays(7)
            };
            
            // Act
            var daysRemaining = license.TrialDaysRemaining;
            
            // Assert
            Assert.InRange(daysRemaining, 6, 7); // Could be 6 or 7 depending on time of day
        }

        [Fact]
        public void TrialDaysRemaining_WithExpiredTrial_ShouldReturnZero()
        {
            // Arrange
            var license = new LicenseInfo
            {
                TrialExpirationDate = DateTime.Now.AddDays(-5)
            };
            
            // Act
            var daysRemaining = license.TrialDaysRemaining;
            
            // Assert
            Assert.Equal(0, daysRemaining);
        }

        [Fact]
        public void TrialDaysRemaining_WithNoTrial_ShouldReturnZero()
        {
            // Arrange
            var license = new LicenseInfo
            {
                TrialExpirationDate = null
            };
            
            // Act
            var daysRemaining = license.TrialDaysRemaining;
            
            // Assert
            Assert.Equal(0, daysRemaining);
        }

        [Fact]
        public void IsValid_WithValidProLicense_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "VALID-KEY-123"
            };
            
            // Act & Assert
            Assert.True(license.IsValid);
        }

        [Fact]
        public void IsValid_WithProButNoKey_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = string.Empty
            };
            
            // Act & Assert
            Assert.False(license.IsValid);
        }

        [Fact]
        public void IsValid_WithProButExpiredLicense_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "VALID-KEY-123",
                ExpirationDate = DateTime.Now.AddDays(-1)
            };
            
            // Act & Assert
            Assert.False(license.IsValid);
        }

        [Fact]
        public void IsValid_WithProAndFutureExpiration_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "VALID-KEY-123",
                ExpirationDate = DateTime.Now.AddYears(1)
            };
            
            // Act & Assert
            Assert.True(license.IsValid);
        }

        [Fact]
        public void IsValid_WithProAndNoExpiration_ShouldReturnTrue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                LicenseKey = "VALID-KEY-123",
                ExpirationDate = null // Lifetime license
            };
            
            // Act & Assert
            Assert.True(license.IsValid);
        }

        [Fact]
        public void IsValid_WithFreeLicense_ShouldReturnFalse()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                LicenseKey = string.Empty
            };
            
            // Act & Assert
            Assert.False(license.IsValid);
        }
    }
}
