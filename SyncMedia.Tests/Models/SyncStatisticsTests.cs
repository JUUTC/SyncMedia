using Xunit;
using SyncMedia.Core.Models;
using System;

namespace SyncMedia.Tests.Models
{
    public class SyncStatisticsTests
    {
        [Fact]
        public void Constructor_InitializesDefaults()
        {
            // Act
            var stats = new SyncStatistics();
            
            // Assert
            Assert.Equal(0, stats.TotalFilesProcessed);
            Assert.Equal(0, stats.DuplicatesFound);
            Assert.Equal(0, stats.TotalBytesProcessed);
        }
        
        [Fact]
        public void FilesPerMinute_CalculatesCorrectly()
        {
            // Arrange
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 120,
                Duration = TimeSpan.FromMinutes(2)
            };
            
            // Act
            var rate = stats.FilesPerMinute;
            
            // Assert
            Assert.Equal(60, rate);
        }
        
        [Fact]
        public void FilesPerMinute_ReturnsZeroForZeroDuration()
        {
            // Arrange
            var stats = new SyncStatistics
            {
                TotalFilesProcessed = 100,
                Duration = TimeSpan.Zero
            };
            
            // Act
            var rate = stats.FilesPerMinute;
            
            // Assert
            Assert.Equal(0, rate);
        }
    }
}
