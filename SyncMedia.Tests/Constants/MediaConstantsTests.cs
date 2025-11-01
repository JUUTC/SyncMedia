using System.Linq;
using Xunit;
using SyncMedia.Constants;

namespace SyncMedia.Tests.Constants
{
    public class MediaConstantsTests
    {
        [Theory]
        [InlineData(".jpg")]
        [InlineData(".jpeg")]
        [InlineData(".png")]
        [InlineData(".webp")]
        [InlineData(".heic")]
        [InlineData(".avif")]
        public void ImageExtensions_ShouldContainCommonFormats(string extension)
        {
            // Act & Assert
            Assert.Contains(extension, MediaConstants.ImageExtensions);
        }
        
        [Theory]
        [InlineData(".mp4")]
        [InlineData(".mov")]
        [InlineData(".avi")]
        [InlineData(".webm")]
        [InlineData(".mkv")]
        public void VideoExtensions_ShouldContainCommonFormats(string extension)
        {
            // Act & Assert
            Assert.Contains(extension, MediaConstants.VideoExtensions);
        }
        
        [Fact]
        public void AllMediaExtensions_ShouldContainBothImageAndVideo()
        {
            // Act
            var allCount = MediaConstants.AllMediaExtensions.Count;
            var imageCount = MediaConstants.ImageExtensions.Count;
            var videoCount = MediaConstants.VideoExtensions.Count;
            
            // Assert
            Assert.Equal(imageCount + videoCount, allCount);
        }
        
        [Fact]
        public void ImageExtensions_ShouldBeCaseInsensitive()
        {
            // Act & Assert
            Assert.Contains(".JPG", MediaConstants.ImageExtensions);
            Assert.Contains(".Jpg", MediaConstants.ImageExtensions);
            Assert.Contains(".jpg", MediaConstants.ImageExtensions);
        }
        
        [Fact]
        public void VideoExtensions_ShouldBeCaseInsensitive()
        {
            // Act & Assert
            Assert.Contains(".MP4", MediaConstants.VideoExtensions);
            Assert.Contains(".Mp4", MediaConstants.VideoExtensions);
            Assert.Contains(".mp4", MediaConstants.VideoExtensions);
        }
        
        [Fact]
        public void Constants_ShouldHaveExpectedValues()
        {
            // Assert
            Assert.Equal(10, MediaConstants.UI_UPDATE_BATCH_SIZE);
            Assert.Equal(10, MediaConstants.POINTS_PER_FILE);
            Assert.Equal(5, MediaConstants.POINTS_PER_DUPLICATE);
            Assert.Equal(1, MediaConstants.POINTS_PER_MB);
        }
        
        [Fact]
        public void SpeedBonuses_ShouldBeProperlyTiered()
        {
            // Assert
            Assert.True(MediaConstants.SPEED_QUICK_BONUS < MediaConstants.SPEED_DEMON_BONUS);
            Assert.True(MediaConstants.SPEED_DEMON_BONUS < MediaConstants.SPEED_SUPER_BONUS);
            Assert.True(MediaConstants.SPEED_SUPER_BONUS < MediaConstants.SPEED_LIGHTNING_BONUS);
        }
    }
}
