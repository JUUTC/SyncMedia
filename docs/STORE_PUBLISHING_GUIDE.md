# Quick Start: Publishing to Microsoft Store

This guide provides the essential steps to publish SyncMedia to the Microsoft Store after the migration.

## ✅ What's Already Done

The application has been fully migrated and is ready for Store packaging:

1. **MSIX Packaging Project Created** - `SyncMedia.Package/` contains all necessary files
2. **Settings Storage Updated** - Now uses `%LOCALAPPDATA%\SyncMedia\settings.xml`
3. **Manifest Configured** - Package.appxmanifest with required capabilities
4. **Multi-Architecture Support** - Supports both x64 and ARM64
5. **Documentation Complete** - Comprehensive guides included

## 🎨 Before Publishing - Customize Visual Assets

**IMPORTANT:** Replace the placeholder images in `SyncMedia.Package/Images/` with your branded assets.

### Quick Method (Recommended)
1. Open `SyncMedia.sln` in Visual Studio 2022
2. Double-click `Package.appxmanifest` in SyncMedia.Package project
3. Go to "Visual Assets" tab
4. Upload a single source image (recommend 400×400 or larger square PNG)
5. Click "Generate all assets"
6. Review and save

### Manual Method
Create PNG images with transparency:
- `Square44x44Logo.png` - 44×44 pixels
- `Square150x150Logo.png` - 150×150 pixels
- `Wide310x150Logo.png` - 310×150 pixels
- `SplashScreen.png` - 620×300 pixels
- `StoreLogo.png` - 50×50 pixels

## 📝 Microsoft Store Setup

### Step 1: Create Developer Account
1. Visit https://partner.microsoft.com/dashboard
2. Sign in with Microsoft account
3. Pay one-time registration fee ($19 for individual)
4. Complete account verification

### Step 2: Reserve App Name
1. In Partner Center, click "Create a new app"
2. Enter "SyncMedia" (or your preferred name)
3. Click "Reserve product name"
4. Save your **Publisher ID** and **Publisher Display Name**

### Step 3: Update Package Identity
Edit `SyncMedia.Package/Package.appxmanifest`:

```xml
<Identity Name="YourPublisherId.SyncMedia"
          Publisher="CN=YOUR-PUBLISHER-ID-HERE"
          Version="1.0.0.0" />

<Properties>
  <PublisherDisplayName>Your Name or Company</PublisherDisplayName>
  ...
</Properties>
```

Replace:
- `YourPublisherId.SyncMedia` with your full package identity from Partner Center
- `YOUR-PUBLISHER-ID-HERE` with your Publisher ID
- `Your Name or Company` with your Publisher Display Name

## 🔨 Building the Package

### Using Visual Studio (Easiest)

1. **Open Solution**
   ```
   Open SyncMedia.sln in Visual Studio 2022
   ```

2. **Create Package**
   ```
   Right-click SyncMedia.Package project
   → Publish
   → Create App Packages
   → Select "Microsoft Store using a new app name"
   → Choose your reserved app name
   → Configure version and architecture
   → Create package
   ```

3. **Output Location**
   ```
   SyncMedia.Package\AppPackages\SyncMedia_[version]_[architecture]_bundle\
   ```

### Using Command Line

```powershell
# From repository root on Windows
msbuild SyncMedia.Package\SyncMedia.Package.wapproj `
  /p:Configuration=Release `
  /p:Platform=x64 `
  /p:AppxBundle=Always `
  /p:UapAppxPackageBuildMode=StoreUpload
```

## 📤 Upload to Store

1. **Prepare Submission**
   - Go to Partner Center → Your App → Submission
   - Upload the `.msixupload` file from AppPackages folder
   
2. **Fill Required Information**
   - **Properties**: Select category (Productivity or Utilities)
   - **Pricing**: Free or Paid
   - **App properties**: Set age rating, categories
   - **Store listings**: 
     - Description (use content from README.md)
     - Screenshots (capture the app in use - at least 1 required)
     - App icon (use your Square150x150Logo.png)
   
3. **Submit for Certification**
   - Review all sections
   - Click "Submit to the Store"
   - Wait 1-3 business days for review

## 📸 Screenshots Needed

Capture these screenshots of the app:
1. Main window showing folder configuration
2. Sync in progress
3. Results/completion screen
4. (Optional) Settings or advanced features

Requirements:
- PNG or JPG format
- Minimum 1366×768 pixels
- At least 1 screenshot required
- Up to 10 screenshots allowed

## 🧪 Testing Before Submission

### Test Sideloading
1. Build the package in Visual Studio
2. Find the `.msix` file in `AppPackages/`
3. Double-click to install locally
4. Test all features:
   - Folder selection and saving
   - Media sync operation
   - Settings persistence (check `%LOCALAPPDATA%\SyncMedia\settings.xml`)

### Test Uninstall/Reinstall
1. Uninstall from Windows Settings
2. Verify settings remain in LocalApplicationData
3. Reinstall and verify settings are restored

## 📋 Certification Checklist

Before submitting, verify:
- [ ] Visual assets replaced with branded images
- [ ] Package.appxmanifest updated with your Publisher ID
- [ ] App builds without errors for x64 and ARM64
- [ ] App installs and runs correctly
- [ ] All features work in packaged mode
- [ ] Screenshots captured and ready
- [ ] Store description written
- [ ] Privacy policy URL prepared (if collecting any data)

## 🎉 After Publishing

Once approved:
1. App appears in Microsoft Store within 24 hours
2. Users can install via Store
3. Automatic updates delivered through Store
4. Monitor reviews and ratings in Partner Center
5. Release updates by creating new submissions

## 🔗 Helpful Resources

- [Partner Center Dashboard](https://partner.microsoft.com/dashboard)
- [Windows Store Policies](https://docs.microsoft.com/windows/apps/publish/store-policies)
- [App Submission Process](https://docs.microsoft.com/windows/apps/publish/publish-your-app/overview)
- [MSIX Documentation](https://docs.microsoft.com/windows/msix/)

## 🆘 Need Help?

- **Migration Details**: See `WINDOWS_STORE_MIGRATION.md`
- **Packaging Details**: See `SyncMedia.Package/README.md`
- **Visual Assets**: See `SyncMedia.Package/Images/README.md`
- **Microsoft Support**: Partner Center → Support

---

**Note**: This entire migration maintains backward compatibility. Users of the non-Store version can continue using it, and their settings will migrate automatically when switching to the Store version.
