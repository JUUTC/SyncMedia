using System;
using System.Collections.Generic;
using SyncMedia.Constants;
using SyncMedia.Models;

namespace SyncMedia.Services
{
    /// <summary>
    /// Service for managing gamification features including points calculation and achievement tracking
    /// </summary>
    public class GamificationService
    {
        private readonly GamificationData _data;
        
        public GamificationService(GamificationData data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }
        
        /// <summary>
        /// Calculate and award points based on session statistics
        /// </summary>
        public void AwardPoints(SyncStatistics stats)
        {
            _data.ResetSessionPoints();
            
            // Base points
            _data.AddSessionPoints(stats.TotalFilesProcessed * MediaConstants.POINTS_PER_FILE);
            _data.AddSessionPoints(stats.DuplicatesFound * MediaConstants.POINTS_PER_DUPLICATE);
            _data.AddSessionPoints((int)(stats.TotalBytesProcessed / (1024 * 1024)) * MediaConstants.POINTS_PER_MB);
            
            // Speed bonuses (tiered)
            if (stats.FilesPerMinute >= MediaConstants.SPEED_LIGHTNING_THRESHOLD)
                _data.AddSessionPoints(MediaConstants.SPEED_LIGHTNING_BONUS);
            else if (stats.FilesPerMinute >= MediaConstants.SPEED_SUPER_THRESHOLD)
                _data.AddSessionPoints(MediaConstants.SPEED_SUPER_BONUS);
            else if (stats.FilesPerMinute >= MediaConstants.SPEED_DEMON_THRESHOLD)
                _data.AddSessionPoints(MediaConstants.SPEED_DEMON_BONUS);
            else if (stats.FilesPerMinute >= MediaConstants.SPEED_QUICK_THRESHOLD)
                _data.AddSessionPoints(MediaConstants.SPEED_QUICK_BONUS);
            
            // Update lifetime statistics
            _data.UpdateLifetimeStats(stats);
        }
        
        /// <summary>
        /// Check for new achievements and return list of newly unlocked achievements
        /// </summary>
        public List<string> CheckAchievements(SyncStatistics stats)
        {
            var newAchievements = new List<string>();
            
            // File count achievements
            newAchievements.AddRange(CheckFileMilestones());
            
            // Data size achievements
            newAchievements.AddRange(CheckDataMilestones());
            
            // Duplicate hunter achievements
            newAchievements.AddRange(CheckDuplicateMilestones());
            
            // Points achievements
            newAchievements.AddRange(CheckPointsMilestones());
            
            // Perfection achievements
            newAchievements.AddRange(CheckPerfectionAchievements(stats));
            
            // Speed achievements
            newAchievements.AddRange(CheckSpeedAchievements(stats));
            
            // Session achievements
            newAchievements.AddRange(CheckSessionAchievements(stats));
            
            // Combo achievements
            newAchievements.AddRange(CheckComboAchievements());
            
            // Legendary achievements
            newAchievements.AddRange(CheckLegendaryAchievements());
            
            return newAchievements;
        }
        
        private List<string> CheckFileMilestones()
        {
            var achievements = new List<string>();
            var milestones = new[]
            {
                (10, "FirstTen", "🌱 First Ten", "Getting started!"),
                (25, "QuarterCentury", "🌿 Quarter Century", "25 files organized!"),
                (50, "HalfCentury", "🌳 Half Century", "50 files strong!"),
                (100, "Century", "🏆 Century Club", "100 files synced!"),
                (250, "QuarterK", "⭐ Quarter K", "250 files organized!"),
                (500, "HalfK", "🌟 Half K", "500 files milestone!"),
                (1000, "Millennium", "💫 Millennium", "1,000 files!"),
                (2500, "TwoPointFiveK", "✨ 2.5K Organizer", "2,500 files!"),
                (5000, "FiveK", "🎯 5K Master", "5,000 files!"),
                (10000, "TenK", "🏅 10K Epic", "10,000 files!"),
                (25000, "TwentyFiveK", "👑 25K Champion", "25,000 files!"),
                (50000, "FiftyK", "💎 50K Legend", "50,000 files!"),
                (75000, "SeventyFiveK", "🔥 75K Elite", "75,000 files!"),
                (100000, "HundredK", "🌈 100K Grandmaster", "100,000 files!"),
                (250000, "TwoFiftyK", "🚀 250K Titan", "250,000 files!"),
                (500000, "HalfMillion", "⚡ Half Million", "500,000 files!"),
                (1000000, "Million", "🎖️ MILLIONAIRE", "1,000,000 files!!!"),
            };
            
            foreach (var (count, id, name, desc) in milestones)
            {
                if (_data.TotalFilesLifetime >= count && !_data.HasAchievement(id))
                {
                    _data.AddAchievement(id);
                    achievements.Add($"{name} - {desc}");
                }
            }
            
            return achievements;
        }
        
