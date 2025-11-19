using System;
using System.Linq;
using Xunit;
using SyncMedia.Core.Constants;

namespace SyncMedia.Tests.Constants
{
    public class MediaConstantsTests
    {
        [Theory]
        [InlineData(".jpg")]
        [InlineData(".jpeg")]
        [InlineData(".png")]
        [InlineData(".gif")]
        [InlineData(".webp")]
        [InlineData(".heic")]
        [InlineData(".avif")]
        public void ImageExtensions_ShouldContainCommonFormats(string extension)
        {
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
            Assert.Contains(extension, MediaConstants.VideoExtensions);
        }

        [Fact]
        public void ImageExtensions_ShouldBeCaseInsensitive()
        {
            Assert.Contains(".JPG", MediaConstants.ImageExtensions);
            Assert.Contains(".jpg", MediaConstants.ImageExtensions);
            Assert.Contains(".Jpg", MediaConstants.ImageExtensions);
        }

        [Fact]
        public void VideoExtensions_ShouldBeCaseInsensitive()
        {
            Assert.Contains(".MP4", MediaConstants.VideoExtensions);
            Assert.Contains(".mp4", MediaConstants.VideoExtensions);
            Assert.Contains(".Mp4", MediaConstants.VideoExtensions);
        }

        [Fact]
        public void AllMediaExtensions_ShouldContainBothImageAndVideo()
        {
            Assert.Contains(".jpg", MediaConstants.AllMediaExtensions);
            Assert.Contains(".png", MediaConstants.AllMediaExtensions);
            Assert.Contains(".mp4", MediaConstants.AllMediaExtensions);
            Assert.Contains(".mov", MediaConstants.AllMediaExtensions);
        }

        [Fact]
        public void UIUpdateBatchSize_ShouldHaveReasonableValue()
        {
            Assert.Equal(10, MediaConstants.UI_UPDATE_BATCH_SIZE);
            Assert.True(MediaConstants.UI_UPDATE_BATCH_SIZE > 0);
        }

        [Fact]
        public void PointsSystemConstants_ShouldHavePositiveValues()
        {
            Assert.True(MediaConstants.POINTS_PER_FILE > 0);
            Assert.True(MediaConstants.POINTS_PER_DUPLICATE > 0);
            Assert.True(MediaConstants.POINTS_PER_MB > 0);
        }

        [Fact]
        public void SpeedBonusThresholds_ShouldBeInAscendingOrder()
        {
            Assert.True(MediaConstants.SPEED_QUICK_THRESHOLD < MediaConstants.SPEED_DEMON_THRESHOLD);
            Assert.True(MediaConstants.SPEED_DEMON_THRESHOLD < MediaConstants.SPEED_SUPER_THRESHOLD);
            Assert.True(MediaConstants.SPEED_SUPER_THRESHOLD < MediaConstants.SPEED_LIGHTNING_THRESHOLD);
        }

        [Fact]
        public void SpeedBonuses_ShouldIncreaseWithThreshold()
        {
            Assert.True(MediaConstants.SPEED_QUICK_BONUS < MediaConstants.SPEED_DEMON_BONUS);
            Assert.True(MediaConstants.SPEED_DEMON_BONUS < MediaConstants.SPEED_SUPER_BONUS);
            Assert.True(MediaConstants.SPEED_SUPER_BONUS < MediaConstants.SPEED_LIGHTNING_BONUS);
        }

        [Fact]
        public void ImageExtensions_ShouldContainModernFormats()
        {
            Assert.Contains(".webp", MediaConstants.ImageExtensions);
            Assert.Contains(".heic", MediaConstants.ImageExtensions);
            Assert.Contains(".avif", MediaConstants.ImageExtensions);
            Assert.Contains(".jxl", MediaConstants.ImageExtensions);
        }

        [Fact]
        public void VideoExtensions_ShouldContainModernFormats()
        {
            Assert.Contains(".webm", MediaConstants.VideoExtensions);
            Assert.Contains(".mkv", MediaConstants.VideoExtensions);
        }

        [Fact]
        public void AllMediaExtensions_Count_ShouldMatchImagePlusVideo()
        {
            var totalCount = MediaConstants.ImageExtensions.Count + MediaConstants.VideoExtensions.Count;
            Assert.Equal(totalCount, MediaConstants.AllMediaExtensions.Count);
        }

        [Fact]
        public void ImageExtensions_ShouldNotBeEmpty()
        {
            Assert.NotEmpty(MediaConstants.ImageExtensions);
            Assert.True(MediaConstants.ImageExtensions.Count >= 10);
        }

        [Fact]
        public void VideoExtensions_ShouldNotBeEmpty()
        {
            Assert.NotEmpty(MediaConstants.VideoExtensions);
            Assert.True(MediaConstants.VideoExtensions.Count >= 10);
        }

        [Theory]
        [InlineData(".txt")]
        [InlineData(".doc")]
        [InlineData(".pdf")]
        public void ImageExtensions_ShouldNotContainNonImageFormats(string extension)
        {
            Assert.DoesNotContain(extension, MediaConstants.ImageExtensions);
        }

        [Theory]
        [InlineData(".txt")]
        [InlineData(".jpg")]
        [InlineData(".pdf")]
        public void VideoExtensions_ShouldNotContainNonVideoFormats(string extension)
        {
            Assert.DoesNotContain(extension, MediaConstants.VideoExtensions);
        }
    }
}
