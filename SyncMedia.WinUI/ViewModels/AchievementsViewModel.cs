using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using SyncMedia.Core.Models;
using SyncMedia.Core.Services;
using System.Linq;

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
        // Load achievements from GamificationService
        var gamificationData = GamificationService.Instance.GetGamificationData();
        var unlockedAchievements = gamificationData.Achievements;
        
        // Define all possible achievements (these should match the ones in GamificationService)
        var allAchievements = new[]
        {
            new AchievementData
            {
                Id = "FirstTen",
                Name = "First Ten",
                Description = "Getting started! Sync your first 10 files",
                IconGlyph = "\uE73E", // CheckMark
                IsUnlocked = unlockedAchievements.Contains("FirstTen"),
                UnlockedDate = System.DateTime.Now.AddDays(-7),
                Progress = gamificationData.TotalFilesLifetime >= 10 ? 10 : gamificationData.TotalFilesLifetime,
                Target = 10,
                UnlockCriteria = "Sync 10 files total"
            },
            new AchievementData
            {
                Id = "Century",
                Name = "Century Club",
                Description = "Join the elite! Sync 100 files",
                IconGlyph = "\uE734", // Favorite
                IsUnlocked = unlockedAchievements.Contains("Century"),
                Progress = gamificationData.TotalFilesLifetime >= 100 ? 100 : gamificationData.TotalFilesLifetime,
                Target = 100,
                UnlockCriteria = "Sync 100 files total"
            },
            new AchievementData
            {
                Id = "Millennium",
                Name = "Millennium",
                Description = "Master of organization! Sync 1,000 files",
                IconGlyph = "\uE7C3", // Award
                IsUnlocked = unlockedAchievements.Contains("Millennium"),
                Progress = gamificationData.TotalFilesLifetime >= 1000 ? 1000 : gamificationData.TotalFilesLifetime,
                Target = 1000,
                UnlockCriteria = "Sync 1,000 files total"
            },
            new AchievementData
            {
                Id = "DuplicateHunter",
                Name = "Duplicate Hunter",
                Description = "Find and eliminate 100 duplicate files",
                IconGlyph = "\uE8FB", // Search
                IsUnlocked = unlockedAchievements.Contains("DuplicateHunter"),
                Progress = gamificationData.TotalDuplicatesLifetime >= 100 ? 100 : gamificationData.TotalDuplicatesLifetime,
                Target = 100,
                UnlockCriteria = "Find 100 duplicate files"
            },
            new AchievementData
            {
                Id = "SpeedDemon",
                Name = "Speed Demon",
                Description = "Process files at lightning speed!",
                IconGlyph = "\uE945", // Lightning
                IsUnlocked = unlockedAchievements.Contains("SpeedDemon"),
                Progress = 0, // Speed-based, can't show progress easily
                Target = 1,
                UnlockCriteria = "Process 50+ files per minute"
            },
            new AchievementData
            {
                Id = "DataMaster",
                Name = "Data Master",
                Description = "Process over 10GB of files",
                IconGlyph = "\uE8B7", // Save
                IsUnlocked = unlockedAchievements.Contains("DataMaster"),
                Progress = (int)(gamificationData.TotalBytesLifetime / (1024L * 1024 * 1024)),
                Target = 10,
                UnlockCriteria = "Process 10GB of data"
            },
            new AchievementData
            {
                Id = "PointsMaster",
                Name = "Points Master",
                Description = "Earn 10,000 points through your efforts",
                IconGlyph = "\uE753", // Star
                IsUnlocked = unlockedAchievements.Contains("PointsMaster"),
                Progress = gamificationData.TotalPoints >= 10000 ? 10000 : gamificationData.TotalPoints,
                Target = 10000,
                UnlockCriteria = "Earn 10,000 total points"
            },
            new AchievementData
            {
                Id = "Perfectionist",
                Name = "Perfectionist",
                Description = "Complete 10 syncs with zero errors",
                IconGlyph = "\uE734", // Favorite
                IsUnlocked = unlockedAchievements.Contains("Perfectionist"),
                Progress = 0, // Would need to track this separately
                Target = 10,
                UnlockCriteria = "Complete 10 error-free syncs"
            }
        };
        
        Achievements = new ObservableCollection<AchievementData>(allAchievements);
    }
}
