using Xunit;
using SyncMedia.Core.Models;
using System;

namespace SyncMedia.Tests.Models
{
    public class LicenseInfoTests
    {
        [Fact]
        public void Constructor_CreatesInstance()
        {
            // Act
            var license = new LicenseInfo();
            
            // Assert
            Assert.NotNull(license);
            Assert.False(license.IsPro);
            Assert.Null(license.LicenseKey);
        }
        
        [Fact]
        public void IsInTrial_ReturnsTrueWhenActive()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                TrialExpirationDate = DateTime.Now.AddDays(5)
            };
            
            // Act
            var result = license.IsInTrial;
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void IsInTrial_ReturnsFalseWhenExpired()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = false,
                TrialExpirationDate = DateTime.Now.AddDays(-1)
            };
            
            // Act
            var result = license.IsInTrial;
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void IsInTrial_ReturnsFalseForPro()
        {
            // Arrange
            var license = new LicenseInfo
            {
                IsPro = true,
                TrialExpirationDate = DateTime.Now.AddDays(5)
            };
            
            // Act
            var result = license.IsInTrial;
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void TrialDaysRemaining_ReturnsCorrectValue()
        {
            // Arrange
            var license = new LicenseInfo
            {
                TrialExpirationDate = DateTime.Now.AddDays(7)
            };
            
            // Act
            var days = license.TrialDaysRemaining;
            
            // Assert
            Assert.True(days >= 6 && days <= 7); // Account for time passing during test
        }
    }
}
