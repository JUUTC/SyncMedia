using System;
using Xunit;
using SyncMedia.Helpers;

namespace SyncMedia.Tests.Helpers
{
    public class FileHelperTests
    {
        [Theory]
        [InlineData(".jpg", true)]
        [InlineData(".jpeg", true)]
        [InlineData(".png", true)]
        [InlineData(".webp", true)]
        [InlineData(".mp4", false)]
        [InlineData(".mov", false)]
        public void IsImageFile_ShouldReturnCorrectValue(string extension, bool expected)
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
        [InlineData(".webm", true)]
        [InlineData(".jpg", false)]
        [InlineData(".png", false)]
        public void IsVideoFile_ShouldReturnCorrectValue(string extension, bool expected)
        {
            // Act
            var result = FileHelper.IsVideoFile(extension);
            
            // Assert
            Assert.Equal(expected, result);
        }
        
        [Theory]
        [InlineData(".jpg", true)]
        [InlineData(".mp4", true)]
        [InlineData(".txt", false)]
        [InlineData(".doc", false)]
        public void IsMediaFile_ShouldReturnCorrectValue(string extension, bool expected)
        {
            // Act
            var result = FileHelper.IsMediaFile(extension);
            
            // Assert
            Assert.Equal(expected, result);
        }
        
        [Fact]
        public void CleanFileName_ShouldRemoveNumbers()
        {
            // Arrange
            var filename = "photo2024-01-15.jpg";
            
            // Act
            var result = FileHelper.CleanFileName(filename);
            
            // Assert
            Assert.DoesNotContain("2024", result);
            Assert.DoesNotContain("01", result);
            Assert.DoesNotContain("15", result);
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
        public void CleanFileName_ShouldRemoveExtensions()
        {
            // Arrange
            var filename = "photo.jpg";
            
            // Act
            var result = FileHelper.CleanFileName(filename);
            
            // Assert
            Assert.DoesNotContain(".jpg", result);
        }
        
        [Fact]
        public void RemoveFolderPath_ShouldRemoveDatePattern()
        {
            // Arrange
            var filename = "2024/01/15/photo.jpg";
            
            // Act
            var result = FileHelper.RemoveFolderPath(filename);
            
            // Assert
            Assert.DoesNotContain("2024/01/15", result);
        }
        
        [Fact]
        public void FormatFileDateTime_ShouldFormatCorrectly()
        {
            // Arrange
            var date = new DateTime(2024, 3, 7);
            
            // Act
            var result = FileHelper.FormatFileDateTime(date);
            
            // Assert
            Assert.Equal("2024-03-07", result);
        }
        
        [Fact]
        public void ValidateFolderPath_WithNull_ShouldReturnFalse()
        {
            // Act
            var result = FileHelper.ValidateFolderPath(null);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void ValidateFolderPath_WithEmptyString_ShouldReturnFalse()
        {
            // Act
            var result = FileHelper.ValidateFolderPath(string.Empty);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void ValidateFolderPath_WithWhitespace_ShouldReturnFalse()
        {
            // Act
            var result = FileHelper.ValidateFolderPath("   ");
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void ValidateFolderPath_WithNonExistingPath_ShouldReturnFalse()
        {
            // Arrange
            var path = "/non/existing/path/12345";
            
            // Act
            var result = FileHelper.ValidateFolderPath(path);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void ValidateFolderPath_WithExistingPath_ShouldReturnTrue()
        {
            // Arrange
            var path = "/tmp";
            
            // Act
            var result = FileHelper.ValidateFolderPath(path);
            
            // Assert
            Assert.True(result);
        }
    }
}