        private List<string> CheckDataMilestones()
        {
            var achievements = new List<string>();
            var milestones = new[]
            {
                (100L * 1024 * 1024, "HundredMB", "📦 100 MB Club", "100 MB synced!"),
                (500L * 1024 * 1024, "HalfGB", "📦 500 MB Master", "500 MB synced!"),
                (1024L * 1024 * 1024, "OneGB", "💾 Gigabyte Club", "1 GB synced!"),
                (5L * 1024 * 1024 * 1024, "FiveGB", "💾 5 GB Veteran", "5 GB synced!"),
                (10L * 1024 * 1024 * 1024, "TenGB", "💿 10 GB Master", "10 GB synced!"),
                (25L * 1024 * 1024 * 1024, "TwentyFiveGB", "💿 25 GB Pro", "25 GB synced!"),
                (50L * 1024 * 1024 * 1024, "FiftyGB", "📀 50 GB Expert", "50 GB synced!"),
                (100L * 1024 * 1024 * 1024, "HundredGB", "📀 100 GB Elite", "100 GB synced!"),
                (250L * 1024 * 1024 * 1024, "TwoFiftyGB", "🗄️ 250 GB Champion", "250 GB synced!"),
                (500L * 1024 * 1024 * 1024, "HalfTB", "🗄️ Half Terabyte", "500 GB synced!"),
                (1024L * 1024 * 1024 * 1024, "OneTB", "🏆 TERABYTE TITAN", "1 TB synced!!!"),
                (2L * 1024 * 1024 * 1024 * 1024, "TwoTB", "👑 2 TB Monarch", "2 TB synced!"),
                (5L * 1024 * 1024 * 1024 * 1024, "FiveTB", "💎 5 TB Legend", "5 TB synced!"),
                (10L * 1024 * 1024 * 1024 * 1024, "TenTB", "🌟 10 TB GODLIKE", "10 TB synced!!!"),
            };
            
            foreach (var (bytes, id, name, desc) in milestones)
            {
                if (_data.TotalBytesLifetime >= bytes && !_data.HasAchievement(id))
                {
                    _data.AddAchievement(id);
                    achievements.Add($"{name} - {desc}");
                }
            }
            
            return achievements;
        }
        
        private List<string> CheckDuplicateMilestones()
        {
            var achievements = new List<string>();
            var milestones = new[]
            {
                (10, "DupeNovice", "🔍 Dupe Novice", "Found 10 duplicates!"),
                (25, "DupeDetective", "🔎 Dupe Detective", "Found 25 duplicates!"),
                (50, "DupeHunter", "🎯 Dupe Hunter", "Found 50 duplicates!"),
                (100, "DupeExpert", "🏹 Dupe Expert", "Found 100 duplicates!"),
                (250, "DupeMaster", "🎖️ Dupe Master", "Found 250 duplicates!"),
                (500, "DupeEliminator", "⚔️ Dupe Eliminator", "Found 500 duplicates!"),
                (1000, "DupeExterminator", "🗡️ Dupe Exterminator", "Found 1,000 duplicates!"),
                (2500, "DupeAnnihilator", "💀 Dupe Annihilator", "Found 2,500 duplicates!"),
                (5000, "DupeNemesis", "👿 Dupe Nemesis", "Found 5,000 duplicates!"),
                (10000, "DupeDestroyer", "🔥 DUPE DESTROYER", "Found 10,000 duplicates!!!"),
            };
            
            foreach (var (count, id, name, desc) in milestones)
            {
                if (_data.TotalDuplicatesLifetime >= count && !_data.HasAchievement(id))
                {
                    _data.AddAchievement(id);
                    achievements.Add($"{name} - {desc}");
                }
            }
            
            return achievements;
        }
        
