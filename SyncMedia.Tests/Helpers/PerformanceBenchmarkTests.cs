using Xunit;
using SyncMedia.Core.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SyncMedia.Tests.Helpers
{
    public class PerformanceBenchmarkTests
    {
        [Fact]
        public void AutoTuneParallelThreshold_WithNonExistentDirectory_ReturnsDefaultThreshold()
        {
            // Act
            var threshold = PerformanceBenchmark.AutoTuneParallelThreshold("/nonexistent/path");
            
            // Assert
            Assert.Equal(PerformanceOptimizer.PARALLEL_THRESHOLD, threshold);
        }
        
        [Fact]
        public void BenchmarkFileProcessing_WithValidFiles_ReturnsBenchmarkResult()
        {
            // Arrange - Create temporary test file
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "test content");
            var files = new List<string> { tempFile, tempFile, tempFile };
            
            try
            {
                // Act
                var result = PerformanceBenchmark.BenchmarkFileProcessing(files);
                
                // Assert
                Assert.NotNull(result);
                Assert.Equal(3, result.FileCount);
                Assert.True(result.DegreeOfParallelism > 0);
                Assert.True(result.SequentialTimeMs >= 0);
                Assert.True(result.ParallelTimeMs >= 0);
                Assert.True(result.SpeedupFactor > 0);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
        
        [Fact]
        public void GetSystemProfile_ReturnsValidProfile()
        {
            // Act
            var profile = PerformanceBenchmark.GetSystemProfile();
            
            // Assert
            Assert.NotNull(profile);
            Assert.True(profile.ProcessorCount > 0);
            Assert.True(profile.RecommendedParallelThreshold > 0);
            Assert.True(profile.SystemPageSize > 0);
        }
        
        [Fact]
        public void BenchmarkResult_CalculatesSpeedupCorrectly()
        {
            // Arrange
            var tempFile = Path.GetTempFileName();
            File.WriteAllText(tempFile, "test");
            var files = Enumerable.Repeat(tempFile, 5).ToList();
            
            try
            {
                // Act
                var result = PerformanceBenchmark.BenchmarkFileProcessing(files);
                
                // Assert
                Assert.True(result.SpeedupFactor > 0);
                Assert.Equal(result.ParallelTimeMs < result.SequentialTimeMs, result.ParallelIsFaster);
            }
            finally
            {
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }
    }
}
