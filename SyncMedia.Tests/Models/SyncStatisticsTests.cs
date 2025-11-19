using System;
using Xunit;
using SyncMedia.Core.Models;

namespace SyncMedia.Tests.Models
{
    public class SyncStatisticsTests
    {
        [Fact]
        public void Constructor_ShouldInitializeWithDefaultValues()
        {
            // Act
            var stats = new SyncStatistics();
            
            // Assert
            Assert.Equal(0, stats.TotalFiles);
            Assert.Equal(0, stats.ProcessedFiles);
            Assert.Equal(0, stats.SuccessfulFiles);
            Assert.Equal(0, stats.SkippedFiles);
            Assert.Equal(0, stats.ErrorFiles);
            Assert.Equal(0, stats.DuplicatesFound);
            Assert.Equal(0, stats.TotalBytesProcessed);
            Assert.Equal(string.Empty, stats.Status);
            Assert.Equal(string.Empty, stats.ErrorMessage);
        }

        [Fact]
        public void ElapsedTime_ShouldCalculateFromStartAndEndTime()
        {
            // Arrange
            var stats = new SyncStatistics
            {
                StartTime = new DateTime(2024, 1, 1, 10, 0, 0),
                EndTime = new DateTime(2024, 1, 1, 10, 5, 30)
            };
            
            // Act
            var elapsed = stats.ElapsedTime;
            
            // Assert
            Assert.Equal(TimeSpan.FromMinutes(5).Add(TimeSpan.FromSeconds(30)), elapsed);
        }

        [Fact]
        public void ElapsedTime_WithDuration_ShouldUseDuration()
        {
            // Arrange
            var expectedDuration = TimeSpan.FromMinutes(10);
            var stats = new SyncStatistics
            {
                StartTime = new DateTime(2024, 1, 1, 10, 0, 0),
                EndTime = new DateTime(2024, 1, 1, 10, 5, 0),
                Duration = expectedDuration
            };
            
            // Act
            var elapsed = stats.ElapsedTime;
            
            // Assert
            Assert.Equal(expectedDuration, elapsed);
        }

        [Fact]
        public void TotalMBProcessed_ShouldConvertBytesToMegabytes()
        {
            // Arrange
            var stats = new SyncStatistics
            {
                TotalBytesProcessed = 10 * 1024 * 1024 // 10 MB
            };
            
            // Act
            var mb = stats.TotalMBProcessed;
            
            // Assert
            Assert.Equal(10.0, mb, 2);
        }

        [Fact]
        public void ProcessingSpeedMBPerSecond_ShouldCalculateCorrectly()
        {
            // Arrange
            var stats = new SyncStatistics
            {
                TotalBytesProcessed = 10 * 1024 * 1024, // 10 MB
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddSeconds(10),
                Duration = TimeSpan.FromSeconds(10)
            };
            
            // Act
            var speed = stats.ProcessingSpeedMBPerSecond;
            
            // Assert
            Assert.Equal(1.0, speed, 2); // 10 MB / 10 seconds = 1 MB/s
        }

        [Fact]
        public void ProcessingSpeedMBPerSecond_WithZeroTime_ShouldReturnZero()
        {
            // Arrange
            var stats = new SyncStatistics
            {
                TotalBytesProcessed = 10 * 1024 * 1024,
                Duration = TimeSpan.Zero
            };
            
            // Act
            var speed = stats.ProcessingSpeedMBPerSecond;
            
            // Assert
            Assert.Equal(0, speed);
        }

        [Fact]
        public void FilesPerMinute_ShouldCalculateCorrectly()
        {
            // Arrange
            var stats = new SyncStatistics
            {
                ProcessedFiles = 60,
                StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddMinutes(10),
                Duration = TimeSpan.FromMinutes(10)
            };
            
            // Act
            var filesPerMinute = stats.FilesPerMinute;
            
            // Assert
            Assert.Equal(6.0, filesPerMinute, 2); // 60 files / 10 minutes = 6 files/min
        }

        [Fact]
        public void FilesPerMinute_WithZeroTime_ShouldReturnZero()
        {
            // Arrange
            var stats = new SyncStatistics
            {
                ProcessedFiles = 60,
                Duration = TimeSpan.Zero
            };
            
            // Act
            var filesPerMinute = stats.FilesPerMinute;
            
            // Assert
            Assert.Equal(0, filesPerMinute);
        }