        private List<string> CheckPointsMilestones()
        {
            var achievements = new List<string>();
            var milestones = new[]
            {
                (1000, "Rookie", "🎮 Rookie", "1,000 points!"),
                (5000, "Apprentice", "🎯 Apprentice", "5,000 points!"),
                (10000, "Skilled", "⭐ Skilled", "10,000 points!"),
                (25000, "Veteran", "🌟 Veteran", "25,000 points!"),
                (50000, "Expert", "💫 Expert", "50,000 points!"),
                (100000, "Master", "✨ Master", "100,000 points!"),
                (250000, "GrandMaster", "👑 Grand Master", "250,000 points!"),
                (500000, "Legend", "💎 Legend", "500,000 points!"),
                (1000000, "Mythic", "🔥 MYTHIC", "1,000,000 points!!!"),
                (2500000, "Immortal", "⚡ IMMORTAL", "2,500,000 points!!!"),
                (5000000, "Divine", "🌈 DIVINE", "5,000,000 points!!!"),
                (10000000, "Transcendent", "🎖️ TRANSCENDENT", "10,000,000 points!!!"),
            };
            
            foreach (var (points, id, name, desc) in milestones)
            {
                if (_data.TotalPoints >= points && !_data.HasAchievement(id))
                {
                    _data.AddAchievement(id);
                    achievements.Add($"{name} - {desc}");
                }
            }
            
            return achievements;
        }
        
        private List<string> CheckPerfectionAchievements(SyncStatistics stats)
        {
            var achievements = new List<string>();
            
            if (stats.TotalFilesProcessed >= 10 && stats.ErrorsEncountered == 0 && !_data.HasAchievement("FlawlessTen"))
            {
                _data.AddAchievement("FlawlessTen");
                achievements.Add("✅ Perfect Ten - 10 files, 0 errors!");
            }
            if (stats.TotalFilesProcessed >= 50 && stats.ErrorsEncountered == 0 && !_data.HasAchievement("FlawlessFifty"))
            {
                _data.AddAchievement("FlawlessFifty");
                achievements.Add("✅ Perfect Fifty - 50 files, 0 errors!");
            }
            if (stats.TotalFilesProcessed >= 100 && stats.ErrorsEncountered == 0 && !_data.HasAchievement("FlawlessHundred"))
            {
                _data.AddAchievement("FlawlessHundred");
                achievements.Add("✅ Flawless Century - 100 files, 0 errors!");
            }
            if (stats.TotalFilesProcessed >= 500 && stats.ErrorsEncountered == 0 && !_data.HasAchievement("FlawlessFiveHundred"))
            {
                _data.AddAchievement("FlawlessFiveHundred");
                achievements.Add("✅ Flawless 500 - 500 files, 0 errors!");
            }
            if (stats.TotalFilesProcessed >= 1000 && stats.ErrorsEncountered == 0 && !_data.HasAchievement("FlawlessThousand"))
            {
                _data.AddAchievement("FlawlessThousand");
                achievements.Add("✅ PERFECT MILLENNIUM - 1,000 files, 0 errors!!!");
            }
            
            return achievements;
        }
        
