using System;
using System.IO;
using Xunit;
using SyncMedia.Core.Helpers;

namespace SyncMedia.Tests.Helpers
{
    public class PathValidatorTests
    {
        [Fact]
        public void IsValidPath_WithNull_ShouldReturnFalse()
        {
            // Act
            var result = PathValidator.IsValidPath(null!, false, out var errorMessage);
            
            // Assert
            Assert.False(result);
            Assert.NotEmpty(errorMessage);
        }

        [Fact]
        public void IsValidPath_WithEmptyString_ShouldReturnFalse()
        {
            // Act
            var result = PathValidator.IsValidPath(string.Empty, false, out var errorMessage);
            
            // Assert
            Assert.False(result);
            Assert.NotEmpty(errorMessage);
        }

        [Fact]
        public void IsValidPath_WithWhitespace_ShouldReturnFalse()
        {
            // Act
            var result = PathValidator.IsValidPath("   ", false, out var errorMessage);
            
            // Assert
            Assert.False(result);
            Assert.NotEmpty(errorMessage);
        }

        [Fact]
        public void IsValidPath_WithInvalidCharacters_ShouldReturnFalse()
        {
            // Arrange - use null character which is invalid on all platforms
            var invalidPath = Path.Combine(Path.GetTempPath(), "Test\0Invalid");
            
            // Act
            var result = PathValidator.IsValidPath(invalidPath, false, out var errorMessage);
            
            // Assert
            Assert.False(result);
            Assert.NotEmpty(errorMessage);
        }

