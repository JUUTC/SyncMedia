namespace SyncMedia.Core.Models;

public class AchievementData
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UnlockCriteria { get; set; } = string.Empty;
    public string IconGlyph { get; set; } = "\uE73E"; // Default checkmark
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedDate { get; set; }
    public int Progress { get; set; }
    public int Target { get; set; }
}
