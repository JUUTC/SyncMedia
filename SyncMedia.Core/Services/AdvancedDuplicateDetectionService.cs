using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SyncMedia.Core.Services
{
    /// <summary>
    /// Service for advanced duplicate detection using Python/imagededup
    /// </summary>
    public class AdvancedDuplicateDetectionService
    {
        private readonly string _pythonScriptPath;
        private readonly string _pythonExecutable;

        public AdvancedDuplicateDetectionService()
        {
            // Get the path to the Python script
            var assemblyPath = Path.GetDirectoryName(typeof(AdvancedDuplicateDetectionService).Assembly.Location);
            _pythonScriptPath = Path.Combine(assemblyPath, "Python", "find_duplicates.py");
            
            // Try to find Python executable
            _pythonExecutable = FindPythonExecutable();
        }

        /// <summary>
        /// Find Python executable in common locations
        /// </summary>
        private string FindPythonExecutable()
        {
            // Try common Python executable names
            var pythonNames = new[] { "python", "python3", "py" };
            
            foreach (var name in pythonNames)
            {
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = name,
                            Arguments = "--version",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        }
                    };
                    
                    process.Start();
                    process.WaitForExit(1000);
                    
                    if (process.ExitCode == 0)
                    {
                        return name;
                    }
                }
                catch
                {
                    // Continue trying other names
                }
            }
            
            // Default to "python"
            return "python";
        }

        /// <summary>
        /// Check if Python and required dependencies are installed
        /// </summary>
        public async Task<PythonEnvironmentStatus> CheckEnvironmentAsync()
        {
            try
            {
                var testInput = new
                {
                    images = new string[] { },
                    method = "phash",
                    threshold = 0.9
                };
                
                var inputJson = JsonSerializer.Serialize(testInput);
                var result = await ExecutePythonScriptAsync(inputJson, CancellationToken.None);
                
                return new PythonEnvironmentStatus
                {
                    IsAvailable = true,
                    PythonVersion = await GetPythonVersionAsync(),
                    HasGpuSupport = result.Contains("gpu_used", StringComparison.OrdinalIgnoreCase),
                    Message = "Python environment is ready"
                };
            }
            catch (Exception ex)
            {
                return new PythonEnvironmentStatus
                {
                    IsAvailable = false,
                    Message = $"Python environment not available: {ex.Message}",
                    ErrorDetails = ex.ToString()
                };
            }
        }

        /// <summary>
        /// Get Python version
        /// </summary>
        private async Task<string> GetPythonVersionAsync()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _pythonExecutable,
                        Arguments = "--version",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                
                process.Start();
                var output = await process.StandardOutput.ReadToEndAsync();
                var error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();
                
                return string.IsNullOrEmpty(output) ? error : output;
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Find duplicate images using advanced detection methods
        /// </summary>
        public async Task<AdvancedDuplicateResult> FindDuplicatesAsync(
            List<string> imagePaths,
            DetectionMethod method = DetectionMethod.PHash,
            double threshold = 0.9,
            bool useGpu = false,
            CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_pythonScriptPath))
            {
                return new AdvancedDuplicateResult
                {
                    Success = false,
                    ErrorMessage = $"Python script not found: {_pythonScriptPath}"
                };
            }

            try
            {
                // Prepare input
                var input = new
                {
                    images = imagePaths,
                    method = method.ToString().ToLower(),
                    threshold = threshold,
                    use_gpu = useGpu
                };

                var inputJson = JsonSerializer.Serialize(input);
                
                // Execute Python script
                var outputJson = await ExecutePythonScriptAsync(inputJson, cancellationToken);
                
                // Parse result
                var result = JsonSerializer.Deserialize<PythonDuplicateResult>(outputJson);
                
                return new AdvancedDuplicateResult
                {
                    Success = result?.success ?? false,
                    Method = method,
                    GpuUsed = result?.gpu_used ?? false,
                    DuplicateGroups = result?.duplicates ?? new Dictionary<string, List<string>>(),
                    TotalFiles = result?.statistics?.total_files ?? 0,
                    ValidFiles = result?.statistics?.valid_files ?? 0,
                    DuplicateGroupCount = result?.statistics?.duplicate_groups ?? 0,
                    TotalDuplicates = result?.statistics?.total_duplicates ?? 0,
                    ErrorMessage = result?.error
                };
            }
            catch (Exception ex)
            {
                return new AdvancedDuplicateResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to execute Python script: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Execute Python script with JSON input
        /// </summary>
        private async Task<string> ExecutePythonScriptAsync(string inputJson, CancellationToken cancellationToken)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _pythonExecutable,
                    Arguments = $"\"{_pythonScriptPath}\" \"{inputJson.Replace("\"", "\\\"")}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await Task.WhenAll(outputTask, errorTask);
            await process.WaitForExitAsync(cancellationToken);

            if (process.ExitCode != 0)
            {
                var error = await errorTask;
                throw new Exception($"Python script failed: {error}");
            }

            return await outputTask;
        }
    }

    /// <summary>
    /// Detection method for duplicate detection
    /// </summary>
    public enum DetectionMethod
    {
        PHash,  // Perceptual Hash (fast, good for exact duplicates)
        DHash,  // Difference Hash (fast, good for similar images)
        WHash,  // Wavelet Hash (slower, more accurate)
        CNN     // Convolutional Neural Network (slowest, most accurate, requires GPU for speed)
    }

    /// <summary>
    /// Result from advanced duplicate detection
    /// </summary>
    public class AdvancedDuplicateResult
    {
        public bool Success { get; set; }
        public DetectionMethod Method { get; set; }
        public bool GpuUsed { get; set; }
        public Dictionary<string, List<string>> DuplicateGroups { get; set; }
        public int TotalFiles { get; set; }
        public int ValidFiles { get; set; }
        public int DuplicateGroupCount { get; set; }
        public int TotalDuplicates { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Status of Python environment
    /// </summary>
    public class PythonEnvironmentStatus
    {
        public bool IsAvailable { get; set; }
        public string PythonVersion { get; set; }
        public bool HasGpuSupport { get; set; }
        public string Message { get; set; }
        public string ErrorDetails { get; set; }
    }

    // Internal classes for JSON deserialization
    internal class PythonDuplicateResult
    {
        public bool success { get; set; }
        public string method { get; set; }
        public bool gpu_used { get; set; }
        public Dictionary<string, List<string>> duplicates { get; set; }
        public PythonStatistics statistics { get; set; }
        public string error { get; set; }
    }

    internal class PythonStatistics
    {
        public int total_files { get; set; }
        public int valid_files { get; set; }
        public int duplicate_groups { get; set; }
        public int total_duplicates { get; set; }
    }
}
