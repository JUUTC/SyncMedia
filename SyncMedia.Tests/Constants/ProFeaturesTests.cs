using Xunit;
using SyncMedia.Core.Constants;

namespace SyncMedia.Tests.Constants
{
    public class ProFeaturesTests
    {
        [Fact]
        public void ProFeatures_HasExpectedConstants()
        {
            // Assert
            Assert.Equal("ParallelProcessing", ProFeatures.ParallelProcessing);
            Assert.Equal("AdvancedDuplicateDetection", ProFeatures.AdvancedDuplicateDetection);
            Assert.Equal("GpuAcceleration", ProFeatures.GpuAcceleration);
            Assert.Equal("AdvancedAnalytics", ProFeatures.AdvancedAnalytics);
            Assert.Equal("PerformanceOptimizations", ProFeatures.PerformanceOptimizations);
            Assert.Equal("AdFree", ProFeatures.AdFree);
            Assert.Equal("CustomSyncProfiles", ProFeatures.CustomSyncProfiles);
            Assert.Equal("PrioritySupport", ProFeatures.PrioritySupport);
        }
    }
}
