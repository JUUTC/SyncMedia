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
        /// Find Python executable - checks bundled Python first, then system Python
        /// </summary>
        private string FindPythonExecutable()
        {
            // Priority 1: Check for bundled Python (MSIX package deployment)
            var bundledPython = TryGetBundledPython();
            if (bundledPython != null)
            {
                return bundledPython;
            }
            
            // Priority 2: Check system PATH for development/user-installed Python
            return FindSystemPython();
        }
        
        /// <summary>
        /// Try to get bundled Python from MSIX package
        /// </summary>
        private string? TryGetBundledPython()
        {
            try
            {
                // Method 1: Try Windows.ApplicationModel.Package (MSIX package)
                // This will only work when running as packaged app
                // Use reflection to avoid compile-time dependency on Windows SDK
                var packageType = Type.GetType("Windows.ApplicationModel.Package, Windows.Foundation.UniversalApiContract");
                if (packageType != null)
                {
                    var currentProperty = packageType.GetProperty("Current");
                    if (currentProperty != null)
                    {
                        var package = currentProperty.GetValue(null);
                        if (package != null)
                        {
                            var installedLocationProperty = package.GetType().GetProperty("InstalledLocation");
                            if (installedLocationProperty != null)
                            {
                                var installedLocation = installedLocationProperty.GetValue(package);
                                if (installedLocation != null)
                                {
                                    var pathProperty = installedLocation.GetType().GetProperty("Path");
                                    if (pathProperty != null)
                                    {
                                        var packagePath = pathProperty.GetValue(installedLocation) as string;
                                        if (packagePath != null)
                                        {
                                            var bundledPython = Path.Combine(packagePath, "Python", "python.exe");
                                            if (File.Exists(bundledPython))
                                            {
                                                return bundledPython;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // Not running as MSIX package or Windows.ApplicationModel not available
            }
            
            try
            {
                // Method 2: Check relative to assembly location
                // For development or non-packaged deployment
                var assemblyPath = Path.GetDirectoryName(typeof(AdvancedDuplicateDetectionService).Assembly.Location);
                var bundledPython = Path.Combine(assemblyPath, "Python", "python.exe");
                
                if (File.Exists(bundledPython))
                {
                    return bundledPython;
                }
                
                // Also check one level up (for different build configurations)
                var parentPath = Directory.GetParent(assemblyPath)?.FullName;
                if (parentPath != null)
                {
                    bundledPython = Path.Combine(parentPath, "Python", "python.exe");
                    if (File.Exists(bundledPython))
                    {
                        return bundledPython;
                    }
                }
            }
            catch
            {
                // Path issues, continue to system Python
            }
            
            return null; // No bundled Python found
        }
        
        /// <summary>
        /// Find Python in system PATH (for development or user-installed Python)
        /// </summary>
        private string FindSystemPython()
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
            
            // Default to "python" (will fail gracefully if not found)
            return "python";
        }

        /// <summary>
        /// Check if Python and required dependencies are installed
        /// </summary>
        public async Task<PythonEnvironmentStatus> CheckEnvironmentAsync()
        {
            // Check if Python script exists
            if (!File.Exists(_pythonScriptPath))
            {
                return new PythonEnvironmentStatus
                {
                    IsAvailable = false,
                    UnavailabilityReason = PythonUnavailabilityReason.ScriptNotFound,
                    Message = "Python integration script not found",
                    ErrorDetails = $"Script path: {_pythonScriptPath}",
                    SetupInstructions = "The Python script is missing. This may occur if the application was not installed correctly. Please reinstall the application.",
                    ShouldFallbackToMD5 = true
                };
            }
            
            try
            {
                // Get Python version first
                var pythonVersion = await GetPythonVersionAsync();
                
                if (pythonVersion == "Unknown" || pythonVersion.Contains("not found", StringComparison.OrdinalIgnoreCase))
                {
                    return new PythonEnvironmentStatus
                    {
                        IsAvailable = false,
                        UnavailabilityReason = PythonUnavailabilityReason.PythonNotFound,
                        Message = "Python not found on this system",
                        ErrorDetails = $"Tried executables: python, python3, py",
                        SetupInstructions = GetPythonInstallationInstructions(),
                        ShouldFallbackToMD5 = true
                    };
                }
                
                // Try to run a test command to check packages
                var testInput = new
                {
                    images = new string[] { },
                    method = "phash",
                    threshold = 0.9
                };
                
                var inputJson = JsonSerializer.Serialize(testInput);
                var result = await ExecutePythonScriptAsync(inputJson, CancellationToken.None);
                
                // Parse result to check for GPU support
                bool hasGpu = result.Contains("gpu_used", StringComparison.OrdinalIgnoreCase);
                
                return new PythonEnvironmentStatus
                {
                    IsAvailable = true,
                    PythonVersion = pythonVersion.Trim(),
                    HasGpuSupport = hasGpu,
                    UnavailabilityReason = PythonUnavailabilityReason.None,
                    Message = "Python environment is ready",
                    SetupInstructions = hasGpu ? "GPU acceleration available" : "GPU not available, will use CPU processing",
                    ShouldFallbackToMD5 = false
                };
            }
            catch (Exception ex)
            {
                // Determine specific reason for failure
                var reason = PythonUnavailabilityReason.UnknownError;
                var message = "Python environment not available";
                var instructions = "Unknown error occurred";
                
                if (ex.Message.Contains("imagededup", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("torch", StringComparison.OrdinalIgnoreCase) ||
                    ex.Message.Contains("ModuleNotFoundError", StringComparison.OrdinalIgnoreCase))
                {
                    reason = PythonUnavailabilityReason.PackagesMissing;
                    message = "Required Python packages not installed";
                    instructions = GetPackageInstallationInstructions();
                }
                else if (ex.Message.Contains("permission", StringComparison.OrdinalIgnoreCase) ||
                         ex.Message.Contains("access denied", StringComparison.OrdinalIgnoreCase))
                {
                    reason = PythonUnavailabilityReason.PermissionDenied;
                    message = "Permission denied when executing Python";
                    instructions = "Try running the application as administrator or check Python installation permissions.";
                }
                
                return new PythonEnvironmentStatus
                {
                    IsAvailable = false,
                    PythonVersion = await GetPythonVersionAsync(),
                    UnavailabilityReason = reason,
                    Message = message,
                    ErrorDetails = ex.ToString(),
                    SetupInstructions = instructions,
                    ShouldFallbackToMD5 = true
                };
            }
        }
        
        /// <summary>
        /// Get Python installation instructions
        /// </summary>
        private string GetPythonInstallationInstructions()
        {
            return @"Python 3.8 or higher is required for AI-powered duplicate detection.

Installation Steps:
1. Download Python from: https://www.python.org/downloads/
2. Run installer and check 'Add Python to PATH'
3. Open Command Prompt and run: pip install -r requirements.txt
4. Restart SyncMedia

Alternative: Continue using standard MD5 duplicate detection (exact matches only).";
        }
        
        /// <summary>
        /// Get package installation instructions
        /// </summary>
        private string GetPackageInstallationInstructions()
        {
            return @"Required Python packages are not installed.

Installation Steps:
1. Open Command Prompt
2. Navigate to: SyncMedia installation directory\Python\
3. Run: pip install -r requirements.txt
4. Wait for installation to complete
5. Restart SyncMedia

Required packages:
- imagededup>=0.3.2
- torch>=2.0.0
- torchvision>=0.15.0
- Pillow>=10.0.0
- numpy>=1.24.0

Alternative: Continue using standard MD5 duplicate detection.";
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
        
        /// <summary>
        /// Specific reason for unavailability
        /// </summary>
        public PythonUnavailabilityReason UnavailabilityReason { get; set; }
        
        /// <summary>
        /// List of missing packages (if Python is found but packages are missing)
        /// </summary>
        public List<string> MissingPackages { get; set; }
        
        /// <summary>
        /// User-friendly setup instructions
        /// </summary>
        public string SetupInstructions { get; set; }
        
        /// <summary>
        /// Whether fallback to MD5 is recommended
        /// </summary>
        public bool ShouldFallbackToMD5 { get; set; }
    }
    
    /// <summary>
    /// Reason why Python environment is unavailable
    /// </summary>
    public enum PythonUnavailabilityReason
    {
        None,                    // Python is available
        PythonNotFound,         // Python executable not found
        PythonVersionTooOld,    // Python version < 3.8
        PackagesMissing,        // Required packages not installed
        ScriptNotFound,         // find_duplicates.py script missing
        PermissionDenied,       // Cannot execute Python
        UnknownError            // Other error
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
