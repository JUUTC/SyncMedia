using System.Collections.Generic;

namespace SyncMedia.Core.Models
{
    /// <summary>
    /// Gamification data including points, achievements, and lifetime statistics
    /// </summary>
    public class GamificationData
    {
        public int TotalPoints { get; set; }
        public int SessionPoints { get; set; }
        public int TotalFilesLifetime { get; set; }
        public int TotalDuplicatesLifetime { get; set; }
        public long TotalBytesLifetime { get; set; }
        public List<string> Achievements { get; set; }
        
        // Additional statistics for UI display
        public int TotalFilesSynced => TotalFilesLifetime;
        public int TotalSyncsCompleted { get; set; }
        public int FailedSyncsCount { get; set; }
        public long TotalSpaceSaved { get; set; }
        public int AchievementsUnlocked => Achievements.Count;
        
        public GamificationData()
        {
            Achievements = new List<string>();
        }
        
        public void ResetSessionPoints()
        {
            SessionPoints = 0;
        }
        
        public void AddSessionPoints(int points)
        {
            SessionPoints += points;
            TotalPoints += points;
        }
        
        public void UpdateLifetimeStats(SyncStatistics stats)
        {
            TotalFilesLifetime += stats.TotalFilesProcessed;
            TotalDuplicatesLifetime += stats.DuplicatesFound;
            TotalBytesLifetime += stats.TotalBytesProcessed;
        }
        
        public bool HasAchievement(string achievementId)
        {
            return Achievements.Contains(achievementId);
        }
        
        public void AddAchievement(string achievementId)
        {
            if (!HasAchievement(achievementId))
            {
                Achievements.Add(achievementId);
            }
        }
    }
}
