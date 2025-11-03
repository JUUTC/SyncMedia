using Xunit;
using SyncMedia.Core.Helpers;
using System.IO;

namespace SyncMedia.Tests.Helpers
{
    public class FileHelperTests
    {
        [Fact]
        public void IsImageFile_WithImageExtension_ReturnsTrue()
        {
            // Arrange
            var imagePath = "test.jpg";
            
            // Act
            var result = FileHelper.IsImageFile(imagePath);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void IsImageFile_WithVideoExtension_ReturnsFalse()
        {
            // Arrange
            var videoPath = "test.mp4";
            
            // Act
            var result = FileHelper.IsImageFile(videoPath);
            
            // Assert
            Assert.False(result);
        }
        
        [Fact]
        public void IsVideoFile_WithVideoExtension_ReturnsTrue()
        {
            // Arrange
            var videoPath = "test.mp4";
            
            // Act
            var result = FileHelper.IsVideoFile(videoPath);
            
            // Assert
            Assert.True(result);
        }
        
        [Fact]
        public void IsVideoFile_WithImageExtension_ReturnsFalse()
        {
            // Arrange
            var imagePath = "test.png";
            
            // Act
            var result = FileHelper.IsVideoFile(imagePath);
            
            // Assert
            Assert.False(result);
        }
        
        [Theory]
        [InlineData(".jpg")]
        [InlineData(".jpeg")]
        [InlineData(".png")]
        [InlineData(".gif")]
        public void IsImageFile_SupportsCommonFormats(string extension)
        {
            // Arrange
            var path = $"test{extension}";
            
            // Act
            var result = FileHelper.IsImageFile(path);
            
            // Assert
            Assert.True(result);
        }
        
        [Theory]
        [InlineData(".mp4")]
        [InlineData(".avi")]
        [InlineData(".mkv")]
        [InlineData(".mov")]
        public void IsVideoFile_SupportsCommonFormats(string extension)
        {
            // Arrange
            var path = $"test{extension}";
            
            // Act
            var result = FileHelper.IsVideoFile(path);
            
            // Assert
            Assert.True(result);
        }
    }
}
