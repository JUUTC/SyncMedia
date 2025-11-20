# SyncMedia

[![Microsoft Store Ready](https://img.shields.io/badge/Microsoft%20Store-Ready-brightgreen)](https://github.com/JUUTC/SyncMedia)

**Intelligent media file synchronization with duplicate detection, AI-powered organization, and flexible monetization.**

---

## 🚀 Quick Start

SyncMedia copies pictures and videos from a source folder to an organized destination folder tree while preventing duplicates through intelligent hashing.

**Installation:**
- **Microsoft Store** (Recommended): Search "SyncMedia" → One-click install
- **Manual**: Download from [Releases](../../releases)

**Basic Setup:**
1. Set **Source** folder (where camera/phone downloads are)
2. Set **Destination** folder (where organized files go)
3. Set **Rejects** folder (where duplicates are moved)
4. Click "Sync Media" → Done!

---

## 📊 Free vs Pro

### Free Version
- ✅ **Unlimited files** (no hard caps!)
- ✅ MD5 duplicate detection
- ✅ Core sync functionality
- ⏱️ **Progressive throttling** (0-10s delays)
- 📺 Banner + video ads
- 🌐 Internet required
- 🎯 **Watch ad to reset counter** → instant processing!

### Pro Version
- ✅ **Zero throttling** (always instant)
- ✅ AI-powered duplicate detection
- ✅ GPU acceleration (10-100x faster)
- ✅ Offline mode
- ❌ No ads

**Progressive Throttling:**
```
0-50 files:   0-500ms    ⚡ Fast
100 files:    1000ms     🟡 Moderate  
200 files:    2000ms     🟠 Slow
500+ files:   5-10s      🔴 Very slow

Watch ad → Reset to 0ms ⚡
```

---

## 📖 Documentation

- **[User Guide](docs/USER_GUIDE.md)** - Complete feature documentation and usage instructions
- **[AI Features](docs/AI_POWERED_MEDIA_FEATURES.md)** - AI-powered duplicate detection guide
- **[Advanced Duplicate Detection](docs/ADVANCED_DUPLICATE_DETECTION.md)** - Technical details on duplicate detection
- **[Store Publishing](docs/STORE_PUBLISHING_GUIDE.md)** - Microsoft Store submission guide
- **[Migration Guide](docs/WINDOWS_STORE_MIGRATION.md)** - Technical migration documentation


## 🛠️ Development

### Requirements
- .NET 9.0 SDK
- Windows 10 SDK (10.0.17763.0+)

### Build
```bash
git clone https://github.com/JUUTC/SyncMedia.git
cd SyncMedia
dotnet restore
dotnet build -c Release
dotnet run --project SyncMedia.WinUI/SyncMedia.WinUI.csproj
```

### Testing
```bash
dotnet test  # Run 42 unit tests
```

**Test Coverage:**
- ✅ 11 LicenseInfo tests
- ✅ 14 LicenseManager tests
- ✅ 17 FeatureFlagService tests
- ✅ Progressive throttling formula
- ✅ Counter reset mechanism
- ✅ Hardware binding validation

---

## 🏗️ Core Library Features

SyncMedia.Core provides a robust, production-ready foundation:

- **Error Handling**: Centralized error management with `IErrorHandler` interface
- **Logging**: Structured logging using Microsoft.Extensions.Logging
- **Input Validation**: Secure path validation with `PathValidator` to prevent path traversal attacks
- **Testing**: Comprehensive test suite with 210+ tests and 49%+ code coverage

### Code Quality

- ✅ Exception handling in all async methods
- ✅ Centralized error management
- ✅ Input validation on all file paths
- ✅ Structured logging for diagnostics
- ✅ Unit tests for business logic
- ✅ Cross-platform compatibility

## 🧪 Testing

Run the test suite:

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

---

## 📄 License

See [LICENSE](LICENSE) file.

**Settings Storage:** `%LOCALAPPDATA%\SyncMedia\settings.xml`  
**License Storage:** `%LOCALAPPDATA%\SyncMedia\license.json`
