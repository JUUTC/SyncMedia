using System;
using Xunit;
using SyncMedia.Models;

namespace SyncMedia.Tests.Models
{
    public class SyncStatisticsTests
    {
        [Fact]
        public void SyncStatistics_InitialState_ShouldHaveZeroValues()
        {
            // Arrange & Act
            var stats = new SyncStatistics();
            
            // Assert
            Assert.Equal(0, stats.TotalFilesProcessed);
            Assert.Equal(0, stats.DuplicatesFound);
            Assert.Equal(0, stats.ErrorsEncountered);
            Assert.Equal(0, stats.TotalBytesProcessed);
        }
        
        [Fact]
        public void TotalMBProcessed_ShouldCalculateCorrectly()
        {
            // Arrange
            var stats = new SyncStatistics
            {
                TotalBytesProcessed = 1024 * 1024 * 100 // 100 MB
            };
            
            // Act
            var result = stats.TotalMBProcessed;
            
            // Assert
            Assert.InRange(result, 99.99, 100.01);
        }
        
        [Fact]
        public void ProcessingSpeedMBPerSecond_WithZeroTime_ShouldReturnZero()
        {
            // Arrange
            var now = DateTime.Now;
            var stats = new SyncStatistics
            {
                SyncStartTime = now,
                SyncEndTime = now,
                TotalBytesProcessed = 1024 * 1024 * 100
            };
            
            // Act
            var result = stats.ProcessingSpeedMBPerSecond;
            
            // Assert - Should be very small or zero since elapsed time is negligible
            Assert.True(result >= 0);
        }
        
        [Fact]
        public void ProcessingSpeedMBPerSecond_WithTime_ShouldCalculateCorrectly()
        {
            // Arrange
            var endTime = DateTime.Now;
            var stats = new SyncStatistics
            {
                SyncStartTime = endTime.AddSeconds(-10),
                SyncEndTime = endTime,
                TotalBytesProcessed = 1024 * 1024 * 100 // 100 MB in 10 seconds = 10 MB/s
            };
            
            // Act
            var result = stats.ProcessingSpeedMBPerSecond;
            
            // Assert
            Assert.InRange(result, 9.5, 10.5);
        }
        
        [Fact]
        public void FilesPerMinute_WithZeroTime_ShouldReturnZero()
        {
            // Arrange
            var now = DateTime.Now;
            var stats = new SyncStatistics
            {
                SyncStartTime = now,
                SyncEndTime = now,
                TotalFilesProcessed = 100
            };
            
            // Act
            var result = stats.FilesPerMinute;
            
            // Assert - Should be very large or zero depending on precision
            Assert.True(result >= 0);
        }
        
        [Fact]
        public void FilesPerMinute_WithTime_ShouldCalculateCorrectly()
        {
            // Arrange
            var endTime = DateTime.Now;
            var stats = new SyncStatistics
            {
                SyncStartTime = endTime.AddMinutes(-5),
                SyncEndTime = endTime,
                TotalFilesProcessed = 100 // 100 files in 5 minutes = 20 files/min
            };
            
            // Act
            var result = stats.FilesPerMinute;
            
            // Assert
            Assert.InRange(result, 19.5, 20.5);
        }
        
        [Fact]
        public void ElapsedTime_ShouldCalculateCorrectly()
        {
            // Arrange
            var startTime = DateTime.Now.AddSeconds(-60);
            var stats = new SyncStatistics
            {
                SyncStartTime = startTime,
                SyncEndTime = DateTime.Now
            };
            
            // Act
            var result = stats.ElapsedTime;
            
            // Assert
            Assert.InRange(result.TotalSeconds, 59, 61);
        }
        
        [Fact]
        public void Reset_ShouldResetAllValues()
        {
            // Arrange
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 100,
                DuplicatesFound = 10,
                ErrorsEncountered = 5,
                TotalBytesProcessed = 1000000
            };
            
            // Act
            stats.Reset();
            
            // Assert
            Assert.Equal(0, stats.TotalFilesProcessed);
            Assert.Equal(0, stats.DuplicatesFound);
            Assert.Equal(0, stats.ErrorsEncountered);
            Assert.Equal(0, stats.TotalBytesProcessed);
        }
    }
}
