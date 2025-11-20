using System;
using System.IO;
using Xunit;
using SyncMedia.Core.Helpers;

namespace SyncMedia.Tests.Helpers
{
    public class FileHelperTests
    {
        [Theory]
        [InlineData(".jpg", true)]
        [InlineData(".jpeg", true)]
        [InlineData(".png", true)]
        [InlineData(".gif", true)]
        [InlineData(".webp", true)]
        [InlineData(".heic", true)]
        [InlineData(".JPG", true)] // Case insensitive
        [InlineData(".mp4", false)]
        [InlineData(".txt", false)]
        [InlineData(".doc", false)]
        public void IsImageFile_ShouldReturnCorrectResult(string extension, bool expected)
        {
            // Act
            var result = FileHelper.IsImageFile(extension);
            
            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(".mp4", true)]
        [InlineData(".mov", true)]
        [InlineData(".avi", true)]
        [InlineData(".wmv", true)]
        [InlineData(".webm", true)]
        [InlineData(".mkv", true)]
        [InlineData(".MP4", true)] // Case insensitive
        [InlineData(".jpg", false)]
        [InlineData(".txt", false)]
        public void IsVideoFile_ShouldReturnCorrectResult(string extension, bool expected)
        {
            // Act
            var result = FileHelper.IsVideoFile(extension);
            
            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(".jpg", true)]
        [InlineData(".png", true)]
        [InlineData(".mp4", true)]
        [InlineData(".mov", true)]
        [InlineData(".webp", true)]
        [InlineData(".webm", true)]
        [InlineData(".txt", false)]
        [InlineData(".doc", false)]
        [InlineData(".pdf", false)]
        public void IsMediaFile_ShouldReturnCorrectResult(string extension, bool expected)
        {
            // Act
            var result = FileHelper.IsMediaFile(extension);
            
            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("IMG_2024-01-15_001.jpg", "img__")]
        [InlineData("Video-2023-12-25.mp4", "video")]
        [InlineData("Photo123.png", "photo")]
        public void CleanFileName_ShouldRemoveDatesAndExtensions(string filename, string expected)
        {
            // Act
            var result = FileHelper.CleanFileName(filename);
            
            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CleanFileName_ShouldConvertToLowerCase()
        {
            // Arrange
            var filename = "MyPhoto.JPG";
            
            // Act
            var result = FileHelper.CleanFileName(filename);
            
            // Assert
            Assert.DoesNotContain("M", result);
            Assert.DoesNotContain("P", result);
            Assert.DoesNotContain("J", result);
        }

        [Fact]
        public void CleanFileName_ShouldRemoveSpecialCharacters()
        {
            // Arrange
            var filename = "photo,test.file.jpg";
            
            // Act
            var result = FileHelper.CleanFileName(filename);
            
            // Assert
            Assert.DoesNotContain(",", result);
            Assert.DoesNotContain(".", result);
        }

        [Fact]
        public void RemoveFolderPath_ShouldRemoveDatePatterns()
        {
            // Arrange
            var filename = "2024/01/15/photo.jpg";
            
            // Act
            var result = FileHelper.RemoveFolderPath(filename);
            
            // Assert
            Assert.DoesNotContain("2024/01/15", result);
        }

        [Theory]
        [InlineData("2024-01-15", "2024-01-15")]
        [InlineData("2024-12-31", "2024-12-31")]
        [InlineData("2023-05-20", "2023-05-20")]
        public void FormatFileDateTime_ShouldFormatCorrectly(string dateString, string expected)
        {
            // Arrange
            var date = DateTime.Parse(dateString);
            
            // Act
            var result = FileHelper.FormatFileDateTime(date);
            
            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FormatFileDateTime_ShouldPadMonthAndDay()
        {
            // Arrange
            var date = new DateTime(2024, 1, 5);
            
            // Act
            var result = FileHelper.FormatFileDateTime(date);
            
            // Assert
            Assert.Equal("2024-01-05", result);
        }

        [Fact]
        public void ValidateFolderPath_WithExistingDirectory_ShouldReturnTrue()
        {
            // Arrange
            var tempPath = Path.GetTempPath();
            
            // Act
            var result = FileHelper.ValidateFolderPath(tempPath);
            
            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ValidateFolderPath_WithNonExistentDirectory_ShouldReturnFalse()
        {
            // Arrange
            var nonExistentPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            
            // Act
            var result = FileHelper.ValidateFolderPath(nonExistentPath);
            
            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("   ", false)]
        public void ValidateFolderPath_WithInvalidInput_ShouldReturnFalse(string path, bool expected)
        {
            // Act
            var result = FileHelper.ValidateFolderPath(path);
            
            // Assert
            Assert.Equal(expected, result);
        }
        
        [Fact]
        public void ValidateFolderPath_WithNullInput_ShouldReturnFalse()
        {
            // Act
            var result = FileHelper.ValidateFolderPath(null!);
            
            // Assert
            Assert.False(result);
        }

        [Fact]
        public void RemoveFolderStructure_WithBackslash_ShouldTrimLeadingSlashes()
        {
            // Arrange
            var fullPath = @"\\folder\\file.jpg";
            var nodate = @"\\test";
            
            // Act
            var result = FileHelper.RemoveFolderStructure(fullPath, nodate);
            
            // Assert
            Assert.False(result.StartsWith("\\"));
        }
    }
}
