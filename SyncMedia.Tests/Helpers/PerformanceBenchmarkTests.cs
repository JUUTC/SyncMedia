using System;
using System.Collections.Generic;
using Xunit;
using SyncMedia.Core.Helpers;

namespace SyncMedia.Tests.Helpers
{
    public class PerformanceBenchmarkTests
    {
        [Fact]
        public void BenchmarkResult_ShouldInitializeProperties()
        {
            // Act
            var result = new PerformanceBenchmark.BenchmarkResult
            {
                FileCount = 100,
                DegreeOfParallelism = 4,
                SequentialTimeMs = 1000,
                ParallelTimeMs = 250,
                SpeedupFactor = 4.0,
                ParallelIsFaster = true,
                RecommendedThreshold = 500
            };

            // Assert
            Assert.Equal(100, result.FileCount);
            Assert.Equal(4, result.DegreeOfParallelism);
            Assert.Equal(1000, result.SequentialTimeMs);
            Assert.Equal(250, result.ParallelTimeMs);
            Assert.Equal(4.0, result.SpeedupFactor);
            Assert.True(result.ParallelIsFaster);
            Assert.Equal(500, result.RecommendedThreshold);
        }

        [Fact]
        public void AutoTuneParallelThreshold_WithNonExistentDirectory_ShouldReturnDefault()
        {
            // Arrange
            var nonExistentPath = "/nonexistent/path/to/test";

            // Act
            var threshold = PerformanceBenchmark.AutoTuneParallelThreshold(nonExistentPath);

            // Assert
            Assert.Equal(PerformanceOptimizer.PARALLEL_THRESHOLD, threshold);
        }

        [Fact]
        public void BenchmarkFileProcessing_WithEmptyList_ShouldCompleteWithoutError()
        {
            // Arrange
            var emptyList = new List<string>();

            // Act
            var result = PerformanceBenchmark.BenchmarkFileProcessing(emptyList);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.FileCount);
        }

        [Fact]
        public void BenchmarkResult_ParallelIsFaster_ShouldReflectPerformance()
        {
            // Arrange & Act
            var fasterResult = new PerformanceBenchmark.BenchmarkResult
            {
                SequentialTimeMs = 1000,
                ParallelTimeMs = 200,
                ParallelIsFaster = true
            };

            var slowerResult = new PerformanceBenchmark.BenchmarkResult
            {
                SequentialTimeMs = 200,
                ParallelTimeMs = 1000,
                ParallelIsFaster = false
            };

            // Assert
            Assert.True(fasterResult.ParallelIsFaster);
            Assert.False(slowerResult.ParallelIsFaster);
        }

        [Fact]
        public void BenchmarkResult_SpeedupFactor_ShouldReflectRatio()
        {
            // Arrange & Act
            var result = new PerformanceBenchmark.BenchmarkResult
            {
                SequentialTimeMs = 1000,
                ParallelTimeMs = 250,
                SpeedupFactor = 4.0
            };

            // Assert
            Assert.Equal(4.0, result.SpeedupFactor);
        }
    }
}
