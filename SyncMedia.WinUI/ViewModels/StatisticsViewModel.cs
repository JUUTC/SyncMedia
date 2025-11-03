using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncMedia.Core;
using SyncMedia.Core.Services;
using System;
using System.Collections.ObjectModel;

namespace SyncMedia.WinUI.ViewModels
{
    public partial class StatisticsViewModel : ObservableObject
    {
        [ObservableProperty]
        private int totalFilesSynced;

        [ObservableProperty]
        private string totalSpaceSaved = "0 MB";

        [ObservableProperty]
        private int totalSyncsCompleted;

        [ObservableProperty]
        private string successRate = "100%";

        [ObservableProperty]
        private int totalPoints;

        [ObservableProperty]
        private int achievementsUnlocked;

        [ObservableProperty]
        private int totalAchievements = 15; // Total possible achievements

        public ObservableCollection<SyncHistoryItem> RecentSyncs { get; } = new();

        public void LoadStatistics()
        {
            // Load from GamificationService
            var gamificationData = GamificationService.Instance.GetGamificationData();
            
            TotalFilesSynced = gamificationData.TotalFilesSynced;
            TotalSyncsCompleted = gamificationData.TotalSyncsCompleted;
            TotalPoints = gamificationData.TotalPoints;
            AchievementsUnlocked = gamificationData.AchievementsUnlocked;

            // Calculate space saved
            long totalBytes = gamificationData.TotalSpaceSaved;
            TotalSpaceSaved = FormatBytes(totalBytes);

            // Calculate success rate
            if (gamificationData.TotalSyncsCompleted > 0)
            {
                int successCount = gamificationData.TotalSyncsCompleted - gamificationData.FailedSyncsCount;
                double rate = (double)successCount / gamificationData.TotalSyncsCompleted * 100;
                SuccessRate = $"{rate:F1}%";
            }

            // Load recent sync history (placeholder - will be replaced with actual data)
            RecentSyncs.Clear();
            RecentSyncs.Add(new SyncHistoryItem
            {
                SyncDate = "Today, 2:30 PM",
                FileCountText = "125 files synced",
                Duration = "1m 45s"
            });
            RecentSyncs.Add(new SyncHistoryItem
            {
                SyncDate = "Yesterday, 5:15 PM",
                FileCountText = "89 files synced",
                Duration = "58s"
            });
            RecentSyncs.Add(new SyncHistoryItem
            {
                SyncDate = "2 days ago, 9:00 AM",
                FileCountText = "234 files synced",
                Duration = "3m 12s"
            });
        }

        [RelayCommand]
        private void ViewAchievements()
        {
            // Navigate to Achievements page
            // This will be implemented when we have navigation service
        }

        private static string FormatBytes(long bytes)
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;
            const long TB = GB * 1024;

            if (bytes >= TB)
                return $"{bytes / (double)TB:F2} TB";
            if (bytes >= GB)
                return $"{bytes / (double)GB:F2} GB";
            if (bytes >= MB)
                return $"{bytes / (double)MB:F2} MB";
            if (bytes >= KB)
                return $"{bytes / (double)KB:F2} KB";

            return $"{bytes} bytes";
        }
    }

    public class SyncHistoryItem
    {
        public string SyncDate { get; set; } = string.Empty;
        public string FileCountText { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
    }
}