        private List<string> CheckSpeedAchievements(SyncStatistics stats)
        {
            var achievements = new List<string>();
            
            if (stats.FilesPerMinute >= 5 && !_data.HasAchievement("SpeedsterV"))
            {
                _data.AddAchievement("SpeedsterV");
                achievements.Add("🏃 Speedster V - 5+ files/min!");
            }
            if (stats.FilesPerMinute >= 10 && !_data.HasAchievement("SpeedsterX"))
            {
                _data.AddAchievement("SpeedsterX");
                achievements.Add("🏃 Speedster X - 10+ files/min!");
            }
            if (stats.FilesPerMinute >= 25 && !_data.HasAchievement("SpeedsterXXV"))
            {
                _data.AddAchievement("SpeedsterXXV");
                achievements.Add("🏃 Speedster XXV - 25+ files/min!");
            }
            if (stats.FilesPerMinute >= 50 && !_data.HasAchievement("SpeedsterL"))
            {
                _data.AddAchievement("SpeedsterL");
                achievements.Add("⚡ LIGHTNING SYNC - 50+ files/min!!!");
            }
            if (stats.FilesPerMinute >= 100 && !_data.HasAchievement("SpeedsterC"))
            {
                _data.AddAchievement("SpeedsterC");
                achievements.Add("⚡ SUPERSONIC - 100+ files/min!!!");
            }
            
            return achievements;
        }
        
        private List<string> CheckSessionAchievements(SyncStatistics stats)
        {
            var achievements = new List<string>();
            
            if (stats.TotalFilesProcessed >= 100 && !_data.HasAchievement($"BigSession{DateTime.Now:yyyyMMdd}"))
            {
                string achievementId = $"BigSession{DateTime.Now:yyyyMMdd}";
                _data.AddAchievement(achievementId);
                achievements.Add("📅 Daily Century - 100+ files in one session!");
            }
            if (stats.TotalFilesProcessed >= 500 && !_data.HasAchievement($"MegaSession{DateTime.Now:yyyyMMdd}"))
            {
                string achievementId = $"MegaSession{DateTime.Now:yyyyMMdd}";
                _data.AddAchievement(achievementId);
                achievements.Add("📅 Daily 500 - 500+ files in one session!");
            }
            if (stats.TotalFilesProcessed >= 1000 && !_data.HasAchievement($"EpicSession{DateTime.Now:yyyyMMdd}"))
            {
                string achievementId = $"EpicSession{DateTime.Now:yyyyMMdd}";
                _data.AddAchievement(achievementId);
                achievements.Add("📅 DAILY THOUSAND - 1,000+ files in one session!!!");
            }
            
            return achievements;
        }
        
        private List<string> CheckComboAchievements()
        {
            var achievements = new List<string>();
            
            if (_data.TotalFilesLifetime >= 1000 && _data.TotalBytesLifetime >= 10L * 1024 * 1024 * 1024 && !_data.HasAchievement("ThousandAndTen"))
            {
                _data.AddAchievement("ThousandAndTen");
                achievements.Add("🎊 Balanced Pro - 1,000 files AND 10 GB!");
            }
            if (_data.TotalFilesLifetime >= 10000 && _data.TotalDuplicatesLifetime >= 1000 && !_data.HasAchievement("TenKAndThousandDupes"))
            {
                _data.AddAchievement("TenKAndThousandDupes");
                achievements.Add("🎊 Cleanup Master - 10K files AND 1K dupes found!");
            }
            if (_data.TotalPoints >= 100000 && _data.TotalFilesLifetime >= 5000 && !_data.HasAchievement("HundredKPoints5KFiles"))
            {
                _data.AddAchievement("HundredKPoints5KFiles");
                achievements.Add("🎊 Elite Organizer - 100K points AND 5K files!");
            }
            
            return achievements;
        }
        
        private List<string> CheckLegendaryAchievements()
        {
            var achievements = new List<string>();
            
            if (_data.TotalFilesLifetime >= 100000 && 
                _data.TotalBytesLifetime >= 1024L * 1024 * 1024 * 1024 && 
                _data.TotalPoints >= 1000000 && 
                !_data.HasAchievement("TripleThrone"))
            {
                _data.AddAchievement("TripleThrone");
                achievements.Add("👑👑👑 TRIPLE THRONE - 100K files, 1TB data, 1M points!!!");
                _data.AddSessionPoints(10000); // Huge bonus!
            }
            
            return achievements;
        }
    }
}
