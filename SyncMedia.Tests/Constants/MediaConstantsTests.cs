using System.Linq;
using Xunit;
using FluentAssertions;
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
            MediaConstants.ImageExtensions.Should().Contain(extension);
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
            MediaConstants.VideoExtensions.Should().Contain(extension);
        }
        
        [Fact]
        public void AllMediaExtensions_ShouldContainBothImageAndVideo()
        {
            // Act
            var allCount = MediaConstants.AllMediaExtensions.Count;
            var imageCount = MediaConstants.ImageExtensions.Count;
            var videoCount = MediaConstants.VideoExtensions.Count;
            
            // Assert
            allCount.Should().Be(imageCount + videoCount);
        }
        
        [Fact]
        public void ImageExtensions_ShouldBeCaseInsensitive()
        {
            // Act & Assert
            MediaConstants.ImageExtensions.Should().Contain(".JPG");
            MediaConstants.ImageExtensions.Should().Contain(".Jpg");
            MediaConstants.ImageExtensions.Should().Contain(".jpg");
        }
        
        [Fact]
        public void VideoExtensions_ShouldBeCaseInsensitive()
        {
            // Act & Assert
            MediaConstants.VideoExtensions.Should().Contain(".MP4");
            MediaConstants.VideoExtensions.Should().Contain(".Mp4");
            MediaConstants.VideoExtensions.Should().Contain(".mp4");
        }
        
        [Fact]
        public void Constants_ShouldHaveExpectedValues()
        {
            // Assert
            MediaConstants.UI_UPDATE_BATCH_SIZE.Should().Be(10);
            MediaConstants.POINTS_PER_FILE.Should().Be(10);
            MediaConstants.POINTS_PER_DUPLICATE.Should().Be(5);
            MediaConstants.POINTS_PER_MB.Should().Be(1);
        }
        
        [Fact]
        public void SpeedBonuses_ShouldBeProperlyTiered()
        {
            // Assert
            MediaConstants.SPEED_QUICK_BONUS.Should().BeLessThan(MediaConstants.SPEED_DEMON_BONUS);
            MediaConstants.SPEED_DEMON_BONUS.Should().BeLessThan(MediaConstants.SPEED_SUPER_BONUS);
            MediaConstants.SPEED_SUPER_BONUS.Should().BeLessThan(MediaConstants.SPEED_LIGHTNING_BONUS);
        }
    }
}