        [Fact]
        public void Reset_ShouldResetAllProperties()
        {
            // Arrange
            var stats = new SyncStatistics
            {
                TotalFiles = 100,
                ProcessedFiles = 50,
                SuccessfulFiles = 45,
                SkippedFiles = 3,
                ErrorFiles = 2,
                DuplicatesFound = 5,
                TotalBytesProcessed = 1024 * 1024,
                Status = "Completed",
                ErrorMessage = "Some error",
                Duration = TimeSpan.FromMinutes(5)
            };
            
            // Act
            stats.Reset();
            
            // Assert
            Assert.Equal(0, stats.TotalFiles);
            Assert.Equal(0, stats.ProcessedFiles);
            Assert.Equal(0, stats.SuccessfulFiles);
            Assert.Equal(0, stats.SkippedFiles);
            Assert.Equal(0, stats.ErrorFiles);
            Assert.Equal(0, stats.DuplicatesFound);
            Assert.Equal(0, stats.TotalBytesProcessed);
            Assert.Equal(string.Empty, stats.Status);
            Assert.Equal(string.Empty, stats.ErrorMessage);
            Assert.Equal(TimeSpan.Zero, stats.Duration);
        }

        [Fact]
        public void LegacyProperty_TotalFilesProcessed_ShouldMapToProcessedFiles()
        {
            // Arrange
            var stats = new SyncStatistics();
            
            // Act
            stats.TotalFilesProcessed = 42;
            
            // Assert
            Assert.Equal(42, stats.ProcessedFiles);
            Assert.Equal(42, stats.TotalFilesProcessed);
        }

        [Fact]
        public void LegacyProperty_ErrorsEncountered_ShouldMapToErrorFiles()
        {
            // Arrange
            var stats = new SyncStatistics();
            
            // Act
            stats.ErrorsEncountered = 5;
            
            // Assert
            Assert.Equal(5, stats.ErrorFiles);
            Assert.Equal(5, stats.ErrorsEncountered);
        }

        [Fact]
        public void LegacyProperty_SyncStartTime_ShouldMapToStartTime()
        {
            // Arrange
            var stats = new SyncStatistics();
            var startTime = new DateTime(2024, 1, 1, 10, 0, 0);
            
            // Act
            stats.SyncStartTime = startTime;
            
            // Assert
            Assert.Equal(startTime, stats.StartTime);
            Assert.Equal(startTime, stats.SyncStartTime);
        }

        [Fact]
        public void LegacyProperty_SyncEndTime_ShouldMapToEndTime()
        {
            // Arrange
            var stats = new SyncStatistics();
            var endTime = new DateTime(2024, 1, 1, 10, 30, 0);
            
            // Act
            stats.SyncEndTime = endTime;
            
            // Assert
            Assert.Equal(endTime, stats.EndTime);
            Assert.Equal(endTime, stats.SyncEndTime);
        }

        [Fact]
        public void AllProperties_ShouldBeSettableAndGettable()
        {
            // Arrange
            var stats = new SyncStatistics();
            
            // Act
            stats.TotalFiles = 100;
            stats.ProcessedFiles = 90;
            stats.SuccessfulFiles = 85;
            stats.SkippedFiles = 3;
            stats.ErrorFiles = 2;
            stats.DuplicatesFound = 5;
            stats.TotalBytesProcessed = 1024 * 1024 * 50;
            stats.Status = "Completed";
            stats.ErrorMessage = "No errors";
            stats.StartTime = DateTime.Now;
            stats.EndTime = DateTime.Now.AddMinutes(5);
            stats.Duration = TimeSpan.FromMinutes(5);
            
            // Assert
            Assert.Equal(100, stats.TotalFiles);
            Assert.Equal(90, stats.ProcessedFiles);
            Assert.Equal(85, stats.SuccessfulFiles);
            Assert.Equal(3, stats.SkippedFiles);
            Assert.Equal(2, stats.ErrorFiles);
            Assert.Equal(5, stats.DuplicatesFound);
            Assert.Equal(1024 * 1024 * 50, stats.TotalBytesProcessed);
            Assert.Equal("Completed", stats.Status);
            Assert.Equal("No errors", stats.ErrorMessage);
            Assert.Equal(TimeSpan.FromMinutes(5), stats.Duration);
        }
    }
}
