using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SyncMedia.Core.Helpers
{
    /// <summary>
    /// Provides secure path validation to prevent path traversal and other security issues
    /// </summary>
    public static class PathValidator
    {
        private static readonly char[] InvalidPathChars = Path.GetInvalidPathChars();
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

        // Common path traversal patterns
        private static readonly string[] DangerousPatterns = new[]
        {
            "..",
            "~",
            "%",
            "$"
        };

        /// <summary>
        /// Validates if a path is safe and well-formed
        /// </summary>
        /// <param name="path">The path to validate</param>
        /// <param name="mustExist">If true, the path must exist on the filesystem</param>
        /// <param name="errorMessage">Output parameter containing the error message if validation fails</param>
        /// <returns>True if the path is valid, false otherwise</returns>
        public static bool IsValidPath(string path, bool mustExist, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(path))
            {
                errorMessage = "Path cannot be null or empty.";
                return false;
            }

            // Check for invalid characters
            if (path.IndexOfAny(InvalidPathChars) >= 0)
            {
                errorMessage = "Path contains invalid characters.";
                return false;
            }

            // Check for path traversal attempts
            if (ContainsPathTraversal(path))
            {
                errorMessage = "Path contains potentially dangerous patterns (path traversal).";
                return false;
            }

            // Validate it's a rooted path (absolute path)
            try
            {
                if (!Path.IsPathRooted(path))
                {
                    errorMessage = "Path must be an absolute path.";
                    return false;
                }
            }
            catch (ArgumentException)
            {
                errorMessage = "Path format is invalid.";
                return false;
            }

            // Check if path exists if required
            if (mustExist)
            {
                if (!Directory.Exists(path) && !File.Exists(path))
                {
                    errorMessage = $"Path does not exist: {path}";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Validates if a directory path is safe and well-formed
        /// </summary>
        /// <param name="path">The directory path to validate</param>
        /// <param name="mustExist">If true, the directory must exist</param>
        /// <param name="errorMessage">Output parameter containing the error message if validation fails</param>
        /// <returns>True if the path is valid, false otherwise</returns>
        public static bool IsValidDirectoryPath(string path, bool mustExist, out string errorMessage)
        {
            if (!IsValidPath(path, mustExist: false, out errorMessage))
            {
                return false;
            }

            if (mustExist && !Directory.Exists(path))
            {
                errorMessage = $"Directory does not exist: {path}";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates if a file name is safe (without path components)
        /// </summary>
        /// <param name="fileName">The file name to validate</param>
        /// <param name="errorMessage">Output parameter containing the error message if validation fails</param>
        /// <returns>True if the file name is valid, false otherwise</returns>
        public static bool IsValidFileName(string fileName, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                errorMessage = "File name cannot be null or empty.";
                return false;
            }

            // Check for invalid file name characters
            if (fileName.IndexOfAny(InvalidFileNameChars) >= 0)
            {
                errorMessage = "File name contains invalid characters.";
                return false;
            }

            // Check if file name contains path separators
            if (fileName.Contains(Path.DirectorySeparatorChar) || fileName.Contains(Path.AltDirectorySeparatorChar))
            {
                errorMessage = "File name cannot contain path separators.";
                return false;
            }

            // Check for dangerous patterns
            if (ContainsPathTraversal(fileName))
            {
                errorMessage = "File name contains potentially dangerous patterns.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if a path contains path traversal attempts
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>True if path traversal patterns are detected, false otherwise</returns>
        private static bool ContainsPathTraversal(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            // Normalize the path for checking
            var normalizedPath = path.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);

            // Check for explicit ".." patterns
            if (normalizedPath.Contains($"{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}") ||
                normalizedPath.StartsWith($"..{Path.DirectorySeparatorChar}") ||
                normalizedPath.EndsWith($"{Path.DirectorySeparatorChar}..") ||
                normalizedPath == "..")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Sanitizes a file name by removing invalid characters
        /// </summary>
        /// <param name="fileName">The file name to sanitize</param>
        /// <param name="replacement">The character to replace invalid characters with (default: underscore)</param>
        /// <returns>A sanitized file name</returns>
        public static string SanitizeFileName(string fileName, char replacement = '_')
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }

            var sanitized = fileName;
            foreach (var c in InvalidFileNameChars)
            {
                sanitized = sanitized.Replace(c, replacement);
            }

            return sanitized;
        }

        /// <summary>
        /// Validates and normalizes a path
        /// </summary>
        /// <param name="path">The path to validate and normalize</param>
        /// <param name="normalizedPath">The normalized path if validation succeeds</param>
        /// <param name="errorMessage">Error message if validation fails</param>
        /// <returns>True if the path is valid and could be normalized, false otherwise</returns>
        public static bool TryNormalizePath(string path, out string normalizedPath, out string errorMessage)
        {
            normalizedPath = string.Empty;
            errorMessage = string.Empty;

            if (!IsValidPath(path, mustExist: false, out errorMessage))
            {
                return false;
            }

            try
            {
                normalizedPath = Path.GetFullPath(path);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = $"Failed to normalize path: {ex.Message}";
                return false;
            }
        }
    }
}
