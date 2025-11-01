using System.Collections.Generic;
using System.Linq;
using Xunit;
using SyncMedia.Models;

namespace SyncMedia.Tests.Models
{
    public class GamificationDataTests
    {
        [Fact]
        public void GamificationData_InitialState_ShouldHaveZeroValues()
        {
            // Arrange & Act
            var data = new GamificationData();
            
            // Assert
            Assert.Equal(0, data.TotalPoints);
            Assert.Equal(0, data.SessionPoints);
            Assert.Equal(0, data.TotalFilesLifetime);
            Assert.Equal(0, data.TotalDuplicatesLifetime);
            Assert.Equal(0, data.TotalBytesLifetime);
            Assert.NotNull(data.Achievements);
            Assert.Empty(data.Achievements);
        }
        
        [Fact]
        public void ResetSessionPoints_ShouldResetToZero()
        {
            // Arrange
            var data = new GamificationData { SessionPoints = 1000 };
            
            // Act
            data.ResetSessionPoints();
            
            // Assert
            Assert.Equal(0, data.SessionPoints);
        }
        
        [Fact]
        public void AddSessionPoints_ShouldIncreaseBothSessionAndTotal()
        {
            // Arrange
            var data = new GamificationData();
            
            // Act
            data.AddSessionPoints(100);
            data.AddSessionPoints(50);
            
            // Assert
            Assert.Equal(150, data.SessionPoints);
            Assert.Equal(150, data.TotalPoints);
        }
        
        [Fact]
        public void UpdateLifetimeStats_ShouldAddToLifetimeValues()
        {
            // Arrange
            var data = new GamificationData
            {
                TotalFilesLifetime = 100,
                TotalDuplicatesLifetime = 10,
                TotalBytesLifetime = 1000000
            };
            
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 50,
                DuplicatesFound = 5,
                TotalBytesProcessed = 500000
            };
            
            // Act
            data.UpdateLifetimeStats(stats);
            
            // Assert
            Assert.Equal(150, data.TotalFilesLifetime);
            Assert.Equal(15, data.TotalDuplicatesLifetime);
            Assert.Equal(1500000, data.TotalBytesLifetime);
        }
        
        [Fact]
        public void HasAchievement_WithExistingAchievement_ShouldReturnTrue()
        {
            // Arrange
            var data = new GamificationData();
            data.Achievements.Add("TestAchievement");
            
            // Act
            var result = data.HasAchievement("TestAchievement");
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void HasAchievement_WithNonExistingAchievement_ShouldReturnFalse()
        {
            // Arrange
            var data = new GamificationData();
            
            // Act
            var result = data.HasAchievement("NonExisting");
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void AddAchievement_ShouldAddNewAchievement()
        {
            // Arrange
            var data = new GamificationData();
            
            // Act
            data.AddAchievement("NewAchievement");
            
            // Assert
            Assert.Contains("NewAchievement", data.Achievements);
        }
        
        [Fact]
        public void AddAchievement_WithDuplicate_ShouldNotAddTwice()
        {
            // Arrange
            var data = new GamificationData();
            data.Achievements.Add("Achievement1");
            
            // Act
            data.AddAchievement("Achievement1");
            
            // Assert
            Assert.Equal(1, data.Achievements.Count(a => a == "Achievement1"));
        }
    }
}
