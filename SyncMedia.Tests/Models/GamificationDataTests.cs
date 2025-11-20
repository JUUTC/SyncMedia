using System.Collections.Generic;
using Xunit;
using SyncMedia.Core.Models;

namespace SyncMedia.Tests.Models
{
    public class GamificationDataTests
    {
        [Fact]
        public void Constructor_ShouldInitializeAchievementsList()
        {
            // Act
            var data = new GamificationData();
            
            // Assert
            Assert.NotNull(data.Achievements);
            Assert.Empty(data.Achievements);
        }

        [Fact]
        public void Constructor_ShouldInitializeWithZeroValues()
        {
            // Act
            var data = new GamificationData();
            
            // Assert
            Assert.Equal(0, data.TotalPoints);
            Assert.Equal(0, data.SessionPoints);
            Assert.Equal(0, data.TotalFilesLifetime);
            Assert.Equal(0, data.TotalDuplicatesLifetime);
            Assert.Equal(0, data.TotalBytesLifetime);
        }

        [Fact]
        public void ResetSessionPoints_ShouldSetSessionPointsToZero()
        {
            // Arrange
            var data = new GamificationData { SessionPoints = 100 };
            
            // Act
            data.ResetSessionPoints();
            
            // Assert
            Assert.Equal(0, data.SessionPoints);
        }

        [Fact]
        public void AddSessionPoints_ShouldIncreaseBothSessionAndTotalPoints()
        {
            // Arrange
            var data = new GamificationData
            {
                SessionPoints = 50,
                TotalPoints = 200
            };
            
            // Act
            data.AddSessionPoints(100);
            
            // Assert
            Assert.Equal(150, data.SessionPoints);
            Assert.Equal(300, data.TotalPoints);
        }

        [Fact]
        public void AddSessionPoints_MultipleAdds_ShouldAccumulateCorrectly()
        {
            // Arrange
            var data = new GamificationData();
            
            // Act
            data.AddSessionPoints(50);
            data.AddSessionPoints(30);
            data.AddSessionPoints(20);
            
            // Assert
            Assert.Equal(100, data.SessionPoints);
            Assert.Equal(100, data.TotalPoints);
        }

        [Fact]
        public void UpdateLifetimeStats_ShouldAddStatsToLifetimeTotals()
        {
            // Arrange
            var data = new GamificationData
            {
                TotalFilesLifetime = 50,
                TotalDuplicatesLifetime = 10,
                TotalBytesLifetime = 1024 * 1024 * 100
            };
            
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 20,
                DuplicatesFound = 5,
                TotalBytesProcessed = 1024 * 1024 * 50
            };
            
            // Act
            data.UpdateLifetimeStats(stats);
            
            // Assert
            Assert.Equal(70, data.TotalFilesLifetime);
            Assert.Equal(15, data.TotalDuplicatesLifetime);
            Assert.Equal(1024 * 1024 * 150, data.TotalBytesLifetime);
        }

        [Fact]
        public void HasAchievement_WithExistingAchievement_ShouldReturnTrue()
        {
            // Arrange
            var data = new GamificationData();
            data.Achievements.Add("FirstTen");
            
            // Act
            var hasAchievement = data.HasAchievement("FirstTen");
            
            // Assert
            Assert.True(hasAchievement);
        }

        [Fact]
        public void HasAchievement_WithNonExistingAchievement_ShouldReturnFalse()
        {
            // Arrange
            var data = new GamificationData();
            
            // Act
            var hasAchievement = data.HasAchievement("FirstTen");
            
            // Assert
            Assert.False(hasAchievement);
        }

        [Fact]
        public void AddAchievement_NewAchievement_ShouldAddToList()
        {
            // Arrange
            var data = new GamificationData();
            
            // Act
            data.AddAchievement("FirstTen");
            
            // Assert
            Assert.Contains("FirstTen", data.Achievements);
        }

        [Fact]
        public void AddAchievement_DuplicateAchievement_ShouldNotAddAgain()
        {
            // Arrange
            var data = new GamificationData();
            data.AddAchievement("FirstTen");
            
            // Act
            data.AddAchievement("FirstTen");
            
            // Assert
            Assert.Single(data.Achievements);
        }

        [Fact]
        public void AddAchievement_MultipleAchievements_ShouldAddAll()
        {
            // Arrange
            var data = new GamificationData();
            
            // Act
            data.AddAchievement("FirstTen");
            data.AddAchievement("Century");
            data.AddAchievement("Speedster");
            
            // Assert
            Assert.Equal(3, data.Achievements.Count);
            Assert.Contains("FirstTen", data.Achievements);
            Assert.Contains("Century", data.Achievements);
            Assert.Contains("Speedster", data.Achievements);
        }

        [Fact]
        public void ResetSessionPoints_ShouldNotAffectTotalPoints()
        {
            // Arrange
            var data = new GamificationData
            {
                SessionPoints = 100,
                TotalPoints = 500
            };
            
            // Act
            data.ResetSessionPoints();
            
            // Assert
            Assert.Equal(0, data.SessionPoints);
            Assert.Equal(500, data.TotalPoints); // Total should remain unchanged
        }
    }
}
