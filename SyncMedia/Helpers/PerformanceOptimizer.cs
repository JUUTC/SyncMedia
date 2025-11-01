using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SyncMedia.Helpers
{
    /// <summary>
    /// Advanced performance optimizations for file processing
    /// </summary>
    public class PerformanceOptimizer
    {
        // Performance configuration constants
        public const int PARALLEL_THRESHOLD = 10000;      // Files before parallel mode
        public const int BUFFER_SIZE = 65536;             // 64KB buffer for I/O
        public const int CACHE_SIZE = 5000;               // LRU cache entries
        public const int UI_THROTTLE_MS = 100;            // UI update frequency
        
        private static readonly ThreadLocal<SHA1> _sha1Provider = new ThreadLocal<SHA1>(() => SHA1.Create());
        private static readonly ConcurrentDictionary<string, FileMetadata> _metadataCache = 
            new ConcurrentDictionary<string, FileMetadata>();
        private static readonly Queue<string> _lruQueue = new Queue<string>();
        private static readonly object _cacheLock = new object();
        
        /// <summary>
        /// Compute hash with buffered streaming (memory optimized)
        /// </summary>
        public static byte[] ComputeHashOptimized(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, BUFFER_SIZE, FileOptions.SequentialScan))
            using (var bufferedStream = new BufferedStream(fs, BUFFER_SIZE))
            {
                var sha1 = _sha1Provider.Value;
                return sha1.ComputeHash(bufferedStream);
            }
        }
        
        /// <summary>
        /// Get file metadata with LRU caching
        /// </summary>
        public static FileMetadata GetFileMetadataWithCache(string filePath)
        {
            if (_metadataCache.TryGetValue(filePath, out var cached))
            {
                return cached;
            }
            
            var fileInfo = new FileInfo(filePath);
            var metadata = new FileMetadata
            {
                FullPath = filePath,
                Name = fileInfo.Name,
                Extension = fileInfo.Extension.ToLower(),
                Length = fileInfo.Length,
                LastWriteTime = fileInfo.LastWriteTime
            };
            
            // Add to cache with LRU eviction
            lock (_cacheLock)
            {
                if (_metadataCache.Count >= CACHE_SIZE)
                {
                    // Remove oldest entry
                    if (_lruQueue.Count > 0)
                    {
                        var oldestKey = _lruQueue.Dequeue();
                        _metadataCache.TryRemove(oldestKey, out _);
                    }
                }
                
                _metadataCache[filePath] = metadata;
                _lruQueue.Enqueue(filePath);
            }
            
            return metadata;
        }
        
        /// <summary>
        /// Process files in parallel for large datasets
        /// </summary>
        public static void ProcessFilesParallel<T>(
            IEnumerable<T> items,
            Action<T> processAction,
            int degreeOfParallelism = -1)
        {
            if (degreeOfParallelism == -1)
            {
                degreeOfParallelism = Environment.ProcessorCount;
            }
            
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = degreeOfParallelism
            };
            
            Parallel.ForEach(items, options, processAction);
        }
        
        /// <summary>
        /// Quick duplicate check using file size pre-screening
        /// </summary>
        public static bool QuickDuplicateCheck(string filePath1, string filePath2)
        {
            var info1 = new FileInfo(filePath1);
            var info2 = new FileInfo(filePath2);
            
            // Different sizes = definitely not duplicates
            if (info1.Length != info2.Length)
            {
                return false;
            }
            
            // Same size = might be duplicates, need full hash
            return true;
        }
        
        /// <summary>
        /// Clear cache
        /// </summary>
        public static void ClearCache()
        {
            lock (_cacheLock)
            {
                _metadataCache.Clear();
                _lruQueue.Clear();
            }
        }
        
        /// <summary>
        /// Dispose thread-local SHA1 providers
        /// </summary>
        public static void Cleanup()
        {
            _sha1Provider.Dispose();
            ClearCache();
        }
    }
    
    /// <summary>
    /// File metadata for caching
    /// </summary>
    public class FileMetadata
    {
        public string FullPath { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public long Length { get; set; }
        public DateTime LastWriteTime { get; set; }
    }
}
