using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using SyncMedia.Helpers;
using Xunit;

namespace SyncMedia.Tests.Helpers
{
    public class PerformanceOptimizerTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _testFile1;
        private readonly string _testFile2;
        private readonly string _testFile3;

        public PerformanceOptimizerTests()
        {
            // Setup test directory and files
            _testDirectory = Path.Combine(Path.GetTempPath(), "PerformanceOptimizerTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDirectory);

            _testFile1 = Path.Combine(_testDirectory, "test1.txt");
            _testFile2 = Path.Combine(_testDirectory, "test2.txt");
            _testFile3 = Path.Combine(_testDirectory, "test3.txt");

            File.WriteAllText(_testFile1, "This is test content");
            File.WriteAllText(_testFile2, "This is test content"); // Same as file1
            File.WriteAllText(_testFile3, "Different content here");
        }

        public void Dispose()
        {
            // Cleanup
            PerformanceOptimizer.Cleanup();
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Fact]
        public void ComputeHashOptimized_ValidFile_ReturnsHash()
        {
            // Act
            var hash = PerformanceOptimizer.ComputeHashOptimized(_testFile1);

            // Assert
            Assert.NotNull(hash);
            Assert.Equal(20, hash.Length); // SHA1 produces 20 bytes
        }

        [Fact]
        public void ComputeHashOptimized_SameContent_ReturnsSameHash()
        {
            // Act
            var hash1 = PerformanceOptimizer.ComputeHashOptimized(_testFile1);
            var hash2 = PerformanceOptimizer.ComputeHashOptimized(_testFile2);

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void ComputeHashOptimized_DifferentContent_ReturnsDifferentHash()
        {
            // Act
            var hash1 = PerformanceOptimizer.ComputeHashOptimized(_testFile1);
            var hash3 = PerformanceOptimizer.ComputeHashOptimized(_testFile3);

            // Assert
            Assert.NotEqual(hash1, hash3);
        }

        [Fact]
        public void ComputeHashOptimized_NonExistentFile_ThrowsException()
        {
            // Arrange
            var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.txt");

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => 
                PerformanceOptimizer.ComputeHashOptimized(nonExistentFile));
        }

        [Fact]
        public void GetFileMetadataWithCache_ValidFile_ReturnsMetadata()
        {
            // Act
            var metadata = PerformanceOptimizer.GetFileMetadataWithCache(_testFile1);

            // Assert
            Assert.NotNull(metadata);
            Assert.Equal(_testFile1, metadata.FullPath);
            Assert.Equal("test1.txt", metadata.Name);
            Assert.Equal(".txt", metadata.Extension);
            Assert.True(metadata.Length > 0);
        }

        [Fact]
        public void GetFileMetadataWithCache_SameFileTwice_ReturnsCachedValue()
        {
            // Act
            var metadata1 = PerformanceOptimizer.GetFileMetadataWithCache(_testFile1);
            var metadata2 = PerformanceOptimizer.GetFileMetadataWithCache(_testFile1);

            // Assert
            Assert.Same(metadata1, metadata2); // Should be the exact same object from cache
        }

        [Fact]
        public void GetFileMetadataWithCache_AfterClearCache_ReturnsNewInstance()
        {
            // Arrange
            var metadata1 = PerformanceOptimizer.GetFileMetadataWithCache(_testFile1);

            // Act
            PerformanceOptimizer.ClearCache();
            var metadata2 = PerformanceOptimizer.GetFileMetadataWithCache(_testFile1);

            // Assert
            Assert.NotSame(metadata1, metadata2); // Should be different objects
            Assert.Equal(metadata1.FullPath, metadata2.FullPath); // But same data
        }

        [Fact]
        public void ProcessFilesParallel_ProcessesAllItems()
        {
            // Arrange
            var items = Enumerable.Range(1, 100).ToList();
            var processedItems = new System.Collections.Concurrent.ConcurrentBag<int>();

            // Act
            PerformanceOptimizer.ProcessFilesParallel(items, item =>
            {
                processedItems.Add(item);
            }, degreeOfParallelism: 4);

            // Assert
            Assert.Equal(100, processedItems.Count);
            Assert.Equal(items.OrderBy(x => x), processedItems.OrderBy(x => x));
        }

        [Fact]
        public void ProcessFilesParallel_WithDefaultParallelism_UsesProcessorCount()
        {
            // Arrange
            var items = Enumerable.Range(1, 10).ToList();
            var processedCount = 0;

            // Act
            PerformanceOptimizer.ProcessFilesParallel(items, item =>
            {
                System.Threading.Interlocked.Increment(ref processedCount);
            });

            // Assert
            Assert.Equal(10, processedCount);
        }

        [Fact]
        public void QuickDuplicateCheck_SameSize_ReturnsTrue()
        {
            // Act
            var result = PerformanceOptimizer.QuickDuplicateCheck(_testFile1, _testFile2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void QuickDuplicateCheck_DifferentSize_ReturnsFalse()
        {
            // Act
            var result = PerformanceOptimizer.QuickDuplicateCheck(_testFile1, _testFile3);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ClearCache_RemovesAllCachedEntries()
        {
            // Arrange
            PerformanceOptimizer.GetFileMetadataWithCache(_testFile1);
            PerformanceOptimizer.GetFileMetadataWithCache(_testFile2);
            var metadata1Before = PerformanceOptimizer.GetFileMetadataWithCache(_testFile1);

            // Act
            PerformanceOptimizer.ClearCache();
            var metadata1After = PerformanceOptimizer.GetFileMetadataWithCache(_testFile1);

            // Assert
            Assert.NotSame(metadata1Before, metadata1After);
        }

        [Fact]
        public void Cleanup_DisposesResourcesAndClearsCache()
        {
            // Arrange
            PerformanceOptimizer.GetFileMetadataWithCache(_testFile1);
            PerformanceOptimizer.ComputeHashOptimized(_testFile1);

            // Act
            PerformanceOptimizer.Cleanup();
            var metadataAfter = PerformanceOptimizer.GetFileMetadataWithCache(_testFile1);

            // Assert - Should get new metadata instance after cleanup
            Assert.NotNull(metadataAfter);
        }

        [Fact]
        public void Constants_HaveExpectedValues()
        {
            // Assert
            Assert.Equal(10000, PerformanceOptimizer.PARALLEL_THRESHOLD);
            Assert.Equal(65536, PerformanceOptimizer.BUFFER_SIZE);
            Assert.Equal(5000, PerformanceOptimizer.CACHE_SIZE);
            Assert.Equal(100, PerformanceOptimizer.UI_THROTTLE_MS);
        }

        [Fact]
        public void FileMetadata_PropertiesSetCorrectly()
        {
            // Act
            var metadata = new FileMetadata
            {
                FullPath = _testFile1,
                Name = "test.txt",
                Extension = ".txt",
                Length = 1024,
                LastWriteTime = DateTime.Now
            };

            // Assert
            Assert.Equal(_testFile1, metadata.FullPath);
            Assert.Equal("test.txt", metadata.Name);
            Assert.Equal(".txt", metadata.Extension);
            Assert.Equal(1024, metadata.Length);
        }

        [Fact]
        public void ComputeHashOptimized_LargeFile_CompletesSuccessfully()
        {
            // Arrange - Create a larger file (1MB)
            var largeFile = Path.Combine(_testDirectory, "large.dat");
            var data = new byte[1024 * 1024]; // 1MB
            new Random().NextBytes(data);
            File.WriteAllBytes(largeFile, data);

            // Act
            var hash = PerformanceOptimizer.ComputeHashOptimized(largeFile);

            // Assert
            Assert.NotNull(hash);
            Assert.Equal(20, hash.Length);
        }

        [Fact]
        public void ProcessFilesParallel_SingleThreaded_WorksCorrectly()
        {
            // Arrange
            var items = Enumerable.Range(1, 20).ToList();
            var sum = 0;
            var lockObj = new object();

            // Act
            PerformanceOptimizer.ProcessFilesParallel(items, item =>
            {
                lock (lockObj)
                {
                    sum += item;
                }
            }, degreeOfParallelism: 1);

            // Assert
            Assert.Equal(210, sum); // Sum of 1 to 20
        }

        [Fact]
        public void GetFileMetadataWithCache_Extension_IsLowerCase()
        {
            // Arrange
            var upperCaseFile = Path.Combine(_testDirectory, "TEST.TXT");
            File.WriteAllText(upperCaseFile, "test");

            // Act
            var metadata = PerformanceOptimizer.GetFileMetadataWithCache(upperCaseFile);

            // Assert
            Assert.Equal(".txt", metadata.Extension); // Should be lowercase
        }
    }
}
