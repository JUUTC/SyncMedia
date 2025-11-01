# Windows App Store Migration Guide

## Overview

SyncMedia has been migrated to support packaging and distribution through the Microsoft Store (Windows App Store). This migration includes:

1. **Windows Application Packaging Project** - MSIX packaging for Store distribution
2. **Updated Storage Mechanism** - Settings now stored in LocalApplicationData
3. **Store-Ready Manifest** - Proper app identity and capabilities configured

## What Changed

### Storage Location

**Before:**
- Settings stored in `app.config` file next to the executable
- Location: `%ProgramFiles%\SyncMedia\SyncMedia.exe.config`

**After:**
- Settings stored in XML file in LocalApplicationData
- Location: `%LOCALAPPDATA%\SyncMedia\settings.xml`
- Works for both packaged and unpackaged apps

### Project Structure

**New Files Added:**
```
SyncMedia.Package/
├── Package.appxmanifest          # App manifest for Store
├── SyncMedia.Package.wapproj     # Packaging project
├── README.md                      # Packaging documentation
└── Images/                        # Visual assets
    ├── Square44x44Logo.png
    ├── Square150x150Logo.png
    ├── Wide310x150Logo.png
    ├── SplashScreen.png
    └── StoreLogo.png
```

**Modified Files:**
- `SyncMedia.csproj` - Updated target framework for Windows 10 SDK
- `SyncMedia.sln` - Added packaging project
- `XmlData.cs` - Replaced ConfigurationManager with file-based settings

### Code Changes

#### XmlData.cs
- Removed dependency on `System.Configuration.ConfigurationManager`
- Implemented file-based XML settings storage
- Settings directory: `Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SyncMedia")`
- Settings file: `settings.xml` with structure:
  ```xml
  <settings>
    <setting key="SourceFolder" value="C:\..." />
    <setting key="DestinationFolder" value="C:\..." />
    ...
  </settings>
  ```

#### SyncMedia.csproj
- Updated `TargetFramework` from `net9.0-windows` to `net9.0-windows10.0.19041.0`
- Added `TargetPlatformMinVersion` for Windows 10 compatibility
- Added `RuntimeIdentifiers` for x64 and ARM64 support
- Configured for packaging compatibility

## Benefits

### For Users
1. **Install via Microsoft Store** - Easy installation and automatic updates
2. **Secure Installation** - App sandboxing and security
3. **Settings Preservation** - Settings survive app reinstallation
4. **UAC Friendly** - No admin rights needed for settings

### For Developers
1. **Modern Distribution** - Reach users through the Microsoft Store
2. **Automatic Updates** - Store handles update delivery
3. **Code Signing** - Store provides trusted signing
4. **Analytics** - Store provides usage analytics

## Building the Package

### Prerequisites
- Windows 10/11
- Visual Studio 2022 with "Desktop development with C++" workload
- Windows 10 SDK (10.0.17763.0 or higher)

### Steps

1. **Open Solution**
   ```
   Open SyncMedia.sln in Visual Studio
   ```

2. **Build Main Project**
   ```
   Right-click SyncMedia project → Build
   ```

3. **Build Package**
   ```
   Right-click SyncMedia.Package project → Build
   or
   Right-click SyncMedia.Package project → Publish → Create App Packages
   ```

4. **Output Location**
   ```
   SyncMedia.Package\AppPackages\SyncMedia.Package_[version]_Test\
   ```

### Command Line Build (Windows only)

```cmd
:: Restore NuGet packages
dotnet restore

:: Build the application
msbuild SyncMedia\SyncMedia.csproj /p:Configuration=Release

:: Build the package
msbuild SyncMedia.Package\SyncMedia.Package.wapproj /p:Configuration=Release /p:Platform=x64 /p:AppxBundle=Always
```

## Publishing to Microsoft Store

### One-Time Setup

1. **Create Developer Account**
   - Go to https://partner.microsoft.com/dashboard
   - Sign up for a developer account ($19 one-time fee for individual)

2. **Reserve App Name**
   - In Partner Center, create new app
   - Reserve the name "SyncMedia"
   - Get your Publisher ID and Publisher Display Name

3. **Update Manifest**
   - Edit `SyncMedia.Package\Package.appxmanifest`
   - Update `Identity` Publisher to your Publisher ID
   - Update `PublisherDisplayName` to your name

