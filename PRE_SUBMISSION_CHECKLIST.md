# Pre-Submission Checklist for Microsoft Store

Use this checklist before submitting SyncMedia to the Microsoft Store.

## 🎨 Visual Assets (Required)

- [ ] Replace `Square44x44Logo.png` with branded 44×44 icon
- [ ] Replace `Square150x150Logo.png` with branded 150×150 icon  
- [ ] Replace `Wide310x150Logo.png` with branded 310×150 icon
- [ ] Replace `SplashScreen.png` with branded 620×300 splash screen
- [ ] Replace `StoreLogo.png` with branded 50×50 store icon
- [ ] Verify all images have transparent backgrounds
- [ ] Test images on both light and dark backgrounds

## 📝 Package Manifest

- [ ] Open `SyncMedia.Package/Package.appxmanifest`
- [ ] Update `Identity.Name` with your package identity from Partner Center
- [ ] Update `Identity.Publisher` with your Publisher ID (starts with "CN=")
- [ ] Update `PublisherDisplayName` with your name or company
- [ ] Verify `Version` number (format: major.minor.build.revision)
- [ ] Review `Description` text

## 🏢 Microsoft Store Account

- [ ] Created developer account at https://partner.microsoft.com/dashboard
- [ ] Paid registration fee ($19 for individual)
- [ ] Account verified and active
- [ ] Reserved app name "SyncMedia" (or your chosen name)
- [ ] Obtained Publisher ID and Publisher Display Name
- [ ] Created or obtained signing certificate

## 🔨 Build & Test

- [ ] Opened solution in Visual Studio 2022 on Windows
- [ ] Successfully built SyncMedia project
- [ ] Successfully built SyncMedia.Package project
- [ ] Created MSIX package (Right-click SyncMedia.Package → Publish)
- [ ] Located .msix or .msixupload file in AppPackages folder

### Testing Checklist

- [ ] Installed MSIX package locally by double-clicking .msix file
- [ ] App installs without errors
- [ ] App launches successfully
- [ ] Configured source, destination, and reject folders
- [ ] Performed a media sync operation
- [ ] Verified files were processed correctly
- [ ] Closed and reopened app - settings persist
- [ ] Checked settings file exists: `%LOCALAPPDATA%\SyncMedia\settings.xml`
- [ ] Uninstalled app
- [ ] Reinstalled app
- [ ] Verified settings were restored
- [ ] No crashes or errors during testing

## 📸 Store Listing Materials

### Required
- [ ] At least 1 screenshot (1366×768 minimum)
- [ ] App description (use/adapt from README.md)
- [ ] Short description (maximum 200 characters)
- [ ] Category selected (Productivity or Utilities)
- [ ] Age rating determined

### Recommended
- [ ] 3-5 high-quality screenshots showing:
  - [ ] Main window with folder configuration
  - [ ] Sync operation in progress
  - [ ] Results/completion screen
- [ ] Feature list prepared
- [ ] Release notes written
- [ ] Support contact information (email or website)
- [ ] Privacy policy URL (if applicable)

## 📋 Store Submission Information

### Properties Section
- [ ] Category: Selected
- [ ] Subcategory: Selected (if applicable)
- [ ] Product declarations: Completed

### Pricing Section
- [ ] Pricing model: Free (or Paid with price set)
- [ ] Markets: Selected (or "All markets")

### Availability Section
- [ ] Release date: Configured
- [ ] Discoverability: Public (or other option selected)

### App Properties Section
- [ ] Age rating: Completed
- [ ] Does the app support pen and ink? (Probably "No")
- [ ] App uses file sharing? (Yes - for media sync)

## 🔒 Capabilities Verification

The manifest already includes necessary capabilities:
- [x] `runFullTrust` - Required for Windows Forms
- [x] `broadFileSystemAccess` - Required for media sync operations

## ✅ Final Checks

- [ ] No build errors in Visual Studio
- [ ] No runtime errors during testing
- [ ] All features work as expected
- [ ] Settings save and load correctly
- [ ] App works on both x64 and ARM64 (if testing on ARM64 device)
- [ ] MSIX package size is reasonable (< 200 MB recommended)
- [ ] All placeholder content replaced with real content
- [ ] Documentation reviewed and accurate
- [ ] Version number incremented if this is an update

## 🚀 Submission Process

1. [ ] Log into Partner Center
2. [ ] Navigate to your app
3. [ ] Start new submission
4. [ ] Upload .msixupload file from AppPackages folder
5. [ ] Complete all required sections:
   - [ ] Properties
   - [ ] Pricing and availability
   - [ ] App properties
   - [ ] Age ratings
   - [ ] Packages (uploaded)
   - [ ] Store listings (with screenshots and description)
6. [ ] Review submission
7. [ ] Submit for certification

## 📊 Post-Submission

- [ ] Monitor submission status in Partner Center
- [ ] Respond to any certification failures within 90 days
- [ ] Test app download from Store after approval
- [ ] Monitor user reviews and ratings
- [ ] Plan for future updates

## 📚 Reference Documents

- **Technical Migration**: See `WINDOWS_STORE_MIGRATION.md`
- **Quick Start Guide**: See `STORE_PUBLISHING_GUIDE.md`
- **Packaging Details**: See `SyncMedia.Package/README.md`
- **Visual Assets Help**: See `SyncMedia.Package/Images/README.md`

## 🆘 Troubleshooting

### Build Issues
- Ensure Windows 10 SDK (10.0.17763.0+) is installed
- Update Visual Studio to latest version
- Clean and rebuild solution

### Certification Failures
Common issues:
- Missing age rating
- Incomplete store listing
- App crashes on launch
- Missing required screenshots
- Privacy policy required but not provided

Check certification report in Partner Center for specific issues.

### Testing Issues
- Cannot install: Check Windows version (must be 10 1809+)
- Settings not persisting: Check folder permissions
- Features not working: Check app capabilities in manifest

---

**Estimated Time to Complete**: 2-4 hours (first time)

**Certification Time**: 1-3 business days after submission
