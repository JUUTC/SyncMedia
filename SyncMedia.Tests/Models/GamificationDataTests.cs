using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
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
            data.TotalPoints.Should().Be(0);
            data.SessionPoints.Should().Be(0);
            data.TotalFilesLifetime.Should().Be(0);
            data.TotalDuplicatesLifetime.Should().Be(0);
            data.TotalBytesLifetime.Should().Be(0);
            data.Achievements.Should().NotBeNull().And.BeEmpty();
        }
        
        [Fact]
        public void ResetSessionPoints_ShouldResetToZero()
        {
            // Arrange
            var data = new GamificationData { SessionPoints = 1000 };
            
            // Act
            data.ResetSessionPoints();
            
            // Assert
            data.SessionPoints.Should().Be(0);
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
            data.SessionPoints.Should().Be(150);
            data.TotalPoints.Should().Be(150);
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
            data.TotalFilesLifetime.Should().Be(150);
            data.TotalDuplicatesLifetime.Should().Be(15);
            data.TotalBytesLifetime.Should().Be(1500000);
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
            result.Should().BeTrue();
        }
        
        [Fact]
        public void HasAchievement_WithNonExistingAchievement_ShouldReturnFalse()
        {
            // Arrange
            var data = new GamificationData();
            
            // Act
            var result = data.HasAchievement("NonExisting");
            
            // Assert
            result.Should().BeFalse();
        }
        
        [Fact]
        public void AddAchievement_ShouldAddNewAchievement()
        {
            // Arrange
            var data = new GamificationData();
            
            // Act
            data.AddAchievement("NewAchievement");
            
            // Assert
            data.Achievements.Should().Contain("NewAchievement");
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
            data.Achievements.Count(a => a == "Achievement1").Should().Be(1);
        }
    }
}
