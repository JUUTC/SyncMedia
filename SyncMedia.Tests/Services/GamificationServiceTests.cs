using Xunit;
using SyncMedia.Core.Services;
using SyncMedia.Core.Models;
using SyncMedia.Core.Constants;

namespace SyncMedia.Tests.Services
{
    public class GamificationServiceTests
    {
        [Fact]
        public void Constructor_WithData_InitializesCorrectly()
        {
            // Arrange
            var data = new GamificationData();
            
            // Act
            var service = new GamificationService(data);
            
            // Assert
            Assert.NotNull(service);
            Assert.NotNull(service.GetGamificationData());
        }
        
        [Fact]
        public void GetGamificationData_ReturnsData()
        {
            // Arrange
            var data = new GamificationData { TotalPoints = 100 };
            var service = new GamificationService(data);
            
            // Act
            var result = service.GetGamificationData();
            
            // Assert
            Assert.Equal(100, result.TotalPoints);
        }
        
        [Fact]
        public void AwardPoints_CalculatesBasePoints()
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
            Assert.True(data.SessionPoints > 0);
            Assert.True(data.SessionPoints >= 10 * MediaConstants.POINTS_PER_FILE);
        }
        
        [Fact]
        public void AwardPoints_WithHighSpeed_AddsSpeedBonus()
        {
            // Arrange
            var data = new GamificationData();
            var service = new GamificationService(data);
            var startTime = DateTime.Now.AddMinutes(-1);
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 100,
                DuplicatesFound = 10,
                TotalBytesProcessed = 50 * 1024 * 1024,
                SyncStartTime = startTime,
                SyncEndTime = DateTime.Now // 100 files in 1 minute = high speed
            };
            
            // Act
            service.AwardPoints(stats);
            
            // Assert
            Assert.True(data.SessionPoints > 0);
        }
        
        [Fact]
        public void CheckAchievements_FindsNewAchievements()
        {
            // Arrange
            var data = new GamificationData();
            var service = new GamificationService(data);
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 100,
                DuplicatesFound = 50,
                TotalBytesProcessed = 100 * 1024 * 1024
            };
            
            // Act
            var newAchievements = service.CheckAchievements(stats);
            
            // Assert
            Assert.NotNull(newAchievements);
        }
        
        [Fact]
        public void AwardPoints_UpdatesLifetimeStatistics()
        {
            // Arrange
            var data = new GamificationData();
            var service = new GamificationService(data);
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 50,
                DuplicatesFound = 10,
                TotalBytesProcessed = 25 * 1024 * 1024
            };
            
            // Act
            service.AwardPoints(stats);
            
            // Assert
            Assert.Equal(50, data.TotalFilesLifetime);
            Assert.Equal(10, data.TotalDuplicatesLifetime);
            Assert.Equal(25 * 1024 * 1024, data.TotalBytesLifetime);
        }
    }
}
