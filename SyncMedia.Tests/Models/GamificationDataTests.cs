using Xunit;
using SyncMedia.Core.Models;

namespace SyncMedia.Tests.Models
{
    public class GamificationDataTests
    {
        [Fact]
        public void Constructor_InitializesDefaults()
        {
            // Act
            var data = new GamificationData();
            
            // Assert
            Assert.Equal(0, data.TotalPoints);
            Assert.Equal(0, data.SessionPoints);
            Assert.Empty(data.Achievements);
        }
        
        [Fact]
        public void AddSessionPoints_IncreasesPoints()
        {
            // Arrange
            var data = new GamificationData();
            
            // Act
            data.AddSessionPoints(100);
            
            // Assert
            Assert.Equal(100, data.SessionPoints);
        }
        
        [Fact]
        public void ResetSessionPoints_ClearsSessionPoints()
        {
            // Arrange
            var data = new GamificationData();
            data.AddSessionPoints(100);
            
            // Act
            data.ResetSessionPoints();
            
            // Assert
            Assert.Equal(0, data.SessionPoints);
        }
        
        [Fact]
        public void AddSessionPoints_AlsoIncreasesTotalPoints()
        {
            // Arrange
            var data = new GamificationData();
            data.AddSessionPoints(150);
            
            // Act  
            var totalBefore = data.TotalPoints;
            data.AddSessionPoints(50);
            
            // Assert
            Assert.Equal(200, data.TotalPoints);
            Assert.Equal(200, data.SessionPoints);
        }
    }
}
