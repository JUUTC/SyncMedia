using Xunit;
using SyncMedia.Core.Helpers;
using SyncMedia.Core.Models;
using System;
using System.IO;

namespace SyncMedia.Tests.Helpers
{
    public class GamificationPersistenceTests : IDisposable
    {
        private readonly string _testDataPath;
        
        public GamificationPersistenceTests()
        {
            _testDataPath = Path.Combine(Path.GetTempPath(), $"SyncMediaTest_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDataPath);
            Environment.SetEnvironmentVariable("LOCALAPPDATA", _testDataPath);
        }
        
        public void Dispose()
        {
            if (Directory.Exists(_testDataPath))
            {
                Directory.Delete(_testDataPath, true);
            }
        }
        
        [Fact]
        public void LoadData_WithNoFile_ReturnsNewData()
        {
            // Act
            var data = GamificationPersistence.LoadData();
            
            // Assert
            Assert.NotNull(data);
            Assert.Equal(0, data.TotalPoints);
        }
        
        [Fact]
        public void SaveData_ThenLoad_PreservesData()
        {
            // Arrange
            var originalData = new GamificationData
            {
                TotalPoints = 500,
                TotalFilesLifetime = 100
            };
            
            // Act
            GamificationPersistence.SaveData(originalData);
            var loadedData = GamificationPersistence.LoadData();
            
            // Assert
            Assert.Equal(500, loadedData.TotalPoints);
            Assert.Equal(100, loadedData.TotalFilesSynced);
        }
    }
}
