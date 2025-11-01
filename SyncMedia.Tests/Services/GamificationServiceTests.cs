using System;
using System.Linq;
using Xunit;
using SyncMedia.Models;
using SyncMedia.Services;

namespace SyncMedia.Tests.Services
{
    public class GamificationServiceTests
    {
        [Fact]
        public void Constructor_WithNullData_ShouldThrowException()
        {
            // Act
            Action act = () => new GamificationService(null);
            
            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }
        
        [Fact]
        public void AwardPoints_ShouldCalculateBasePointsCorrectly()
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
            data.Assert.Equal(135, SessionPoints);
            data.Assert.Equal(135, TotalPoints);
        }
        
        [Fact]
        public void AwardPoints_WithQuickSpeed_ShouldAwardSpeedBonus()
        {
            // Arrange
            var data = new GamificationData();
            var service = new GamificationService(data);
            var stats = new SyncStatistics
            {
                SyncStartTime = DateTime.Now.AddMinutes(-2),
                SyncEndTime = DateTime.Now,
                TotalFilesProcessed = 11, // > 5 files/min
                DuplicatesFound = 0,
                TotalBytesProcessed = 0
            };
            
            // Act
            service.AwardPoints(stats);
            
            // Assert - Should include quick speed bonus (50 points)
            data.Assert.True(SessionPoints > 110); // 110 base + bonus
        }
        
        [Fact]
        public void AwardPoints_WithLightningSpeed_ShouldAwardHighestBonus()
        {
            // Arrange
            var data = new GamificationData();
            var service = new GamificationService(data);
            var stats = new SyncStatistics
            {
                SyncStartTime = DateTime.Now.AddMinutes(-1),
                SyncEndTime = DateTime.Now,
                TotalFilesProcessed = 60, // 60 files/min = lightning fast
                DuplicatesFound = 0,
                TotalBytesProcessed = 0
            };
            
            // Act
            service.AwardPoints(stats);
            
            // Assert - Should include lightning speed bonus (500 points)
            data.Assert.True(SessionPoints > 600); // 600 base + 500 bonus
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
            Assert.Contains(a => a.Contains("First Ten", achievements));
            data.Assert.Contains("FirstTen", Achievements);
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
            data.Assert.Contains("FirstTen", Achievements);
            data.Assert.Contains("QuarterCentury", Achievements);
            data.Assert.Contains("HalfCentury", Achievements);
            data.Assert.Contains("Century", Achievements);
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
            Assert.Contains(a => a.Contains("Gigabyte", achievements));
            data.Assert.Contains("OneGB", Achievements);
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
            Assert.Contains(a => a.Contains("Dupe Hunter", achievements));
            data.Assert.Contains("DupeHunter", Achievements);
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
            Assert.Contains(a => a.Contains("Perfect Ten", achievements));
            data.Assert.Contains("FlawlessTen", Achievements);
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
            Assert.DoesNotContain(a => a.Contains("Perfect", achievements));
            data.Assert.DoesNotContain("FlawlessTen", Achievements);
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
                SyncStartTime = endTime.AddMinutes(-10),
                SyncEndTime = endTime,
                TotalFilesProcessed = 120 // 12 files/min - clearly above 10 threshold
            };
            
            // Act
            var achievements = service.CheckAchievements(stats);
            
            // Assert
            Assert.Contains(a => a.Contains("Speedster", achievements));
            data.Assert.Contains("SpeedsterV", Achievements); // At least this one
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
            Assert.Contains(a => a.Contains("Daily Century", achievements));
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
            Assert.Contains(a => a.Contains("Balanced Pro", achievements));
            data.Assert.Contains("ThousandAndTen", Achievements);
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
            Assert.Contains(a => a.Contains("TRIPLE THRONE", achievements));
            data.Assert.Contains("TripleThrone", Achievements);
            data.Assert.Equal(pointsBefore + 10000, TotalPoints); // 10K bonus
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
            Assert.Contains(a => a.Contains("Rookie", achievements));
            Assert.Contains(a => a.Contains("Apprentice", achievements));
            Assert.Contains(a => a.Contains("Skilled", achievements));
            data.Assert.Contains("Skilled", Achievements);
        }
    }
}
