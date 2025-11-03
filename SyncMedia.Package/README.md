# SyncMedia - Windows App Store Package

This project packages the SyncMedia application for distribution through the Microsoft Store.

## Project Structure

- **Package.appxmanifest**: App manifest defining identity, capabilities, and visual assets
- **SyncMedia.Package.wapproj**: Windows Application Packaging Project
- **Images/**: Visual assets (logos, splash screen, tiles)

## Building the Package

### Prerequisites
- Visual Studio 2022 with Windows App SDK workload
- Windows 10 SDK (version 10.0.17763.0 or higher)

### Build Steps

1. Open `SyncMedia.sln` in Visual Studio
2. Set `SyncMedia.Package` as the startup project (optional)
3. Select the target platform (x64 or ARM64)
4. Build the solution

The packaged MSIX file will be created in:
```
SyncMedia.Package\AppPackages\
```

### Command Line Build

```bash
# Restore packages
dotnet restore

# Build the main app
dotnet build SyncMedia\SyncMedia.csproj -c Release

# Build the package (requires MSBuild with Desktop Bridge tools)
msbuild SyncMedia.Package\SyncMedia.Package.wapproj /p:Configuration=Release /p:Platform=x64
```

## App Capabilities

The packaged app requires the following capabilities:
- **runFullTrust**: Allows the app to run with full trust (required for Windows Forms)
- **broadFileSystemAccess**: Allows access to user's file system for media sync operations

## Storage Location

When packaged, the app stores its settings in:
```
%LOCALAPPDATA%\SyncMedia\settings.xml
```

This location works for both packaged and unpackaged versions of the app.

## Publishing to Microsoft Store

1. Create a developer account at https://partner.microsoft.com/dashboard
2. Reserve the app name "SyncMedia"
3. Update the `Identity` section in `Package.appxmanifest` with your publisher info
4. Create and associate a certificate for signing
5. Build the package in Release mode
6. Upload the MSIX package through Partner Center

## Customizing Visual Assets

Replace the placeholder images in the `Images/` folder with your custom branding:
- **Square44x44Logo.png**: App list icon (44x44)
- **Square150x150Logo.png**: Medium tile (150x150)
- **Wide310x150Logo.png**: Wide tile (310x150)
- **SplashScreen.png**: Splash screen (620x300)
- **StoreLogo.png**: Store listing icon (50x50)

All images should be PNG format with transparent backgrounds.