        [Fact]
        public void IsValidPath_WithPathTraversal_ShouldReturnFalse()
        {
            // Arrange - use absolute path with traversal
            var traversalPath = Path.Combine(Path.GetTempPath(), "..", "..", "System32");
            
            // Act
            var result = PathValidator.IsValidPath(traversalPath, false, out var errorMessage);
            
            // Assert
            Assert.False(result);
            Assert.Contains("path traversal", errorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void IsValidPath_WithRelativePath_ShouldReturnFalse()
        {
            // Arrange
            var relativePath = "relative\\path";
            
            // Act
            var result = PathValidator.IsValidPath(relativePath, false, out var errorMessage);
            
            // Assert
            Assert.False(result);
            Assert.Contains("absolute", errorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void IsValidPath_WithValidAbsolutePath_ShouldReturnTrue()
        {
            // Arrange
            var validPath = Path.GetTempPath();
            
            // Act
            var result = PathValidator.IsValidPath(validPath, false, out var errorMessage);
            
            // Assert
            Assert.True(result);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValidPath_WithNonExistentPath_MustExist_ShouldReturnFalse()
        {
            // Arrange
            var nonExistentPath = Path.Combine(Path.GetTempPath(), "NonExistentPath" + Guid.NewGuid().ToString());
            
            // Act
            var result = PathValidator.IsValidPath(nonExistentPath, mustExist: true, out var errorMessage);
            
            // Assert
            Assert.False(result);
            Assert.Contains("does not exist", errorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void IsValidDirectoryPath_WithExistingDirectory_ShouldReturnTrue()
        {
            // Arrange
            var existingDir = Path.GetTempPath();
            
            // Act
            var result = PathValidator.IsValidDirectoryPath(existingDir, mustExist: true, out var errorMessage);
            
            // Assert
            Assert.True(result);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValidDirectoryPath_WithNonExistentDirectory_MustExist_ShouldReturnFalse()
        {
            // Arrange
            var nonExistentDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            
            // Act
            var result = PathValidator.IsValidDirectoryPath(nonExistentDir, mustExist: true, out var errorMessage);
            
            // Assert
            Assert.False(result);
            Assert.Contains("does not exist", errorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void IsValidFileName_WithNull_ShouldReturnFalse()
        {
            // Act
            var result = PathValidator.IsValidFileName(null!, out var errorMessage);
            
            // Assert
            Assert.False(result);
            Assert.NotEmpty(errorMessage);
        }

        [Fact]
        public void IsValidFileName_WithEmptyString_ShouldReturnFalse()
        {
            // Act
            var result = PathValidator.IsValidFileName(string.Empty, out var errorMessage);
            
            // Assert
            Assert.False(result);
            Assert.NotEmpty(errorMessage);
        }

        [Fact]
        public void IsValidFileName_WithInvalidCharacters_ShouldReturnFalse()
        {
            // Arrange - use null character which is always invalid
            var invalidFileName = "test\0file.txt";
            
            // Act
            var result = PathValidator.IsValidFileName(invalidFileName, out var errorMessage);
            
            // Assert
            Assert.False(result);
            Assert.Contains("invalid characters", errorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void IsValidFileName_WithPathSeparators_ShouldReturnFalse()
        {
            // Arrange
            var fileNameWithPath = $"folder{Path.DirectorySeparatorChar}file.txt";
            
            // Act
            var result = PathValidator.IsValidFileName(fileNameWithPath, out var errorMessage);
            
            // Assert
            Assert.False(result);
            // Path separator is caught as either invalid character or path separator error
            Assert.True(errorMessage.Contains("invalid characters", StringComparison.OrdinalIgnoreCase) ||
                       errorMessage.Contains("path separator", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void IsValidFileName_WithValidFileName_ShouldReturnTrue()
        {
            // Arrange
            var validFileName = "validfile.txt";
            
            // Act
            var result = PathValidator.IsValidFileName(validFileName, out var errorMessage);
            
            // Assert
            Assert.True(result);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValidFileName_WithDotDot_ShouldReturnFalse()
        {
            // Arrange
            var fileName = "..";
            
            // Act
            var result = PathValidator.IsValidFileName(fileName, out var errorMessage);
            
            // Assert
            Assert.False(result);
            Assert.Contains("dangerous", errorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void SanitizeFileName_WithInvalidCharacters_ShouldReplaceWithUnderscore()
        {
            // Arrange - Test with path separator which is always invalid in a file name
            var invalidChars = Path.GetInvalidFileNameChars();
            if (invalidChars.Length == 0)
            {
                // Skip test if no invalid chars (shouldn't happen)
                return;
            }
            
            var testChar = invalidChars[0];
            var fileName = $"test{testChar}file.txt";
            
            // Act
            var sanitized = PathValidator.SanitizeFileName(fileName);
            
            // Assert
            Assert.NotEqual(fileName, sanitized);
            Assert.Contains("_", sanitized);
            Assert.DoesNotContain(testChar, sanitized.ToCharArray());
        }

        [Fact]
        public void SanitizeFileName_WithCustomReplacement_ShouldUseCustomCharacter()
        {
            // Arrange - Test with path separator which is always invalid in a file name
            var invalidChars = Path.GetInvalidFileNameChars();
            if (invalidChars.Length == 0)
            {
                // Skip test if no invalid chars
                return;
            }
            
            var testChar = invalidChars[0];
            var fileName = $"test{testChar}file.txt";
            
            // Act
            var sanitized = PathValidator.SanitizeFileName(fileName, '-');
            
            // Assert
            Assert.NotEqual(fileName, sanitized);
            Assert.Contains("-", sanitized);
            Assert.DoesNotContain(testChar, sanitized.ToCharArray());
        }

        [Fact]
        public void SanitizeFileName_WithNullOrEmpty_ShouldReturnEmptyString()
        {
            // Act
            var sanitized1 = PathValidator.SanitizeFileName(null!);
            var sanitized2 = PathValidator.SanitizeFileName(string.Empty);
            
            // Assert
            Assert.Empty(sanitized1);
            Assert.Empty(sanitized2);
        }

        [Fact]
        public void TryNormalizePath_WithValidPath_ShouldReturnNormalizedPath()
        {
            // Arrange
            var tempPath = Path.GetTempPath();
            var testPath = Path.Combine(tempPath, "test");
            
            // Act
            var result = PathValidator.TryNormalizePath(testPath, out var normalizedPath, out var errorMessage);
            
            // Assert
            Assert.True(result);
            Assert.NotEmpty(normalizedPath);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void TryNormalizePath_WithInvalidPath_ShouldReturnFalse()
        {
            // Arrange - use null character which is always invalid
            var invalidPath = Path.Combine(Path.GetTempPath(), "Test\0Invalid");
            
            // Act
            var result = PathValidator.TryNormalizePath(invalidPath, out var normalizedPath, out var errorMessage);
            
            // Assert
            Assert.False(result);
            Assert.Empty(normalizedPath);
            Assert.NotEmpty(errorMessage);
        }

        [Fact]
        public void IsValidPath_WithVariousTraversalPatterns_ShouldReturnFalse()
        {
            // Arrange - Test various path traversal patterns
            var patterns = new[]
            {
                Path.Combine(Path.GetTempPath(), "..", "..", "System32"),
                Path.Combine(Path.GetTempPath(), "..", "Windows"),
                ".." + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Windows"
            };

            foreach (var path in patterns)
            {
                // Act
                var result = PathValidator.IsValidPath(path, false, out var errorMessage);
                
                // Assert
                Assert.False(result, $"Path should be invalid: {path}");
            }
        }

        [Theory]
        [InlineData("testfile.txt")]
        [InlineData("my-file_123.doc")]
        [InlineData("image.2024.jpg")]
        public void IsValidFileName_WithVariousValidNames_ShouldReturnTrue(string fileName)
        {
            // Act
            var result = PathValidator.IsValidFileName(fileName, out var errorMessage);
            
            // Assert
            Assert.True(result);
            Assert.Empty(errorMessage);
        }
    }
}
