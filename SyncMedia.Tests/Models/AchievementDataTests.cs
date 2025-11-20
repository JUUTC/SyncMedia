using System;
using Xunit;
using SyncMedia.Core.Models;

namespace SyncMedia.Tests.Models
{
    public class AchievementDataTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var achievement = new AchievementData();

            // Assert
            Assert.Equal(string.Empty, achievement.Id);
            Assert.Equal(string.Empty, achievement.Name);
            Assert.Equal(string.Empty, achievement.Description);
            Assert.Equal(string.Empty, achievement.UnlockCriteria);
            Assert.Equal("\uE73E", achievement.IconGlyph); // Default checkmark
            Assert.False(achievement.IsUnlocked);
            Assert.Null(achievement.UnlockedDate);
            Assert.Equal(0, achievement.Progress);
            Assert.Equal(0, achievement.Target);
        }

        [Fact]
        public void Properties_ShouldBeSettableAndGettable()
        {
            // Arrange
            var achievement = new AchievementData();
            var testDate = DateTime.Now;

            // Act
            achievement.Id = "test-achievement";
            achievement.Name = "Test Achievement";
            achievement.Description = "Test Description";
            achievement.UnlockCriteria = "Complete 10 tasks";
            achievement.IconGlyph = "\uE734";
            achievement.IsUnlocked = true;
            achievement.UnlockedDate = testDate;
            achievement.Progress = 75;
            achievement.Target = 100;

            // Assert
            Assert.Equal("test-achievement", achievement.Id);
            Assert.Equal("Test Achievement", achievement.Name);
            Assert.Equal("Test Description", achievement.Description);
            Assert.Equal("Complete 10 tasks", achievement.UnlockCriteria);
            Assert.Equal("\uE734", achievement.IconGlyph);
            Assert.True(achievement.IsUnlocked);
            Assert.Equal(testDate, achievement.UnlockedDate);
            Assert.Equal(75, achievement.Progress);
            Assert.Equal(100, achievement.Target);
        }

        [Fact]
        public void IsUnlocked_WhenSet_ShouldRetainValue()
        {
            // Arrange
            var achievement = new AchievementData();

            // Act
            achievement.IsUnlocked = true;

            // Assert
            Assert.True(achievement.IsUnlocked);
        }

        [Fact]
        public void UnlockedDate_WhenSet_ShouldRetainValue()
        {
            // Arrange
            var achievement = new AchievementData();
            var date = new DateTime(2024, 1, 15);

            // Act
            achievement.UnlockedDate = date;

            // Assert
            Assert.Equal(date, achievement.UnlockedDate);
        }

        [Fact]
        public void Progress_ShouldTrackCompletion()
        {
            // Arrange
            var achievement = new AchievementData
            {
                Target = 100
            };

            // Act
            achievement.Progress = 50;

            // Assert
            Assert.Equal(50, achievement.Progress);
            Assert.Equal(100, achievement.Target);
            Assert.False(achievement.Progress >= achievement.Target);
        }

        [Fact]
        public void Progress_WhenReachesTarget_ShouldBeComplete()
        {
            // Arrange
            var achievement = new AchievementData
            {
                Target = 100,
                Progress = 100
            };

            // Assert
            Assert.True(achievement.Progress >= achievement.Target);
        }
    }
}
