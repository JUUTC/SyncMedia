using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SyncMedia.Core.Helpers;
using SyncMedia.Core.Models;

namespace SyncMedia.Core.Services
{
    /// <summary>
    /// Core sync engine for file synchronization and duplicate detection
    /// </summary>
    public class SyncService
    {
        // Media extension sets (optimized with HashSet for O(1) lookup)
        private static readonly HashSet<string> ImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff",
            ".webp", ".heic", ".heif", ".avif", ".jxl"
        };
        
        private static readonly HashSet<string> VideoExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".mov", ".mp4", ".wmv", ".avi", ".m4v", ".mpg", ".mpeg",
            ".webm", ".mkv", ".flv", ".ts", ".mts", ".3gp", ".3g2", ".ogv", ".vob"
        };

        private static readonly HashSet<string> MusicExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a", ".opus"
        };

        private static readonly HashSet<string> DocumentExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt"
        };

        // Progress reporting
        public event EventHandler<SyncProgressEventArgs>? ProgressChanged;
        public event EventHandler<FileProcessedEventArgs>? FileProcessed;
        public event EventHandler<SyncCompletedEventArgs>? SyncCompleted;

        private List<string> _storedHashes = new List<string>();
        private List<string> _namingExclusions = new List<string>();
        private SyncStatistics _statistics = new SyncStatistics();
        private DateTime _syncStartTime;
        private readonly ILogger<SyncService>? _logger;
        private readonly IErrorHandler? _errorHandler;

        /// <summary>
        /// Initializes a new instance of the SyncService class
        /// </summary>
        public SyncService()
        {
            LoadConfiguration();
        }

        /// <summary>
        /// Initializes a new instance of the SyncService class with logging and error handling
        /// </summary>
        /// <param name="logger">Logger instance for diagnostics</param>
        /// <param name="errorHandler">Error handler for centralized error management</param>
        public SyncService(ILogger<SyncService>? logger, IErrorHandler? errorHandler) : this()
        {
            _logger = logger;
            _errorHandler = errorHandler;
        }

        private void LoadConfiguration()
        {
            // Load stored hashes
            string hashesStr = XmlData.ReadSetting("Hashes");
            if (!string.IsNullOrEmpty(hashesStr))
            {
                _storedHashes = hashesStr.Split('|').ToList();
            }

            // Load naming exclusions
            string namingListStr = XmlData.ReadSetting("NamingList");
            if (!string.IsNullOrEmpty(namingListStr))
            {
                _namingExclusions = namingListStr.Split('|').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            }
        }

        /// <summary>
        /// Start sync operation with input validation, error handling, and logging
        /// </summary>
        /// <param name="sourceFolder">Source folder path</param>
        /// <param name="destinationFolder">Destination folder path</param>
        /// <param name="options">Sync options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Sync result with statistics</returns>
        public async Task<SyncResult> StartSyncAsync(
            string sourceFolder,
            string destinationFolder,
            SyncOptions options,
            CancellationToken cancellationToken = default)
        {
            _logger?.LogInformation("Starting sync operation from {SourceFolder} to {DestinationFolder}", 
                sourceFolder, destinationFolder);
            
            _syncStartTime = DateTime.Now;
            _statistics = new SyncStatistics();
            _statistics.StartTime = _syncStartTime;

            try
            {
                // Validate input paths
                if (!PathValidator.IsValidDirectoryPath(sourceFolder, mustExist: false, out var sourceError))
                {
                    var errorMsg = $"Invalid source folder: {sourceError}";
                    _logger?.LogError(errorMsg);
                    _errorHandler?.LogError(errorMsg, "StartSyncAsync");
                    throw new ArgumentException(errorMsg, nameof(sourceFolder));
                }

                if (!PathValidator.IsValidDirectoryPath(destinationFolder, mustExist: false, out var destError))
                {
                    var errorMsg = $"Invalid destination folder: {destError}";
                    _logger?.LogError(errorMsg);
                    _errorHandler?.LogError(errorMsg, "StartSyncAsync");
                    throw new ArgumentException(errorMsg, nameof(destinationFolder));
                }

                // Check source exists
                if (!Directory.Exists(sourceFolder))
                {
                    var errorMsg = $"Source folder not found: {sourceFolder}";
                    _logger?.LogError(errorMsg);
                    _errorHandler?.LogError(errorMsg, "StartSyncAsync");
                    throw new DirectoryNotFoundException(errorMsg);
                }

                // Create destination if it doesn't exist
                if (!Directory.Exists(destinationFolder))
                {
                    _logger?.LogInformation("Creating destination folder: {DestinationFolder}", destinationFolder);
                    Directory.CreateDirectory(destinationFolder);
                }

                // Get all files from source
                _logger?.LogInformation("Scanning source folder for files...");
                var files = await Task.Run(() => GetFilesRecursive(sourceFolder, options), cancellationToken);
                
                _statistics.TotalFiles = files.Count;
                _logger?.LogInformation("Found {FileCount} files to process", files.Count);

                // Process files
                for (int i = 0; i < files.Count; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        _logger?.LogWarning("Sync operation cancelled by user");
                        _statistics.Status = "Cancelled";
                        break;
                    }

                    var filePath = files[i];
                    try
                    {
                        await ProcessFileAsync(filePath, sourceFolder, destinationFolder, options, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "Error processing file: {FilePath}", filePath);
                        _errorHandler?.HandleException(ex, customMessage: $"Error processing file: {filePath}");
                        _statistics.ErrorFiles++;
                    }

                    // Report progress
                    _statistics.ProcessedFiles = i + 1;
                    var progress = (double)(i + 1) / files.Count * 100;
                    
                    ProgressChanged?.Invoke(this, new SyncProgressEventArgs
                    {
                        ProgressPercentage = progress,
                        CurrentFile = Path.GetFileName(filePath),
                        FilesProcessed = i + 1,
                        TotalFiles = files.Count,
                        ElapsedTime = DateTime.Now - _syncStartTime
                    });
                }

                // Save updated hashes
                _logger?.LogDebug("Saving updated hash list");
                XmlData.AddUpdateAppSettings("Hashes", string.Join("|", _storedHashes));

                _statistics.Status = cancellationToken.IsCancellationRequested ? "Cancelled" : "Completed";
                _statistics.EndTime = DateTime.Now;
                _statistics.Duration = _statistics.EndTime - _syncStartTime;

                _logger?.LogInformation("Sync completed: {Status}. Processed {ProcessedFiles}/{TotalFiles} files in {Duration}",
                    _statistics.Status, _statistics.ProcessedFiles, _statistics.TotalFiles, _statistics.Duration);

                // Raise completion event
                SyncCompleted?.Invoke(this, new SyncCompletedEventArgs
                {
                    Statistics = _statistics,
                    WasCancelled = cancellationToken.IsCancellationRequested
                });

                return new SyncResult
                {
                    Success = true,
                    Statistics = _statistics,
                    WasCancelled = cancellationToken.IsCancellationRequested
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Sync operation failed with error: {ErrorMessage}", ex.Message);
                _errorHandler?.HandleException(ex, customMessage: "Sync operation failed");
                
                _statistics.Status = "Failed";
                _statistics.ErrorMessage = ex.Message;
                _statistics.EndTime = DateTime.Now;
                _statistics.Duration = _statistics.EndTime - _syncStartTime;
                
                return new SyncResult
                {
                    Success = false,
                    Statistics = _statistics,
                    ErrorMessage = ex.Message
                };
            }
        }

        private List<string> GetFilesRecursive(string folder, SyncOptions options)
        {
            var files = new List<string>();

            try
            {
                // Get all files in current directory
                var allFiles = Directory.GetFiles(folder);

                foreach (var file in allFiles)
                {
                    var ext = Path.GetExtension(file);
                    
                    bool include = false;
                    if (options.IncludeImages && ImageExtensions.Contains(ext))
                        include = true;
                    else if (options.IncludeVideos && VideoExtensions.Contains(ext))
                        include = true;
                    else if (options.IncludeMusic && MusicExtensions.Contains(ext))
                        include = true;
                    else if (options.IncludeDocuments && DocumentExtensions.Contains(ext))
                        include = true;
                    else if (options.CustomExtensions != null && options.CustomExtensions.Contains(ext))
                        include = true;

                    if (include)
                    {
                        // Check naming exclusions
                        var fileName = Path.GetFileNameWithoutExtension(file);
                        bool excluded = _namingExclusions.Any(pattern =>
                            fileName.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0);

                        if (!excluded)
                            files.Add(file);
                    }
                }

                // Recursively get files from subdirectories
                var subDirectories = Directory.GetDirectories(folder);
                foreach (var subDir in subDirectories)
                {
                    files.AddRange(GetFilesRecursive(subDir, options));
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Skip inaccessible directories
            }

            return files;
        }

        private async Task ProcessFileAsync(
            string filePath,
            string sourceFolder,
            string destinationFolder,
            SyncOptions options,
            CancellationToken cancellationToken)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                var hash = await ComputeFileHashAsync(filePath, cancellationToken);
                
                var result = new FileProcessResult
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath,
                    SizeBytes = fileInfo.Length,
                    LastModified = fileInfo.LastWriteTime,
                    Hash = hash
                };

                // Check if duplicate
                if (_storedHashes.Contains(hash))
                {
                    result.Status = "Skipped";
                    result.Action = "Duplicate";
                    result.StatusIcon = "⊘";
                    _statistics.SkippedFiles++;
                    _statistics.DuplicatesFound++;
                }
                else
                {
                    // Copy file to destination
                    var relativePath = Path.GetRelativePath(sourceFolder, filePath);
                    var destPath = Path.Combine(destinationFolder, relativePath);
                    var destDir = Path.GetDirectoryName(destPath);

                    if (!Directory.Exists(destDir))
                        Directory.CreateDirectory(destDir);

                    await Task.Run(() => File.Copy(filePath, destPath, true), cancellationToken);

                    _storedHashes.Add(hash);

                    result.Status = "Success";
                    result.Action = "Copied";
                    result.StatusIcon = "✓";
                    _statistics.SuccessfulFiles++;
                    _statistics.TotalBytesProcessed += fileInfo.Length;
                }

                // Raise file processed event
                FileProcessed?.Invoke(this, new FileProcessedEventArgs
                {
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _statistics.ErrorFiles++;

                var result = new FileProcessResult
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath,
                    Status = "Error",
                    Action = "Failed",
                    StatusIcon = "✗",
                    ErrorMessage = ex.Message
                };

                FileProcessed?.Invoke(this, new FileProcessedEventArgs
                {
                    Result = result
                });
            }
        }

        private async Task<string> ComputeFileHashAsync(string filePath, CancellationToken cancellationToken)
        {
            using (var md5 = MD5.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = await md5.ComputeHashAsync(stream, cancellationToken);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        public SyncStatistics GetStatistics()
        {
            return _statistics;
        }
    }

    // Event argument classes
    public class SyncProgressEventArgs : EventArgs
    {
        public double ProgressPercentage { get; set; }
        public string CurrentFile { get; set; }
        public int FilesProcessed { get; set; }
        public int TotalFiles { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }

    public class FileProcessedEventArgs : EventArgs
    {
        public FileProcessResult Result { get; set; }
    }

    public class SyncCompletedEventArgs : EventArgs
    {
        public SyncStatistics Statistics { get; set; }
        public bool WasCancelled { get; set; }
    }

    // Supporting classes
    public class SyncOptions
    {
        public bool IncludeImages { get; set; } = true;
        public bool IncludeVideos { get; set; } = true;
        public bool IncludeMusic { get; set; } = false;
        public bool IncludeDocuments { get; set; } = false;
        public HashSet<string> CustomExtensions { get; set; }
    }

    public class SyncResult
    {
        public bool Success { get; set; }
        public SyncStatistics Statistics { get; set; }
        public string ErrorMessage { get; set; }
        public bool WasCancelled { get; set; }
    }

    public class FileProcessResult
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public long SizeBytes { get; set; }
        public DateTime LastModified { get; set; }
        public string Hash { get; set; }
        public string Status { get; set; } // Success, Error, Skipped
        public string StatusIcon { get; set; } // ✓, ✗, ⊘
        public string Action { get; set; } // Copied, Skipped, Failed
        public string ErrorMessage { get; set; }

        public string FormattedSize
        {
            get
            {
                const long KB = 1024;
                const long MB = KB * 1024;
                const long GB = MB * 1024;

                if (SizeBytes >= GB)
                    return $"{SizeBytes / (double)GB:F2} GB";
                if (SizeBytes >= MB)
                    return $"{SizeBytes / (double)MB:F2} MB";
                if (SizeBytes >= KB)
                    return $"{SizeBytes / (double)KB:F2} KB";

                return $"{SizeBytes} bytes";
            }
        }
    }
}
