# SyncMedia - Complete Documentation

[![Microsoft Store Ready](https://img.shields.io/badge/Microsoft%20Store-Ready-brightgreen)](https://github.com/JUUTC/SyncMedia)

**Intelligent media file synchronization with duplicate detection, AI-powered organization, and flexible monetization.**

---

## Table of Contents

1. [Overview](#overview)
2. [Quick Start](#quick-start)
3. [Free vs Pro Comparison](#free-vs-pro-comparison)
4. [Advertising & Monetization](#advertising--monetization)
5. [Technical Architecture](#technical-architecture)
6. [Building from Source](#building-from-source)
7. [Testing](#testing)
8. [License Protection](#license-protection)
9. [Microsoft Store Deployment](#microsoft-store-deployment)
10. [FAQ](#faq)

---

## Overview

SyncMedia is a powerful Windows application that copies pictures and videos from a source folder to an organized destination folder tree while preventing duplicates through intelligent hashing. It features:

- **Smart Duplicate Detection**: MD5 hash-based detection (Free) + AI-powered perceptual detection (Pro)
- **Organized Storage**: Automatic date-based folder structure
- **Gamification**: Achievements and stats tracking
- **Flexible Licensing**: Ad-supported free version with unlimited Pro upgrade
- **Modern UI**: Built with WinUI 3 for Windows 11

---

## Quick Start

### Installation

**Option 1: Microsoft Store (Recommended)**
- Search for "SyncMedia" in Microsoft Store
- One-click install with automatic updates
- Hardware-bound Pro licenses for security

**Option 2: Manual Install**
- Download installer from [Releases](../../releases)
- Run the `.msix` installer
- Accept permissions

### Basic Usage

1. **Set Folders**:
   - **Source**: Where your camera/phone downloads photos (e.g., `C:\Downloads\Camera`)
   - **Destination**: Where organized files go (e.g., `C:\Users\YourName\Pictures\Organized`)
   - **Rejects**: Where duplicates are moved (e.g., `C:\Users\YourName\Pictures\Rejects`)

2. **Configure Naming** (optional):
   - Click "Update Naming List" to scan source for device names
   - Check boxes to retain original names (e.g., "iPhone", "Galaxy S23")
   - Uncheck all for automatic date-based naming

3. **Sync**:
   - Click "Sync Media" button
   - Watch progress in the log area
   - Files are hashed, organized, and duplicates rejected

**Supported File Types**: `.jpg`, `.png`, `.bmp`, `.jpeg`, `.gif`, `.tif`, `.tiff`, `.mov`, `.mp4`, `.wmv`, `.avi`, `.m4v`, `.mpg`, `.mpeg`

---

## Free vs Pro Comparison

### Free Version ✨

**Core Features**:
- ✅ Unlimited file processing (no hard limits!)
- ✅ MD5 hash-based duplicate detection
- ✅ Basic gamification and achievements
- ✅ File preview (3s images, 10s videos)

**Monetization Model**:
- 📺 **Banner ads** (728x90) at bottom of window
- 🎬 **Video ads** with sync progress display
- 🌐 **Internet required** for ad delivery
- ⏱️ **Progressive throttling**: Delay increases with file count

**Progressive Throttling Details**:
```
Formula: delay = (FilesProcessedCount / 10) × 100ms (capped at 10 seconds)

Examples:
• 0-10 files:   0-100ms    ⚡ Very fast
• 50 files:     500ms      ⚡ Fast  
• 100 files:    1000ms     🟡 Moderate
• 200 files:    2000ms     🟠 Slow
• 500 files:    5000ms     🔴 Very slow
• 1000+ files:  10000ms    🔴 Max slowdown (capped)

Reset to 0ms by:
✅ Watching complete video ad
✅ Clicking banner ad
```

**Why Progressive Throttling?**
- No frustrating hard limits or blocking
- Process as many files as you want
- Natural incentive to engage with ads when convenient
- Watch ad anytime to reset to instant processing
- Full control over your experience

**Internet Requirement**: Free version needs active internet for ad delivery. App pauses with clear messaging if connection lost. Upgrade to Pro for offline access.

### Pro Version 🚀

**Everything in Free, plus**:
- ✅ **Zero throttling** (always instant processing)
- ✅ AI-powered perceptual duplicate detection
- ✅ GPU-accelerated processing (10-100x faster)
- ✅ Find similar images (crops, edits, filters)
- ✅ Deep learning duplicate detection (CNN)
- ✅ Parallel file processing
- ✅ Advanced performance optimizations
- ❌ No ads
- ✅ **Works completely offline**

**Pro Pricing**: One-time purchase, lifetime license. Upgrade in Settings page.

### Comparison Table

| Feature | Free | Pro |
|---------|------|-----|
| File Processing | Unlimited | Unlimited |
| Throttling | Progressive (0-10s) | None (0ms) |
| Duplicate Detection | MD5 Hash | MD5 + AI Perceptual |
| GPU Acceleration | ❌ | ✅ |
| Similar Image Search | ❌ | ✅ |
| Parallel Processing | ❌ | ✅ |
| Ads | Banner + Video | None |
| Internet Required | ✅ Yes | ❌ No (offline) |
| Speed Boost (60min) | Via ads | Always active |

---

## Advertising & Monetization

### How Ads Work

**Banner Ads**:
- 728x90 standard banner at bottom of main window
- Microsoft Advertising SDK integration
- Click-through tracked for bonus (resets counter)

**Video Ads**:
- Full-screen interstitial ads
- Sync progress displayed around video
- Skip after 15 seconds
- Complete watch rewards: counter reset + 60min speed boost

**Ad-Blocking Detection**:
- 3 consecutive failures = ad blocker detected
- App pauses with explanation
- User must disable blocker or upgrade to Pro

**Connectivity Monitoring**:
- Real-time internet detection (Windows APIs)
- Event-driven (no polling)
- Instant pause when offline
- Auto-resume when connection restored

### User Experience Flow

**Free User Processing Files**:
```
Files 1-50: Fast processing (0-500ms delays)
    ↓
Files 51-100: Moderate slowdown (500ms-1s)
    ↓
Files 101-200: Noticeable delays (1-2s)
    ↓
Files 201+: Significant delays (2-10s)
    ↓
User decision: Continue slow or watch ad?
    ↓
Watch video ad → Counter resets to 0
    ↓
Back to fast processing! (+ 60min speed boost)
```

**Benefits**:
- No hard limits = no frustration
- Full control over when to engage with ads
- Unlimited ad viewing opportunities = better revenue
- Clear Pro value proposition (always fast)

### Settings Page Features

Free users see:
- Files processed this session
- Current throttle delay
- Speed boost status (if active)
- Time remaining on boost
- "Watch ad to reset" button
- Upgrade to Pro option

---

## Technical Architecture

### Core Components

```
SyncMedia/
├── SyncMedia.Core/              # Business logic
│   ├── Interfaces/
│   │   ├── IAdvertisingService.cs
│   │   ├── IConnectivityService.cs
│   │   └── IDisplayRequestService.cs
│   ├── Models/
│   │   └── LicenseInfo.cs
│   ├── Services/
│   │   ├── LicenseManager.cs
│   │   └── FeatureFlagService.cs
│   └── ...
├── SyncMedia.WinUI/             # WinUI 3 UI
│   ├── Services/
│   │   ├── MicrosoftAdvertisingService.cs
│   │   ├── ConnectivityService.cs
│   │   └── DisplayRequestService.cs
│   ├── Views/
│   │   ├── MainWindow.xaml
│   │   ├── VideoAdWithProgressPage.xaml
│   │   └── ConnectivityRequiredOverlay.xaml
│   └── ...
└── SyncMedia.Tests/             # Unit tests
    └── Core/
        ├── Models/
        └── Services/
```

### Key Services

**IAdvertisingService / MicrosoftAdvertisingService**:
- Banner ad management (AdControl)
- Video ad integration (InterstitialAd)
- Ad-blocking detection (3-failure threshold)
- Event-driven state tracking

**IConnectivityService / ConnectivityService**:
- Real-time network monitoring
- Windows networking APIs
- Event-driven updates (NetworkStatusChanged)
- Zero polling for efficiency

**IDisplayRequestService / DisplayRequestService**:
- Screen sleep prevention
- Active during file processing and video ads
- Windows.System.Display.DisplayRequest API

**LicenseManager**:
- License activation/deactivation
- File counter tracking
- Counter reset mechanism
- Hardware binding (store builds)
- License persistence

**FeatureFlagService**:
- Progressive throttle calculation
- Feature flag management
- Pro access determination
- Speed boost tracking

### Progressive Throttling Logic

```csharp
public int GetThrottleDelayMs()
{
    // Pro users: always instant
    if (HasProAccess) return 0;
    
    // Speed boost active: instant
    if (_licenseManager.CurrentLicense.HasActiveSpeedBoost) return 0;
    
    // Free users: progressive formula
    int filesProcessed = _licenseManager.CurrentLicense.FilesProcessedCount;
    int delay = (filesProcessed / 10) * 100;  // 100ms per 10 files
    
    // Cap at 10 seconds
    return Math.Min(delay, 10000);
}
```

### Dependency Injection

All services registered in `App.xaml.cs`:
```csharp
services.AddSingleton<IConnectivityService, ConnectivityService>();
services.AddSingleton<IAdvertisingService, MicrosoftAdvertisingService>();
services.AddSingleton<IDisplayRequestService, DisplayRequestService>();
services.AddSingleton<LicenseManager>();
services.AddSingleton<FeatureFlagService>();
```

---

## Building from Source

### Requirements

- .NET 9.0 SDK
- Windows 10 SDK (10.0.17763.0+)
- Visual Studio 2022 (optional, for packaging)

### Build Steps

```bash
# Clone repository
git clone https://github.com/JUUTC/SyncMedia.git
cd SyncMedia

# Restore dependencies
dotnet restore

# Build solution
dotnet build -c Release

# Run application
dotnet run --project SyncMedia.WinUI/SyncMedia.WinUI.csproj
```

### Build Configurations

**Classic Build (Default)**:
- No hardware binding
- Community-friendly
- Open-source license
```bash
dotnet build -c Release
```

**Store Build (Hardware-Bound)**:
- Hardware-bound Pro licenses
- Machine ID validation
- Define STORE_VERSION compiler flag
```bash
dotnet build -c Release /p:DefineConstants=STORE_VERSION
```

---

## Testing

### Test Suite Overview

**42 comprehensive unit tests** covering all advertising and licensing logic:

```
SyncMedia.Tests/
├── Core/
│   ├── Models/
│   │   └── LicenseInfoTests.cs          (11 tests)
│   └── Services/
│       ├── LicenseManagerTests.cs        (14 tests)
│       └── FeatureFlagServiceTests.cs    (17 tests)
```

### Coverage

**LicenseInfoTests** (11 tests):
- Speed boost activation/expiration
- 30-day period reset
- License validation
- Property calculations

**LicenseManagerTests** (14 tests):
- File counter incrementation
- Counter reset mechanism (watch ad, click ad)
- License activation/deactivation
- Hardware binding validation
- License key generation
- Persistence (save/load)

**FeatureFlagServiceTests** (17 tests):
- Progressive throttling formula
- Throttle at 0, 10, 20, 50, 100, 200, 500, 1000, 2000 files
- 10-second cap enforcement
- Speed boost bypass (0ms)
- Pro user bypass (always 0ms)
- Feature flag management

### Running Tests

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --verbosity detailed

# Run specific test suite
dotnet test --filter "FullyQualifiedName~LicenseInfoTests"
```

### Test Examples

**Progressive Throttling**:
```csharp
[Theory]
[InlineData(0, 0)]        // 0 files = 0ms
[InlineData(10, 100)]     // 10 files = 100ms
[InlineData(50, 500)]     // 50 files = 500ms
[InlineData(100, 1000)]   // 100 files = 1s
[InlineData(200, 2000)]   // 200 files = 2s
[InlineData(500, 5000)]   // 500 files = 5s
[InlineData(1000, 10000)] // 1000+ files = 10s (capped)
public void GetThrottleDelayMs_ReturnsCorrectDelay(int files, int expectedDelay)
{
    // Test validates progressive formula
}
```

**Counter Reset**:
```csharp
[Fact]
public void ResetFilesProcessedCounter_ResetsToZero()
{
    _licenseManager.IncrementFilesProcessed();
    _licenseManager.IncrementFilesProcessed();
    Assert.Equal(2, _licenseManager.CurrentLicense.FilesProcessedCount);
    
    _licenseManager.ResetFilesProcessedCounter();
    Assert.Equal(0, _licenseManager.CurrentLicense.FilesProcessedCount);
}
```

---

## License Protection

### Hardware Binding (Store Version Only)

**Purpose**: Prevent unauthorized redistribution on Microsoft Store while maintaining open-source community builds.

**How It Works**:
1. Pro license purchased through Microsoft Store
2. License key activated on user's machine
3. Machine ID generated from:
   - CPU ID
   - Motherboard serial
   - Machine name
   - SHA256 hashed for privacy
4. License bound to this machine ID
5. Every app launch validates machine ID match
6. Mismatch = license invalidated

**Store Build Configuration**:
```csharp
#if STORE_VERSION
    // Hardware binding enforced
    var machineId = GetMachineId();
    if (_currentLicense.MachineId != machineId)
    {
        // License invalid on this machine
        DeactivateLicense();
    }
#endif
```

**Classic Build**:
- No hardware binding
- Community can download, build, modify
- Open-source friendly
- No machine ID checks

**Benefits**:
- Protects business from store republishing
- Prevents license sharing across machines
- Maintains community goodwill
- Clear separation: Store = protected, Classic = open

### License File Security

**Features**:
- SHA256 signature verification
- Tamper detection on every load
- Automatic reinitialization if corrupted
- Encrypted storage (Windows Data Protection API)

**Location**: `%LOCALAPPDATA%\SyncMedia\license.json`

---

## Microsoft Store Deployment

### Pre-Deployment Checklist

**Ad Configuration**:
- [ ] Register app at https://apps.microsoft.com
- [ ] Create ad unit in Microsoft Partner Center
- [ ] Replace test IDs with production IDs in `MicrosoftAdvertisingService.cs`
- [ ] Test ads on Windows device

**Build Configuration**:
- [ ] Define STORE_VERSION for hardware binding
- [ ] Set version in `SyncMedia.WinUI.csproj`
- [ ] Update package identity in `Package.appxmanifest`
- [ ] Build in Release mode

**Testing**:
- [ ] Test free version with ads
- [ ] Test ad-blocker detection
- [ ] Test offline behavior
- [ ] Test Pro activation with hardware binding
- [ ] Test license on different machine (should fail)
- [ ] Test counter reset mechanism
- [ ] Test progressive throttling

**Documentation**:
- [ ] Update privacy policy (ads + hardware ID)
- [ ] Prepare store listing description
- [ ] Create screenshots (free + Pro)
- [ ] Write feature list

### Submission Steps

1. **Create MSIX Package**:
```bash
# Build with hardware binding
dotnet publish -c Release /p:DefineConstants=STORE_VERSION

# Package with Visual Studio or CLI
# See Package.appxmanifest for configuration
```

2. **Test Package**:
```powershell
# Install locally
Add-AppxPackage -Path ".\SyncMedia.msix"

# Test all features
# Uninstall
Remove-AppxPackage -Package "SyncMedia_..."
```

3. **Submit to Partner Center**:
- Login to https://partner.microsoft.com
- Create new app submission
- Upload MSIX package
- Configure store listing
- Set pricing (Pro in-app purchase)
- Submit for review

### Production Ad IDs

Update in `MicrosoftAdvertisingService.cs`:
```csharp
// BEFORE (test IDs):
private const string APPLICATION_ID = "test";
private const string AD_UNIT_ID = "test";

// AFTER (production IDs from Partner Center):
private const string APPLICATION_ID = "your-app-id";
private const string AD_UNIT_ID = "your-ad-unit-id";
```

---

## FAQ

### General

**Q: Is SyncMedia free?**  
A: Yes! The free version includes unlimited file processing with progressive throttling and ads. Upgrade to Pro for instant processing, offline access, and AI features.

**Q: What's progressive throttling?**  
A: Free version adds delay between files (0-10s) based on how many you've processed. Watch a video ad or click a banner to reset counter back to instant processing.

**Q: Can I use SyncMedia offline?**  
A: Pro users can work completely offline. Free users need internet for ad delivery.

### Technical

**Q: Where are my settings stored?**  
A: `%LOCALAPPDATA%\SyncMedia\settings.xml`

**Q: Where is the license file?**  
A: `%LOCALAPPDATA%\SyncMedia\license.json`

**Q: How does duplicate detection work?**  
A: Free version uses MD5 hashing. Pro version adds AI-powered perceptual detection to find similar images (crops, edits, filters).

**Q: Can I build from source?**  
A: Yes! Classic builds are open-source. Clone the repo and run `dotnet build`. Store builds use hardware-bound licenses.

### Advertising

**Q: What ads are shown?**  
A: Banner ads (728x90) at bottom of window + optional video ads for counter resets.

**Q: Can I skip video ads?**  
A: Yes, after 15 seconds. Full watch gives counter reset + 60min speed boost.

**Q: What if I use an ad blocker?**  
A: App will detect after 3 failures and pause. You'll need to disable blocker or upgrade to Pro.

**Q: How do I reset the throttle counter?**  
A: Watch a complete video ad or click a banner ad. Both reset counter to 0 for fast processing.

### Licensing

**Q: How do Pro licenses work?**  
A: Store version: Hardware-bound to your machine for security. Classic version: Traditional license key (no binding).

**Q: Can I use my Pro license on multiple computers?**  
A: Store version: No (hardware-bound). Classic version: Check license terms.

**Q: What happens if I reinstall Windows?**  
A: Store version: Contact support for license transfer. Classic version: Reactivate with same key.

---

## Support & Contributing

### Getting Help

- **Issues**: [GitHub Issues](https://github.com/JUUTC/SyncMedia/issues)
- **Discussions**: [GitHub Discussions](https://github.com/JUUTC/SyncMedia/discussions)
- **Email**: [Support Contact]

### Contributing

Contributions welcome! Please:
1. Fork the repository
2. Create feature branch
3. Make changes with tests
4. Submit pull request

### Roadmap

See [MODERNIZATION_ROADMAP.md](MODERNIZATION_ROADMAP.md) for future plans.

---

## License

This project is licensed under [LICENSE](LICENSE).

**Store Version**: Hardware-bound licenses for security.  
**Classic Version**: Open-source, community-friendly.

---

**Built with ❤️ for Windows 11 | Powered by WinUI 3 | Monetized by Microsoft Advertising**
