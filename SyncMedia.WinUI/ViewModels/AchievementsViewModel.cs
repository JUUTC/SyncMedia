using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using SyncMedia.Core.Models;

namespace SyncMedia.WinUI.ViewModels;

public partial class AchievementsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<AchievementData> achievements = new();

    public AchievementsViewModel()
    {
        LoadAchievements();
    }

    private void LoadAchievements()
    {
        // Sample achievements - in production, these would be loaded from storage
        Achievements = new ObservableCollection<AchievementData>
        {
            new AchievementData
            {
                Id = "first_sync",
                Name = "First Sync",
                Description = "Complete your first file synchronization",
                IconGlyph = "\uE73E", // CheckMark
                IsUnlocked = true,
                UnlockedDate = System.DateTime.Now.AddDays(-7),
                Progress = 1,
                Target = 1,
                UnlockCriteria = "Complete 1 sync operation"
            },
            new AchievementData
            {
                Id = "speed_demon",
                Name = "Speed Demon",
                Description = "Sync 100 files in under 1 minute",
                IconGlyph = "\uE945", // Lightning
                IsUnlocked = false,
                Progress = 45,
                Target = 100,
                UnlockCriteria = "Sync 100 files in less than 60 seconds"
            },
            new AchievementData
            {
                Id = "storage_saver",
                Name = "Storage Saver",
                Description = "Free up 1GB of space with duplicate detection",
                IconGlyph = "\uE8B7", // Save
                IsUnlocked = false,
                Progress = 512,
                Target = 1024,
                UnlockCriteria = "Save 1GB (1024MB) by removing duplicates (Pro only)"
            },
            new AchievementData
            {
                Id = "perfectionist",
                Name = "Perfectionist",
                Description = "Complete 50 syncs with zero errors",
                IconGlyph = "\uE734", // Favorite
                IsUnlocked = false,
                Progress = 12,
                Target = 50,
                UnlockCriteria = "Complete 50 sync operations without any errors"
            },
            new AchievementData
            {
                Id = "streak_master",
                Name = "Streak Master",
                Description = "Use the app 7 days in a row",
                IconGlyph = "\uE82D", // Calendar
                IsUnlocked = false,
                Progress = 3,
                Target = 7,
                UnlockCriteria = "Use SyncMedia for 7 consecutive days"
            },
            new AchievementData
            {
                Id = "night_owl",
                Name = "Night Owl",
                Description = "Use the app after 10 PM",
                IconGlyph = "\uE708", // NightLight
                IsUnlocked = false,
                Progress = 0,
                Target = 1,
                UnlockCriteria = "Run a sync operation after 10:00 PM"
            },
            new AchievementData
            {
                Id = "early_bird",
                Name = "Early Bird",
                Description = "Use the app before 6 AM",
                IconGlyph = "\uE706", // Sunny
                IsUnlocked = false,
                Progress = 0,
                Target = 1,
                UnlockCriteria = "Run a sync operation before 6:00 AM"
            },
            new AchievementData
            {
                Id = "power_user",
                Name = "Power User",
                Description = "Sync 10,000 files total",
                IconGlyph = "\uE7C3", // GripperBarHorizontal (represents power/strength)
                IsUnlocked = false,
                Progress = 1247,
                Target = 10000,
                UnlockCriteria = "Process 10,000 files across all sync operations"
            }
        };
    }
}
