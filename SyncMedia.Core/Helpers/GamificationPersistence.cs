using System;
using System.Linq;
using SyncMedia.Core.Models;

namespace SyncMedia.Core.Helpers
{
    /// <summary>
    /// Helper class for persisting gamification data to XML settings
    /// </summary>
    public static class GamificationPersistence
    {
        /// <summary>
        /// Load gamification data from XML settings
        /// </summary>
        public static GamificationData LoadData()
        {
            var data = new GamificationData();
            
            try
            {
                data.TotalPoints = int.Parse(XmlData.ReadSetting("TotalPoints") ?? "0");
                data.TotalFilesLifetime = int.Parse(XmlData.ReadSetting("TotalFilesLifetime") ?? "0");
                data.TotalDuplicatesLifetime = int.Parse(XmlData.ReadSetting("TotalDuplicatesLifetime") ?? "0");
                data.TotalBytesLifetime = long.Parse(XmlData.ReadSetting("TotalBytesLifetime") ?? "0");
                data.TotalSyncsCompleted = int.Parse(XmlData.ReadSetting("TotalSyncsCompleted") ?? "0");
                data.FailedSyncsCount = int.Parse(XmlData.ReadSetting("FailedSyncsCount") ?? "0");
                data.TotalSpaceSaved = long.Parse(XmlData.ReadSetting("TotalSpaceSaved") ?? "0");
                
                string achievementsStr = XmlData.ReadSetting("Achievements");
                if (!string.IsNullOrEmpty(achievementsStr))
                {
                    data.Achievements = achievementsStr.Split('|').ToList();
                }
            }
            catch
            {
                // First time user - initialize with defaults
                data.TotalPoints = 0;
                data.TotalFilesLifetime = 0;
                data.TotalDuplicatesLifetime = 0;
                data.TotalBytesLifetime = 0;
                data.TotalSyncsCompleted = 0;
                data.FailedSyncsCount = 0;
                data.TotalSpaceSaved = 0;
                data.Achievements.Clear();
            }
            
            return data;
        }
        
        /// <summary>
        /// Save gamification data to XML settings
        /// </summary>
        public static void SaveData(GamificationData data)
        {
            XmlData.AddUpdateAppSettings("TotalPoints", data.TotalPoints.ToString());
            XmlData.AddUpdateAppSettings("TotalFilesLifetime", data.TotalFilesLifetime.ToString());
            XmlData.AddUpdateAppSettings("TotalDuplicatesLifetime", data.TotalDuplicatesLifetime.ToString());
            XmlData.AddUpdateAppSettings("TotalBytesLifetime", data.TotalBytesLifetime.ToString());
            XmlData.AddUpdateAppSettings("TotalSyncsCompleted", data.TotalSyncsCompleted.ToString());
            XmlData.AddUpdateAppSettings("FailedSyncsCount", data.FailedSyncsCount.ToString());
            XmlData.AddUpdateAppSettings("TotalSpaceSaved", data.TotalSpaceSaved.ToString());
            XmlData.AddUpdateAppSettings("Achievements", string.Join("|", data.Achievements));
        }
    }
}