4. **Create Certificate**
   - In Visual Studio, right-click SyncMedia.Package
   - Select "Publish" → "Create App Packages"
   - Follow wizard to create certificate
   - Or use certificate from Partner Center

### Publishing Process

1. **Create Package for Submission**
   ```
   Right-click SyncMedia.Package → Publish → Create App Packages
   Choose "Microsoft Store as [your account]"
   Select app name from dropdown
   Configure version number and architecture
   Create package
   ```

2. **Upload to Store**
   - Go to Partner Center
   - Navigate to your app submission
   - Upload the .msixupload file from AppPackages folder
   - Fill in store listing details
   - Submit for certification

3. **Certification**
   - Microsoft reviews (typically 1-3 days)
   - App goes live after approval

## Migration Path for Existing Users

### Automatic Settings Migration

The app will automatically work for existing users because:

1. **First Launch Detection**
   - New storage location is empty
   - User will need to reconfigure folders
   - Settings are saved to new location

2. **No Data Loss**
   - Hash database files are stored in user-selected destination folder
   - Not affected by settings storage change

### Manual Migration (Optional)

Users can manually copy their old settings:

1. **Old Settings Location**
   - Find: `%ProgramFiles%\SyncMedia\SyncMedia.exe.config`
   - Or: Installation directory

2. **New Settings Location**
   - `%LOCALAPPDATA%\SyncMedia\settings.xml`

3. **Settings to Migrate**
   - SourceFolder
   - DestinationFolder
   - RejectFolder
   - EmergencySave
   - TotalPoints, TotalFilesLifetime, etc.

## Testing

### Test Unpackaged Version
```bash
dotnet run --project SyncMedia\SyncMedia.csproj
```

### Test Packaged Version
1. Build the package
2. Right-click the .msix file in AppPackages folder
3. Select "Install"
4. Run from Start Menu

### Verify Settings Storage
1. Run the app
2. Configure folders
3. Close app
4. Check `%LOCALAPPDATA%\SyncMedia\settings.xml` exists
5. Reopen app - settings should be preserved

## Troubleshooting

### Build Errors

**Error: Cannot find Windows 10 SDK**
- Install Windows 10 SDK from Visual Studio Installer
- Minimum version: 10.0.17763.0

**Error: Cannot build package**
- Ensure Visual Studio has "Desktop Bridge" components
- Update to latest Visual Studio 2022

### Runtime Issues

**Settings not saving**
- Check folder permissions for `%LOCALAPPDATA%\SyncMedia`
- Run app as regular user (not admin)

**App won't install from Store**
- Check Windows version (must be Windows 10 1809 or later)
- Ensure Windows Update is current

## File Locations Reference

### Development
```
Solution Root/
├── SyncMedia/                    # Main application
│   ├── SyncMedia.csproj
│   ├── XmlData.cs               # Settings storage
│   └── ...
├── SyncMedia.Package/            # Packaging project
│   ├── Package.appxmanifest     # App manifest
│   ├── SyncMedia.Package.wapproj
│   └── Images/                   # Store assets
└── SyncMedia.Tests/              # Unit tests
```

### User Data (Runtime)
```
%LOCALAPPDATA%\SyncMedia\
└── settings.xml                  # User settings

[User-Selected Destination]/
└── *.xml                         # Hash databases
```

### Installed App (Store Package)
```
C:\Program Files\WindowsApps\SyncMedia_[version]_x64__[publisherId]\
├── SyncMedia.exe
└── [dependencies]
```

## Future Enhancements

Potential improvements for Store distribution:

1. **Themed Icons** - Create proper branded logos for all sizes
2. **Store Screenshots** - Add compelling app screenshots
3. **Localization** - Add multi-language support
4. **In-App Ratings** - Prompt for Store ratings
5. **Crash Reporting** - Add telemetry for better support

## Additional Resources

- [Microsoft Store Documentation](https://docs.microsoft.com/windows/uwp/publish/)
- [MSIX Packaging](https://docs.microsoft.com/windows/msix/)
- [Desktop Bridge](https://docs.microsoft.com/windows/apps/desktop/modernize/)
- [Partner Center](https://partner.microsoft.com/dashboard)
