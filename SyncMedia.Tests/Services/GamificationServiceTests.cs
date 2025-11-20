using System;
using System.Linq;
using Xunit;
using SyncMedia.Core.Constants;
using SyncMedia.Core.Models;
using SyncMedia.Core.Services;

namespace SyncMedia.Tests.Services
{
    public class GamificationServiceTests
    {
        [Fact]
        public void Constructor_WithNullData_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GamificationService(null!));
        }

        [Fact]
        public void AwardPoints_WithBasicStats_ShouldCalculatePointsCorrectly()
        {
            // Arrange
            var data = new GamificationData();
            var service = new GamificationService(data);
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 10,
                DuplicatesFound = 5,
                TotalBytesProcessed = 10 * 1024 * 1024 // 10 MB
            };

            // Act
            service.AwardPoints(stats);

            // Assert
            // 10 files * 10 points = 100
            // 5 duplicates * 5 points = 25
            // 10 MB * 1 point = 10
            // Total = 135
            Assert.Equal(135, data.SessionPoints);
            Assert.Equal(135, data.TotalPoints);
        }

        [Fact]
        public void AwardPoints_WithQuickSpeed_ShouldAwardSpeedBonus()
        {
            // Arrange
            var data = new GamificationData();
            var service = new GamificationService(data);
            var stats = new SyncStatistics
            {
                StartTime = DateTime.Now.AddMinutes(-2),
                EndTime = DateTime.Now,
                Duration = TimeSpan.FromMinutes(2),
                ProcessedFiles = 11, // 5.5 files/min - above quick threshold (5.0)
                TotalFilesProcessed = 11,
                DuplicatesFound = 0,
                TotalBytesProcessed = 0
            };

            // Act
            service.AwardPoints(stats);

            // Assert - Should include quick speed bonus (50 points)
            Assert.True(data.SessionPoints >= 110 + MediaConstants.SPEED_QUICK_BONUS); // 110 base + 50 bonus
        }

        [Fact]
        public void AwardPoints_WithLightningSpeed_ShouldAwardHighestBonus()
        {
            // Arrange
            var data = new GamificationData();
            var service = new GamificationService(data);
            var stats = new SyncStatistics
            {
                StartTime = DateTime.Now.AddMinutes(-1),
                EndTime = DateTime.Now,
                Duration = TimeSpan.FromMinutes(1),
                ProcessedFiles = 60, // 60 files/min - lightning fast
                TotalFilesProcessed = 60,
                DuplicatesFound = 0,
                TotalBytesProcessed = 0
            };

            // Act
            service.AwardPoints(stats);

            // Assert - Should include lightning speed bonus (500 points)
            Assert.True(data.SessionPoints >= 600 + MediaConstants.SPEED_LIGHTNING_BONUS); // 600 base + 500 bonus
        }

        [Fact]
        public void CheckAchievements_WithFirstTenFiles_ShouldUnlockAchievement()
        {
            // Arrange
            var data = new GamificationData { TotalFilesLifetime = 10 };
            var service = new GamificationService(data);
            var stats = new SyncStatistics();

            // Act
            var achievements = service.CheckAchievements(stats);

            // Assert
            Assert.Contains(achievements, a => a.Contains("First Ten"));
            Assert.True(data.HasAchievement("FirstTen"));
        }

        [Fact]
        public void CheckAchievements_WithCentury_ShouldUnlockMultipleAchievements()
        {
            // Arrange
            var data = new GamificationData { TotalFilesLifetime = 100 };
            var service = new GamificationService(data);
            var stats = new SyncStatistics();

            // Act
            var achievements = service.CheckAchievements(stats);

            // Assert
            Assert.True(achievements.Count > 0);
            Assert.True(data.HasAchievement("FirstTen"));
            Assert.True(data.HasAchievement("QuarterCentury"));
            Assert.True(data.HasAchievement("HalfCentury"));
            Assert.True(data.HasAchievement("Century"));
        }

        [Fact]
        public void CheckAchievements_WithGigabyte_ShouldUnlockDataAchievement()
        {
            // Arrange
            var data = new GamificationData
            {
                TotalBytesLifetime = 1024L * 1024 * 1024 * 1 // 1 GB
            };
            var service = new GamificationService(data);
            var stats = new SyncStatistics();

            // Act
            var achievements = service.CheckAchievements(stats);

            // Assert
            Assert.Contains(achievements, a => a.Contains("Gigabyte"));
            Assert.True(data.HasAchievement("OneGB"));
        }

        [Fact]
        public void CheckAchievements_WithDuplicates_ShouldUnlockDupeHunter()
        {
            // Arrange
            var data = new GamificationData { TotalDuplicatesLifetime = 50 };
            var service = new GamificationService(data);
            var stats = new SyncStatistics();

            // Act
            var achievements = service.CheckAchievements(stats);

            // Assert
            Assert.Contains(achievements, a => a.Contains("Dupe Hunter"));
            Assert.True(data.HasAchievement("DupeHunter"));
        }

        [Fact]
        public void CheckAchievements_WithPerfectSession_ShouldUnlockFlawlessAchievement()
        {
            // Arrange
            var data = new GamificationData();
            var service = new GamificationService(data);
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 10,
                ErrorsEncountered = 0
            };

            // Act
            var achievements = service.CheckAchievements(stats);

            // Assert
            Assert.Contains(achievements, a => a.Contains("Perfect Ten"));
            Assert.True(data.HasAchievement("FlawlessTen"));
        }

        [Fact]
        public void CheckAchievements_WithErrors_ShouldNotUnlockFlawlessAchievement()
        {
            // Arrange
            var data = new GamificationData();
            var service = new GamificationService(data);
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 10,
                ErrorsEncountered = 1
            };

            // Act
            var achievements = service.CheckAchievements(stats);

            // Assert
            Assert.DoesNotContain(achievements, a => a.Contains("Perfect"));
            Assert.False(data.HasAchievement("FlawlessTen"));
        }

        [Fact]
        public void CheckAchievements_WithHighSpeed_ShouldUnlockSpeedAchievement()
        {
            // Arrange
            var data = new GamificationData();
            var service = new GamificationService(data);
            var endTime = DateTime.Now;
            var stats = new SyncStatistics
            {
                StartTime = endTime.AddMinutes(-10),
                EndTime = endTime,
                Duration = TimeSpan.FromMinutes(10),
                ProcessedFiles = 120, // 12 files/min - clearly above 10 threshold
                TotalFilesProcessed = 120
            };

            // Act
            var achievements = service.CheckAchievements(stats);

            // Assert
            Assert.Contains(achievements, a => a.Contains("Speedster"));
            Assert.True(data.HasAchievement("SpeedsterV")); // At least this one
        }

        [Fact]
        public void CheckAchievements_WithDailySession_ShouldUnlockDailyAchievement()
        {
            // Arrange
            var data = new GamificationData();
            var service = new GamificationService(data);
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 100
            };

            // Act
            var achievements = service.CheckAchievements(stats);

            // Assert
            Assert.Contains(achievements, a => a.Contains("Daily Century"));
        }

        [Fact]
        public void CheckAchievements_WithComboConditions_ShouldUnlockComboAchievement()
        {
            // Arrange
            var data = new GamificationData
            {
                TotalFilesLifetime = 1000,
                TotalBytesLifetime = 10L * 1024 * 1024 * 1024 // 10 GB
            };
            var service = new GamificationService(data);
            var stats = new SyncStatistics();

            // Act
            var achievements = service.CheckAchievements(stats);

            // Assert
            Assert.Contains(achievements, a => a.Contains("Balanced Pro"));
            Assert.True(data.HasAchievement("ThousandAndTen"));
        }

        [Fact]
        public void CheckAchievements_WithTripleThrone_ShouldUnlockAndAwardBonusPoints()
        {
            // Arrange
            var data = new GamificationData
            {
                TotalFilesLifetime = 100000,
                TotalBytesLifetime = 1024L * 1024 * 1024 * 1024, // 1 TB
                TotalPoints = 1000000
            };
            var service = new GamificationService(data);
            var stats = new SyncStatistics();

            var pointsBefore = data.TotalPoints;

            // Act
            var achievements = service.CheckAchievements(stats);

            // Assert
            Assert.Contains(achievements, a => a.Contains("TRIPLE THRONE"));
            Assert.True(data.HasAchievement("TripleThrone"));
            Assert.Equal(pointsBefore + 10000, data.TotalPoints); // 10K bonus
        }

        [Fact]
        public void CheckAchievements_AlreadyUnlocked_ShouldNotUnlockAgain()
        {
            // Arrange
            var data = new GamificationData { TotalFilesLifetime = 100 };
            var service = new GamificationService(data);
            var stats = new SyncStatistics();

            // Act - First check
            var firstCheck = service.CheckAchievements(stats);

            // Act - Second check
            var secondCheck = service.CheckAchievements(stats);

            // Assert
            Assert.True(firstCheck.Count > 0);
            Assert.Empty(secondCheck);
        }

        [Fact]
        public void CheckAchievements_WithPointsMilestones_ShouldUnlockProgressively()
        {
            // Arrange
            var data = new GamificationData { TotalPoints = 10000 };
            var service = new GamificationService(data);
            var stats = new SyncStatistics();

            // Act
            var achievements = service.CheckAchievements(stats);

            // Assert
            Assert.Contains(achievements, a => a.Contains("Rookie"));
            Assert.Contains(achievements, a => a.Contains("Apprentice"));
            Assert.Contains(achievements, a => a.Contains("Skilled"));
            Assert.True(data.HasAchievement("Skilled"));
        }

        [Fact]
        public void AwardPoints_ShouldResetSessionPoints()
        {
            // Arrange
            var data = new GamificationData { SessionPoints = 500 };
            var service = new GamificationService(data);
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 1,
                DuplicatesFound = 0,
                TotalBytesProcessed = 0
            };

            // Act
            service.AwardPoints(stats);

            // Assert
            Assert.Equal(10, data.SessionPoints); // Only 1 file * 10 points
        }

        [Fact]
        public void AwardPoints_ShouldUpdateLifetimeStats()
        {
            // Arrange
            var data = new GamificationData();
            var service = new GamificationService(data);
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 10,
                DuplicatesFound = 5,
                TotalBytesProcessed = 1024 * 1024 * 100
            };

            // Act
            service.AwardPoints(stats);

            // Assert
            Assert.Equal(10, data.TotalFilesLifetime);
            Assert.Equal(5, data.TotalDuplicatesLifetime);
            Assert.Equal(1024 * 1024 * 100, data.TotalBytesLifetime);
        }

        [Fact]
        public void CheckAchievements_WithLargeNumbers_ShouldUnlockHighTierAchievements()
        {
            // Arrange
            var data = new GamificationData
            {
                TotalFilesLifetime = 1000000, // 1 million files
                TotalBytesLifetime = 10L * 1024 * 1024 * 1024 * 1024, // 10 TB
                TotalDuplicatesLifetime = 10000,
                TotalPoints = 10000000
            };
            var service = new GamificationService(data);
            var stats = new SyncStatistics();

            // Act
            var achievements = service.CheckAchievements(stats);

            // Assert
            Assert.Contains(achievements, a => a.Contains("MILLIONAIRE"));
            Assert.Contains(achievements, a => a.Contains("GODLIKE"));
            Assert.Contains(achievements, a => a.Contains("DUPE DESTROYER"));
            Assert.Contains(achievements, a => a.Contains("TRANSCENDENT"));
        }
    }
}
