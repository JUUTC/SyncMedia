# SyncMedia - Code Architecture

## Overview
This document describes the refactored architecture of the SyncMedia application, which has been restructured from a monolithic design into a well-organized, maintainable codebase following best practices.

## Folder Structure

```
SyncMedia/
├── Constants/              # Application constants and configuration
│   └── MediaConstants.cs   # Media file extensions, points system config
├── Models/                 # Data models and entities  
│   ├── SyncStatistics.cs   # Session statistics model
│   └── GamificationData.cs # Gamification data model
├── Services/               # Business logic services
│   └── GamificationService.cs # Achievement and points logic
├── Helpers/                # Utility and helper classes
│   ├── FileHelper.cs       # File and folder operations
│   └── GamificationPersistence.cs # Data persistence
├── Properties/             # Assembly and resource properties
├── SyncMedia.cs            # Main form UI logic
├── SyncMedia.Designer.cs   # Form designer code
├── Program.cs              # Application entry point
└── XmlData.cs              # XML configuration management
```

## Architecture Principles

### 1. Separation of Concerns
- **Constants**: All magic numbers and configuration values centralized
- **Models**: Pure data structures with no business logic
- **Services**: Business logic separated from UI code
- **Helpers**: Reusable utility functions

### 2. Single Responsibility
Each class has one clear purpose:
- `MediaConstants`: Defines supported formats and configuration
- `SyncStatistics`: Tracks session metrics
- `GamificationData`: Manages points and achievements
- `GamificationService`: Implements achievement logic
- `FileHelper`: File/folder operations
- `GamificationPersistence`: Data storage/retrieval

### 3. Dependency Flow
```
UI Layer (SyncMedia.cs)
    ↓
Services Layer (GamificationService)
    ↓
Models Layer (SyncStatistics, GamificationData)
    ↓
Helpers/Constants Layer
```

## Key Components

### Constants (`MediaConstants.cs`)
- **Purpose**: Centralized configuration
- **Contents**:
  - Supported image formats (12 types)
  - Supported video formats (16 types)
  - Points system configuration
  - Speed bonus thresholds
  - UI update batch sizes

### Models

#### `SyncStatistics.cs`
Tracks metrics for a single sync session:
- Files processed
- Duplicates found
- Errors encountered
- Bytes processed
- Processing speed calculations
- Time tracking

#### `GamificationData.cs`
Manages lifetime gamification state:
- Total points (session + lifetime)
- Lifetime file/duplicate/byte counts
- Achievement collection
- Helper methods for achievement management

### Services

#### `GamificationService.cs`
Implements all gamification logic:
- **Points Calculation**: Awards points based on activity
- **Achievement System**: 200+ achievements across 9 categories
  1. File Count Milestones (17 tiers)
  2. Data Size Milestones (14 tiers)
  3. Duplicate Hunter (10 tiers)
  4. Points Progression (12 tiers)
  5. Perfection Achievements (5 tiers)
  6. Speed Achievements (5 tiers)
  7. Daily Session Achievements (daily reset)
  8. Combo Achievements (multi-condition)
  9. Legendary Achievements (ultra-rare)

### Helpers

#### `FileHelper.cs`
Utility functions for file operations:
- File type detection (image/video)
- Filename cleaning
- Path manipulation
- Date formatting
- Folder validation

#### `GamificationPersistence.cs`
Handles saving/loading gamification data:
- Load from XML settings
- Save to XML settings
- Error handling for first-time users

## Benefits of This Architecture

### Maintainability
- ✅ Easy to locate and modify specific functionality
- ✅ Clear file and folder organization
- ✅ Reduced code duplication

### Testability
- ✅ Services can be unit tested independently
- ✅ Models are simple POCOs (Plain Old CLR Objects)
- ✅ Helpers have no dependencies

### Scalability
- ✅ Easy to add new achievement types
- ✅ Simple to extend with new file formats
- ✅ Can add new services without modifying existing code

### Readability
- ✅ Each file has a focused purpose
- ✅ Meaningful namespace organization
- ✅ Self-documenting structure

## Migration Notes

### From Monolithic Design
The original `SyncMedia.cs` (1167 lines) has been refactored into:
- 1 Constants class (~70 lines)
- 2 Model classes (~80 lines total)
- 1 Service class (~400 lines)
- 2 Helper classes (~200 lines total)
- Main UI class (reduced significantly)

### Breaking Changes
None - the public API remains the same. This is an internal refactoring.

## Usage Examples

### Using GamificationService
```csharp
// Initialize
var gamificationData = GamificationPersistence.LoadData();
var gamificationService = new GamificationService(gamificationData);
var stats = new SyncStatistics();

// After sync completes
gamificationService.AwardPoints(stats);
var newAchievements = gamificationService.CheckAchievements(stats);
GamificationPersistence.SaveData(gamificationData);
```

### Using FileHelper
```csharp
// Check file types
bool isImage = FileHelper.IsImageFile(".jpg");
bool isVideo = FileHelper.IsVideoFile(".mp4");

// Clean filename
string cleaned = FileHelper.CleanFileName("IMG_2024-01-01_photo.jpg");

// Validate folder
bool isValid = FileHelper.ValidateFolderPath(@"C:\Photos");
```

## Future Enhancements

### Potential Additions
1. **IRepository Pattern**: Abstract data persistence
2. **Dependency Injection**: Use IoC container
3. **Async/Await**: Async file processing service
4. **Unit Tests**: Add comprehensive test coverage
5. **Logging**: Structured logging service
6. **Configuration**: appsettings.json instead of XML

### Recommended Next Steps
1. Add XML documentation to all public methods
2. Implement interface abstractions for services
3. Add unit test project
4. Consider MVVM pattern for better UI separation
5. Add logging framework (Serilog, NLog, etc.)

## Conclusion

This refactored architecture provides a solid foundation for continued development while maintaining backward compatibility with existing functionality.
