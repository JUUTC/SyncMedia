using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SyncMedia.Helpers
{
    /// <summary>
    /// Auto-tuning benchmark system for optimal parallel processing configuration
    /// </summary>
    public class PerformanceBenchmark
    {
        private const int BENCHMARK_SAMPLE_SIZE = 100;        // Files to test with
        private const int MIN_THRESHOLD = 1000;               // Minimum file count for parallel
        private const int MAX_THRESHOLD = 50000;              // Maximum file count for parallel
        private const int THRESHOLD_STEP = 1000;              // Step size for testing
        
        /// <summary>
        /// Benchmark results for a specific configuration
        /// </summary>
        public class BenchmarkResult
        {
            public int FileCount { get; set; }
            public int DegreeOfParallelism { get; set; }
            public long SequentialTimeMs { get; set; }
            public long ParallelTimeMs { get; set; }
            public double SpeedupFactor { get; set; }
            public bool ParallelIsFaster { get; set; }
            public int RecommendedThreshold { get; set; }
        }
        
        /// <summary>
        /// Auto-tune parallel processing threshold based on system capabilities
        /// </summary>
        public static int AutoTuneParallelThreshold(string testDirectory, int sampleSize = BENCHMARK_SAMPLE_SIZE)
        {
            if (!Directory.Exists(testDirectory))
            {
                return PerformanceOptimizer.PARALLEL_THRESHOLD; // Return default if no test directory
            }
            
            var testFiles = Directory.GetFiles(testDirectory, "*.*", SearchOption.AllDirectories)
                                     .Take(sampleSize)
                                     .ToList();
            
            if (testFiles.Count < 10)
            {
                return PerformanceOptimizer.PARALLEL_THRESHOLD; // Not enough files to benchmark
            }
            
            var results = new List<BenchmarkResult>();
            
            // Test different file counts to find break-even point
            var testCounts = new[] { 100, 500, 1000, 5000, 10000 };
            
            foreach (var fileCount in testCounts)
            {
                if (fileCount > testFiles.Count) continue;
                
                var subset = testFiles.Take(fileCount).ToList();
                var result = BenchmarkFileProcessing(subset);
                results.Add(result);
                
                if (result.ParallelIsFaster && result.SpeedupFactor > 1.2) // 20% speedup threshold
                {
                    return result.FileCount; // Found optimal threshold
                }
            }
            
            // Return the smallest file count where parallel is faster
            var optimal = results.FirstOrDefault(r => r.ParallelIsFaster && r.SpeedupFactor > 1.2);
            return optimal?.FileCount ?? PerformanceOptimizer.PARALLEL_THRESHOLD;
        }
        
        /// <summary>
        /// Benchmark sequential vs parallel processing for a file set
        /// </summary>
        public static BenchmarkResult BenchmarkFileProcessing(List<string> files)
        {
            var degreeOfParallelism = Environment.ProcessorCount;
            
            // Benchmark sequential processing
            var sw = Stopwatch.StartNew();
            foreach (var file in files)
            {
                SimulateFileProcessing(file);
            }
            sw.Stop();
            var sequentialTime = sw.ElapsedMilliseconds;
            
            // Benchmark parallel processing
            sw.Restart();
            PerformanceOptimizer.ProcessFilesParallel(files, SimulateFileProcessing, degreeOfParallelism);
            sw.Stop();
            var parallelTime = sw.ElapsedMilliseconds;
            
            var speedup = sequentialTime / (double)Math.Max(1, parallelTime);
            
            return new BenchmarkResult
            {
                FileCount = files.Count,
                DegreeOfParallelism = degreeOfParallelism,
                SequentialTimeMs = sequentialTime,
                ParallelTimeMs = parallelTime,
                SpeedupFactor = speedup,
                ParallelIsFaster = parallelTime < sequentialTime,
                RecommendedThreshold = CalculateRecommendedThreshold(files.Count, speedup)
            };
        }
        
        /// <summary>
        /// Simulate file processing (hash computation)
        /// </summary>
        private static void SimulateFileProcessing(string filePath)
        {
            try
            {
                // Use actual hash computation for realistic benchmark
                var hash = PerformanceOptimizer.ComputeHashOptimized(filePath);
            }
            catch
            {
                // Ignore errors during benchmarking
            }
        }
        
        /// <summary>
        /// Calculate recommended threshold based on observed speedup
        /// </summary>
        private static int CalculateRecommendedThreshold(int fileCount, double speedup)
        {
            // If speedup is less than 1.2x, recommend higher threshold
            if (speedup < 1.2)
            {
                return fileCount * 2;
            }
            
            // If speedup is good, recommend current file count
            if (speedup >= 1.5)
            {
                return Math.Max(1000, fileCount / 2);
            }
            
            return fileCount;
        }
        
        /// <summary>
        /// Get system performance profile
        /// </summary>
        public static SystemPerformanceProfile GetSystemProfile()
        {
            return new SystemPerformanceProfile
            {
                ProcessorCount = Environment.ProcessorCount,
                Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                Is64BitProcess = Environment.Is64BitProcess,
                SystemPageSize = Environment.SystemPageSize,
                WorkingSet = Environment.WorkingSet,
                RecommendedParallelThreshold = EstimateOptimalThreshold()
            };
        }
        
        /// <summary>
        /// Estimate optimal threshold based on CPU cores
        /// </summary>
        private static int EstimateOptimalThreshold()
        {
            var cores = Environment.ProcessorCount;
            
            // More cores = lower threshold beneficial
            if (cores >= 8) return 5000;
            if (cores >= 4) return 10000;
            if (cores >= 2) return 15000;
            return 20000;
        }
    }
    
    /// <summary>
    /// System performance profile
    /// </summary>
    public class SystemPerformanceProfile
    {
        public int ProcessorCount { get; set; }
        public bool Is64BitOperatingSystem { get; set; }
        public bool Is64BitProcess { get; set; }
        public int SystemPageSize { get; set; }
        public long WorkingSet { get; set; }
        public int RecommendedParallelThreshold { get; set; }
        
        public override string ToString()
        {
            return $"CPU Cores: {ProcessorCount}, 64-bit: {Is64BitOperatingSystem}, " +
                   $"Recommended Threshold: {RecommendedParallelThreshold} files";
        }
    }
}
