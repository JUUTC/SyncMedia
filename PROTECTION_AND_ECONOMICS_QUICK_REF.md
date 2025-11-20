# License Protection & Economics Update - Quick Reference

## What Changed (Commit caaf7b1)

### 🔒 License Protection (NEW)

**Before**: Basic MD5 checksum validation  
**After**: Hardware-bound licenses with signature verification

#### Store Version Features
- ✅ Licenses tied to machine hardware (CPU ID + motherboard + machine name)
- ✅ SHA256 signature verification prevents tampering
- ✅ Prevents copying licenses to other computers
- ✅ Prevents unauthorized Microsoft Store republishing

#### Classic Version (Open Source)
- ❌ No hardware binding (community-friendly)
- ✅ Basic license validation still works
- ✅ Full functionality for contributors
- ✅ Compile flag: `STORE_VERSION` controls which mode

### 💰 Free Tier Economics (CHANGED)

| Metric | Before | After | Impact |
|--------|--------|-------|--------|
| **Base Files/30 Days** | 100 | **25** | More sustainable, creates engagement |
| **Video Ad Bonus** | +20 | **+50** | Strong incentive (1 video = 2x base!) |
| **Click Ad Bonus** | +10 | **+5** | Reduces fraud, focuses on videos |

### 📊 User Impact Examples

| User Type | Monthly Files | Engagement Level |
|-----------|--------------|------------------|
| Passive Free | 25 files | No ads watched |
| Watches 1 Video | 75 files | Low engagement |
| Watches 2 Videos | 125 files | **More than old model!** |
| Watches 3 Videos | 175 files | **75% more than old model!** |
| Pro User | ∞ Unlimited | No ads ever |

**Key Insight**: Engaged free users watching 2-3 videos get MORE value (125-175 files) than the old passive model (100 files).

### 🖥️ Screen Sleep Prevention (NEW)

**New Service**: `IDisplayRequestService`

**Purpose**: Keep screen on during:
- File processing operations
- Video ad playback  
- Long-running tasks

**Implementation**: 
```csharp
// Prevent screen sleep
_displayRequestService.RequestActive();

// Allow screen sleep
_displayRequestService.RequestRelease();
```

**Benefit**: Video ads won't be interrupted by screen timeout, better UX.

## Files Modified

### Core Layer
1. **LicenseInfo.cs** - Added `MachineId`, `IsStoreLicense`, updated constants
2. **LicenseManager.cs** - Added hardware binding, signature verification
3. **SyncMedia.Core.csproj** - Added System.Management package
4. **IDisplayRequestService.cs** - New interface (screen sleep prevention)

### WinUI Layer
5. **DisplayRequestService.cs** - WinUI implementation
6. **App.xaml.cs** - Registered DisplayRequestService

### Tests
7. **LicenseInfoTests.cs** - Updated for new constants (25/50/5)

### Documentation
8. **LICENSE_PROTECTION_AND_ECONOMICS.md** - Complete guide (400+ lines)

## Why These Changes?

### Protection Rationale

**Problem**: Someone could clone SyncMedia, rebuild it, and republish on Microsoft Store under a different name, stealing users and revenue.

**Solution**: Store-purchased licenses are hardware-bound. If someone tries to:
1. Copy the app → Still needs valid license
2. Copy license file → Machine ID won't match, license invalid
3. Rebuild from source → Gets classic version without hardware binding
4. Republish on store → Users need to buy again (can't use copied licenses)

**Result**: Protects business while keeping open-source spirit alive.

### Economics Rationale

**Problem**: 100 free files/month was too generous:
- Users had no incentive to watch ads
- Not sustainable for ad-supported model
- Minimal engagement with video ads
- Easy to "game" with banner clicks

**Solution**: Lower base (25), higher video reward (50), lower click reward (5)

**Benefits**:
- Creates real engagement with ad system
- Strong incentive for valuable video ad views
- Reduces click fraud risk
- Engaged users get MORE value than before
- Sustainable business model
- Clear upgrade path to Pro

### Screen Sleep Rationale

**Problem**: Video ads would be interrupted when screen sleeps:
- Poor user experience
- Incomplete ad views = no revenue
- Users frustrated with timeout during long operations

**Solution**: Use DisplayRequest API to prevent screen sleep during:
- Video ad playback
- File processing
- Any operation user is watching

**Benefits**:
- Better completion rates for video ads
- Users can watch progress without interaction
- Professional UX matching other media apps

## Migration Path

### Existing Free Users

**If processed < 25 files**: No impact, continue normally  
**If processed 25-100 files**: 
- Can watch 1 video ad to get +50 files (total 75 available)
- Can upgrade to Pro for unlimited
- Or wait for 30-day period reset

**If processed 100+ files**: Period will reset after 30 days, start fresh with 25 base

### Existing Pro Users

**No change**: Unlimited files, no ads, works offline

### New Users

- Start with 25 files
- Clear messaging: "Watch ad for 50 more files"
- Can earn up to 175 files/month with 3 video ads
- Or upgrade to Pro for unlimited

## Testing Checklist

### License Protection Testing
- [ ] Activate store license on Machine A → Works
- [ ] Copy license file to Machine B → Fails (machine ID mismatch)
- [ ] Edit license XML manually → Fails (signature invalid)
- [ ] Build classic version → Works without hardware binding
- [ ] Build store version → Enforces hardware binding

### Economics Testing
- [ ] New user starts with 25 files
- [ ] Watch video ad → Adds 50 files
- [ ] Click banner ad → Adds 5 files
- [ ] Reach 25 files limit → Shows "Watch ad" prompt
- [ ] 30 days pass → Counter resets to 0

### Screen Sleep Testing
- [ ] Start file processing → Screen stays on
- [ ] Play video ad → Screen stays on
- [ ] Complete operation → Screen sleep resumes
- [ ] Manually lock screen → Works normally

## Production Deployment

### Store Build
1. Enable `STORE_VERSION` compile flag
2. Verify hardware binding is active
3. Test license on multiple machines
4. Replace test ad IDs with production IDs
5. Submit to Microsoft Store

### Classic Build
1. Ensure `STORE_VERSION` is NOT defined
2. Verify no hardware binding
3. Publish source code as usual
4. Community can build and contribute

## Summary

**Protection**: ✅ Hardware-bound licenses prevent unauthorized distribution  
**Economics**: ✅ Fair model rewards engagement (25/50/5)  
**UX**: ✅ Screen stays on during operations  
**Balance**: ✅ Engaged free users get MORE value than before  
**Business**: ✅ Sustainable ad-supported model  
**Community**: ✅ Classic builds remain open-source friendly  

**Result**: Professional app protection + fair economics + excellent UX = sustainable business that respects both free and Pro users.
