# SyncMedia

[![Microsoft Store Ready](https://img.shields.io/badge/Microsoft%20Store-Ready-brightgreen)](WINDOWS_STORE_MIGRATION.md)

Copy pictures and video from source folder to destination folder tree and prevents dupes by hashing each file and saving
a xml list of hashes.

## Installation

### Microsoft Store (Recommended)
This application is packaged and ready for Microsoft Store distribution. See [Windows Store Migration Guide](WINDOWS_STORE_MIGRATION.md) for publishing instructions.

### Manual Installation
Download and run the installer from the [Releases](../../releases) page.

## Usage

Once the application starts you will need to "set" three folders (This will be saved).
The first is the folder you downloaded your files from your camera or phone to. 
This folder needs to be outside of the Destination folder or it will make a mess.

For example you shouldn't set the source and destination folder like c:\pictures\ and c:\pictures\sorted.
The source folder and all sub folders will be imported.

A good example would be c:\pictures\ and c:\users\johndoe\my pictures\

The destination folder should be set to an empty folder the first time you use the application.

The reject duplicate folder should also be a new empty folder outside of the source folder.

A good example would be to create a folder in your destination folder. c:\users\johndoe\my pictures\rejects\

If you are syncing multiple devices when you import them give them unique names.
For example when Jane imports her iPhone she labels them "Jane iPhone - date-time".
The "Update Naming List" will search the source folder for possible naming conventions.
Check the box next to the names you want to retained on the files after they are imported.
If none are selected all the files will be named in the following way:

For pictures: Date Taken - sequence number from this import (a number from 001 to the total count of files you are importing) 
For movies: Date Modified - sequence number from this import (a number from 001 to the total count of files you are importing)

Once all of your folders are set and any names checked you can press "Sync Media" and the gray area will 
populate with the file names and a status message for each.

Each file is hashed to get a unique signature so the system will not place duplicate files into the folder structure.

The application will only sort the following file types:
.jpg, .png, .bmp, .jpeg, .gif, .tif, .tiff, .mov, .mp4, .wmv, .avi, .m4v, .mpg and .mpeg

## Settings Storage

Your folder preferences and gamification data are stored in:
```
%LOCALAPPDATA%\SyncMedia\settings.xml
```

This location ensures your settings are preserved across app updates and work properly with Windows Store packaging.

## Building from Source

### Requirements
- .NET 9.0 SDK
- Windows 10 SDK (10.0.17763.0 or higher) for packaging
- Visual Studio 2022 (optional, for packaging project)

### Build Instructions

```bash
# Clone the repository
git clone https://github.com/JUUTC/SyncMedia.git
cd SyncMedia

# Restore dependencies
dotnet restore

# Build the application
dotnet build -c Release

# Run the application
dotnet run --project SyncMedia\SyncMedia.csproj
```

### Creating Microsoft Store Package

See [Windows Store Migration Guide](WINDOWS_STORE_MIGRATION.md) for detailed instructions on creating and publishing the MSIX package.

## Architecture

The application follows a clean architecture with separation of concerns. See [ARCHITECTURE.md](SyncMedia/ARCHITECTURE.md) for detailed documentation.

### Core Library Features

SyncMedia.Core provides a robust, production-ready foundation:

- **Error Handling**: Centralized error management with `IErrorHandler` interface
- **Logging**: Structured logging using Microsoft.Extensions.Logging
- **Input Validation**: Secure path validation with `PathValidator` to prevent path traversal attacks
- **Testing**: Comprehensive test suite with 139+ tests and 27%+ code coverage

### Code Quality

- ✅ Exception handling in all async methods
- ✅ Centralized error management
- ✅ Input validation on all file paths
- ✅ Structured logging for diagnostics
- ✅ Unit tests for business logic
- ✅ Cross-platform compatibility

## Testing

Run the test suite:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

## License

See [LICENSE](LICENSE) file for details.
