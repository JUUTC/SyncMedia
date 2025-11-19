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
- **ProFeatures.cs**: Defines Pro-exclusive feature flags

### Models
- **GamificationData.cs**: Data model for gamification system (achievements, stats)
- **SyncStatistics.cs**: Data model for sync operation statistics with performance metrics
- **LicenseInfo.cs**: License and trial management model
- **AchievementData.cs**: Achievement definitions and tracking

### Services
- **GamificationService.cs**: Manages achievements, streaks, and gamification logic
- **SyncService.cs**: Core sync engine with error handling and logging
- **FeatureFlagService.cs**: Manages Pro features and trial access
- **LicenseManager.cs**: License validation and management
- **ErrorHandler.cs**: Centralized error management with logging
- **IErrorHandler.cs**: Error handling service interface

### Helpers
- **FileHelper.cs**: File system utilities and operations
- **PathValidator.cs**: Secure path validation to prevent path traversal attacks
- **GamificationPersistence.cs**: Persists gamification data to storage
- **PerformanceBenchmark.cs**: Performance measurement and profiling
- **PerformanceOptimizer.cs**: Performance optimization utilities

### Core
- **XmlData.cs**: XML-based settings storage (LocalApplicationData)

## Dependencies

- **.NET 9.0**: Target framework
- **System.Drawing.Common**: For image manipulation (will be replaced in WinUI 3 version)
- **Microsoft.Extensions.Logging.Abstractions**: For structured logging
- **Microsoft.Extensions.Logging**: Logging infrastructure

## Usage

### Error Handling

```csharp
using SyncMedia.Core.Services;
using Microsoft.Extensions.Logging;

// Create error handler with logging
var logger = loggerFactory.CreateLogger<ErrorHandler>();
var errorHandler = new ErrorHandler(logger);

// Handle exceptions with context
try
{
    // Your code here
}
catch (Exception ex)
{
    errorHandler.HandleException(ex, context: "MyOperation");
    // Or rethrow after logging
    errorHandler.HandleException(ex, context: "MyOperation", rethrow: true);
}

// Check last error
string lastError = errorHandler.GetLastError();
```

### Path Validation

```csharp
using SyncMedia.Core.Helpers;

// Validate directory paths
if (!PathValidator.IsValidDirectoryPath(userPath, mustExist: true, out string error))
{
    Console.WriteLine($"Invalid path: {error}");
    return;
}

// Validate file names
if (!PathValidator.IsValidFileName(fileName, out error))
{
    Console.WriteLine($"Invalid file name: {error}");
    return;
}

// Sanitize file names
string safeName = PathValidator.SanitizeFileName("unsafe<file>name.txt");
// Returns: "unsafe_file_name.txt"
```

### SyncService with Logging

```csharp
using SyncMedia.Core.Services;
using Microsoft.Extensions.Logging;

// Create SyncService with dependencies
var logger = loggerFactory.CreateLogger<SyncService>();
var errorHandler = new ErrorHandler(errorLogger);
var syncService = new SyncService(logger, errorHandler);

// Start sync with automatic input validation and error handling
var options = new SyncOptions 
{ 
    IncludeImages = true, 
    IncludeVideos = true 
};

var result = await syncService.StartSyncAsync(
    sourceFolder: @"C:\Photos\Import",
    destinationFolder: @"C:\Photos\Library",
    options: options
);

if (result.Success)
{
    Console.WriteLine($"Synced {result.Statistics.SuccessfulFiles} files");
}
else
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
}
```

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
public MainViewModel(
    IGamificationService gamificationService,
    ILogger<MainViewModel> logger,
    IErrorHandler errorHandler)
{
    _gamificationService = gamificationService;
    _logger = logger;
    _errorHandler = errorHandler;
}
```

## Testing

The library includes comprehensive test coverage:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Current coverage: 27%+ line coverage with 139+ tests
```

### Test Suites
- **ErrorHandler**: 18 tests for error management
- **PathValidator**: 28 tests for input validation
- **SyncStatistics**: 12 tests for statistics tracking
- **LicenseInfo**: 18 tests for license management
- **FeatureFlagService**: 18 tests for feature flags
- **GamificationData**: 15 tests for gamification
- **FileHelper**: 30 tests for file operations

## Migration Notes

### Namespace Changes
All code in this library uses the `SyncMedia.Core` namespace instead of `SyncMedia`.

### Platform Dependencies
Some helpers (like FilePreviewHelper) currently use Windows Forms controls. These will be abstracted with interfaces for WinUI 3 compatibility.

### Future Enhancements
- Add interfaces for all services (IGamificationService, IFileService)
- Abstract UI-dependent code
- Increase test coverage to 60%+
- Remove System.Drawing.Common dependency for cross-platform compatibility
- Add integration tests for SyncService

## Security Features

- **Path Traversal Prevention**: PathValidator detects and blocks `..` and other dangerous patterns
- **Input Sanitization**: File names are validated and sanitized before processing
- **Secure Defaults**: All paths must be absolute and rooted
- **Error Context**: Errors are logged with context for security auditing

## Performance

- **Optimized Hash Sets**: O(1) lookups for file extension checks
- **Async/Await**: Non-blocking file operations
- **Batch Processing**: UI updates batched for efficiency
- **Memory Management**: Proper disposal and cleanup

## Version History

- **v1.2** (Phase 3 Week 3): Added error handling, logging, input validation, and comprehensive tests
- **v1.1** (Phase 3 Week 2): Added feature flags, license management, and sync service
- **v1.0** (Phase 3 Week 1): Initial extraction of business logic from SyncMedia Windows Forms application

## Related Projects

- **SyncMedia**: Legacy Windows Forms application (maintenance mode)
- **SyncMedia.WinUI**: Modern WinUI 3 application (in development)
- **SyncMedia.Package**: MSIX packaging project

## Documentation

See `PHASE3_WINUI_MIGRATION.md` for the complete WinUI 3 migration plan and timeline.
