using Xunit;
using SyncMedia.Core.Constants;

namespace SyncMedia.Tests.Constants
{
    public class MediaConstantsTests
    {
        [Fact]
        public void MediaConstants_HasExpectedValues()
        {
            // Points per action
            Assert.Equal(10, MediaConstants.POINTS_PER_FILE);
            Assert.Equal(25, MediaConstants.POINTS_PER_DUPLICATE);
            Assert.Equal(5, MediaConstants.POINTS_PER_MB);
            
            // Speed thresholds
            Assert.True(MediaConstants.SPEED_QUICK_THRESHOLD > 0);
            Assert.True(MediaConstants.SPEED_DEMON_THRESHOLD > MediaConstants.SPEED_QUICK_THRESHOLD);
            Assert.True(MediaConstants.SPEED_SUPER_THRESHOLD > MediaConstants.SPEED_DEMON_THRESHOLD);
            Assert.True(MediaConstants.SPEED_LIGHTNING_THRESHOLD > MediaConstants.SPEED_SUPER_THRESHOLD);
        }
    }
}
