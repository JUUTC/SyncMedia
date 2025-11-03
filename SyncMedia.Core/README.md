# SyncMedia.Core

## Overview

SyncMedia.Core is a shared business logic library extracted from the monolithic SyncMedia Windows Forms application. This library contains platform-independent code that can be used by both the legacy Windows Forms app and the modern WinUI 3 app.

## Purpose

This library is part of Phase 3 (WinUI 3 Migration) of the SyncMedia modernization roadmap. By extracting business logic into a separate library:

- **Code Reuse**: Share logic between Windows Forms and WinUI 3 versions
- **Separation of Concerns**: Keep UI and business logic separate
- **Testability**: Easier to write unit tests for business logic
- **Maintainability**: Changes to business logic don't affect UI code
- **Gradual Migration**: Allows incremental migration to WinUI 3

## Structure

```
SyncMedia.Core/
├── Constants/          # Application constants (media types, settings keys)
├── Models/             # Data models (GamificationData, SyncStatistics)
├── Services/           # Business services (GamificationService)
├── Helpers/            # Utility helpers (FileHelper, PerformanceOptimizer)
├── Interfaces/         # Service interfaces for dependency injection
└── XmlData.cs          # Settings storage management
```

## Contents

### Constants
- **MediaConstants.cs**: Defines supported image and video file extensions

### Models
- **GamificationData.cs**: Data model for gamification system (achievements, stats)
- **SyncStatistics.cs**: Data model for sync operation statistics

### Services
- **GamificationService.cs**: Manages achievements, streaks, and gamification logic

### Helpers
- **FileHelper.cs**: File system utilities and operations
- **FilePreviewHelper.cs**: Manages file preview display (images and videos)
- **GamificationPersistence.cs**: Persists gamification data to storage
- **PerformanceBenchmark.cs**: Performance measurement and profiling
- **PerformanceOptimizer.cs**: Performance optimization utilities

### Core
- **XmlData.cs**: XML-based settings storage (LocalApplicationData)

## Dependencies

- **.NET 9.0**: Target framework
- **System.Drawing.Common**: For image manipulation (will be replaced in WinUI 3 version)

## Usage

### From SyncMedia (Windows Forms)

```csharp
using SyncMedia.Core.Services;
using SyncMedia.Core.Models;

// Use the gamification service
var gamificationService = new GamificationService();
var data = gamificationService.LoadData();
```

### From SyncMedia.WinUI (Future)

```csharp
using SyncMedia.Core.Services;
using SyncMedia.Core.Interfaces;

// Inject via dependency injection
public MainViewModel(IGamificationService gamificationService)
{
    _gamificationService = gamificationService;
}
```

## Migration Notes

### Namespace Changes
All code in this library uses the `SyncMedia.Core` namespace instead of `SyncMedia`.

### Platform Dependencies
Some helpers (like FilePreviewHelper) currently use Windows Forms controls. These will be abstracted with interfaces for WinUI 3 compatibility.

### Future Enhancements
- Add interfaces for all services (IGamificationService, IFileService)
- Abstract UI-dependent code
- Add comprehensive unit tests
- Remove System.Drawing.Common dependency for cross-platform compatibility

## Version History

- **v1.0** (Phase 3 Week 1): Initial extraction of business logic from SyncMedia Windows Forms application

## Related Projects

- **SyncMedia**: Legacy Windows Forms application (maintenance mode)
- **SyncMedia.WinUI**: Modern WinUI 3 application (in development)
- **SyncMedia.Package**: MSIX packaging project

## Documentation

See `PHASE3_WINUI_MIGRATION.md` for the complete WinUI 3 migration plan and timeline.
