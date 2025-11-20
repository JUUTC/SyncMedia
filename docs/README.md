# SyncMedia Documentation

## User Documentation

### [User Guide](USER_GUIDE.md)
Complete guide to using SyncMedia, including:
- Installation and setup
- Feature overview
- Free vs Pro comparison
- Advertising system
- License management
- Troubleshooting and FAQ

### [AI-Powered Features](AI_POWERED_MEDIA_FEATURES.md)
Guide to AI-powered duplicate detection:
- Feature overview
- Usage instructions
- Performance optimization
- GPU acceleration

### [Advanced Duplicate Detection](ADVANCED_DUPLICATE_DETECTION.md)
Technical details on duplicate detection algorithms:
- MD5 hashing (Free tier)
- Perceptual hashing (Pro tier)
- CNN-based detection
- Detection methods comparison

## Developer Documentation

### [Store Publishing Guide](STORE_PUBLISHING_GUIDE.md)
Guide for publishing to Microsoft Store:
- Package preparation
- Store submission process
- Certification requirements
- Update procedures

### [Windows Store Migration](WINDOWS_STORE_MIGRATION.md)
Technical migration documentation:
- Architecture overview
- WinUI 3 migration details
- MSIX packaging
- Build configuration

### [Pre-Submission Checklist](PRE_SUBMISSION_CHECKLIST.md)
Checklist before submitting to Store:
- Code quality checks
- Testing requirements
- Documentation review
- Compliance verification

## Python Integration

For Python/AI features, see:
- [`SyncMedia.Core/Python/README.md`](../SyncMedia.Core/Python/README.md) - Python setup and usage

## Project Structure

```
SyncMedia/
├── SyncMedia.Core/         # Business logic library
├── SyncMedia.WinUI/        # WinUI 3 UI application
├── SyncMedia.Package/      # MSIX packaging project
├── SyncMedia.Tests/        # Unit tests (210 tests, 49% coverage)
└── docs/                   # Documentation (you are here)
```

## Getting Started

1. Start with the [User Guide](USER_GUIDE.md) for usage instructions
2. Review [AI Features](AI_POWERED_MEDIA_FEATURES.md) for Pro features
3. For development, see the main [README](../README.md)
4. For Store publishing, see [Store Publishing Guide](STORE_PUBLISHING_GUIDE.md)

## Support

- Report issues on [GitHub Issues](https://github.com/JUUTC/SyncMedia/issues)
- Check the User Guide FAQ section for common questions
