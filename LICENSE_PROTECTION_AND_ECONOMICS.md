# License Protection & Fair Monetization Model

## Overview

This document describes the enhanced license protection system and updated free tier economics implemented to protect SyncMedia from unauthorized distribution while maintaining a fair free tier.

## License Protection Enhancements

### 1. Hardware-Bound Licenses (Store Version)

**Purpose**: Prevent license key sharing and unauthorized app republishing on Microsoft Store.

**Implementation**:
- Machine ID generated from hardware identifiers (CPU ID, motherboard serial, machine name)
- Store-purchased licenses are bound to the activating machine
- License file includes machine ID and store license flag
- Validation checks machine ID match on every app launch

**Code Location**: `SyncMedia.Core/Services/LicenseManager.cs`

```csharp
// Machine ID generation using hardware identifiers
private string GetMachineId()
{
    // Combines CPU ID, motherboard serial, machine name
    // Hashed with SHA256 for consistent 32-char identifier
}

// License activation with hardware binding
public bool ActivateLicense(string licenseKey, bool isStoreLicense = false)
{
    if (isStoreLicense)
    {
        _currentLicense.MachineId = _machineId; // Bind to machine
    }
}
```

### 2. License File Integrity Protection

**Features**:
- XML signature verification using SHA256
- Tampering detection on every load
- Signature includes machine ID + encryption key + content

**Benefits**:
- Prevents manual editing of license files
- Detects file corruption or tampering
- Automatic reinitialization if integrity check fails

### 3. Classic vs Store Versions

| Feature | Classic (Open Source) | Store Version |
|---------|----------------------|---------------|
| License Validation | Basic checksum | Hardware-bound + signature |
| Machine Binding | ❌ No | ✅ Yes |
| Offline Activation | ✅ Yes | ✅ Yes (after initial activation) |
| License Sharing | Possible | Prevented |
| License Transfer | Free | Requires deactivation |

**Classic Version**: Community can build and use with basic license validation. No hardware binding, allowing flexibility for developers.

**Store Version**: Full protection with hardware binding prevents unauthorized republishing while still allowing legitimate single-user activation.

## Updated Free Tier Economics

### Previous Model (Too Generous)
- **100 files** per 30-day period
- **+20 files** per video ad watched
- **+10 files** per banner click

**Problems**:
- Users could process 100 files/month without watching ads
- Too generous for sustainable ad-supported model
- No incentive to watch video ads for many users
- Banner clicks too valuable (easier to game)

### New Model (Fair & Sustainable)

#### Base Limits
- **25 files** per 30-day period (was 100)
- **+50 files** per video ad watched (was +20)
- **+5 files** per banner click (was +10)

#### Rationale

**Lower Base Limit (25 files)**:
- Creates meaningful engagement with ad system
- Still useful for casual users (25 files = significant work)
- Encourages video ad watching
- Makes Pro upgrade more attractive
- Sustainable for ad-supported business model

**Higher Video Ad Reward (50 files)**:
- Strong incentive to watch video ads
- One video ad = 2x base monthly allowance
- Prioritizes valuable video ad impressions
- Rewards user engagement
- Can earn 100+ files/month with 2-3 video ads

**Lower Click Reward (5 files)**:
- Reduces incentive for click fraud
- Keeps clicks as secondary bonus
- Focuses monetization on video ads
- Still rewards genuine engagement

#### Economics Example

| Scenario | Files Available | Ad Engagement |
|----------|----------------|---------------|
| Passive Free User | 25/month | Minimal (sees banner ads) |
| Watches 1 Video Ad | 75/month (25+50) | 1 video/month |
| Watches 2 Video Ads | 125/month (25+100) | 2 videos/month |
| Watches 3 Video Ads | 175/month (25+150) | 3 videos/month |
| Clicks 5 Banners | 50/month (25+25) | Active clicking |
| Pro User | Unlimited | None (no ads) |

**Key Insight**: Engaged free users watching 2-3 video ads per month get more value (100-175 files) than old passive model (100 files), while passive users have incentive to engage or upgrade.

### Progressive Throttling (Unchanged)

Throttling remains the same to encourage speed boost engagement:
- **0-25 files**: 500ms delay per file
- **26-50 files**: 1000ms delay per file
- **51+ files**: 2000ms delay per file
- **Speed boost active**: 0ms delay (60 minutes from video ad)

## Screen Sleep Prevention

### Implementation

**Purpose**: Prevent screen from turning off during:
- File processing operations
- Video ad playback
- Active sync operations

**Interface**: `IDisplayRequestService`
```csharp
public interface IDisplayRequestService
{
    void RequestActive();    // Prevent sleep
    void RequestRelease();   // Allow sleep
    bool IsActive { get; }
}
```

**WinUI Implementation**: Uses `Windows.System.Display.DisplayRequest`
```csharp
public class DisplayRequestService : IDisplayRequestService
{
    private DisplayRequest _displayRequest;
    private int _requestCount; // Reference counting for nested calls
    
    public void RequestActive()
    {
        _displayRequest.RequestActive(); // Prevent screen sleep
    }
    
    public void RequestRelease()
    {
        _displayRequest.RequestRelease(); // Allow screen sleep
    }
}
```

**Usage Locations**:
1. **Sync Operations**: Active during file processing
2. **Video Ad Playback**: Active during video playback
3. **Large Operations**: Any operation >30 seconds

**Benefits**:
- Better user experience during long operations
- Video ads won't be interrupted by screen timeout
- Users can watch progress without touching mouse/keyboard

## Migration Strategy

### Existing Users

**Free Users**:
- Current `FilesProcessedCount` carries over
- If over new 25-file limit, they can:
  - Watch video ad to gain 50 more files immediately
  - Upgrade to Pro for unlimited files
  - Wait for 30-day period reset

**Example**: User has processed 80 files in current period
- New limit: 25 base (already exceeded)
- Solution: Watch 1 video ad → 75 files available (25+50)
- Already processed: 80 files
- Still need: Watch another video ad for 50 more

**Pro Users**:
- Unaffected by changes
- Unlimited files always
- No throttling
- Works offline

### New Users

- Start with 25 files
- Clear messaging about earning more through ads
- "Watch ad for 50 more files" button when limit reached
- Upgrade path always visible

## Store Submission Considerations

### Production Checklist

1. **License Protection**:
   - [ ] Hardware binding enabled for store builds
   - [ ] Compile flag: `STORE_VERSION`
   - [ ] Server validation endpoint configured (optional)

2. **Ad Configuration**:
   - [ ] Replace test ad IDs with production IDs
   - [ ] Microsoft Partner Center ad units created
   - [ ] Privacy policy updated with ad disclosure

3. **Build Configuration**:
   ```xml
   <PropertyGroup Condition="'$(Configuration)' == 'Store'">
       <DefineConstants>STORE_VERSION</DefineConstants>
   </PropertyGroup>
   ```

4. **Testing**:
   - [ ] Verify hardware binding prevents sharing
   - [ ] Test license activation on multiple machines
   - [ ] Validate tamper detection
   - [ ] Test file limit enforcement
   - [ ] Verify video ad rewards
   - [ ] Test screen sleep prevention

### Open Source Considerations

**Classic builds** should:
- Not include hardware binding (#ifdef STORE_VERSION)
- Use basic license validation
- Allow development/testing license keys
- Maintain full functionality for contributors

**Example**:
```csharp
#if STORE_VERSION
// Hardware-bound license for store
_currentLicense.MachineId = _machineId;
#else
// Classic open-source build - no hardware binding
_currentLicense.MachineId = null;
#endif
```

## Summary

### Protection Improvements
✅ Hardware-bound licenses prevent sharing  
✅ Signature verification prevents tampering  
✅ Machine ID validation prevents copying  
✅ Store version has full protection  
✅ Classic version remains open for community  

### Economics Improvements
✅ Lower base limit (25 files) creates engagement  
✅ Higher video rewards (50 files) incentivize valuable ads  
✅ Lower click rewards (5 files) reduce fraud risk  
✅ Total monthly potential (175+ files) higher for engaged users  
✅ Clear upgrade path for unlimited access  

### User Experience Improvements
✅ Screen sleep prevention during operations  
✅ Video ads don't timeout screen  
✅ Better progress visibility  
✅ Clear reward messaging  

### Business Benefits
✅ Prevents unauthorized store republishing  
✅ Sustainable ad-supported free tier  
✅ Strong incentive for Pro upgrades  
✅ Fair value for both free and Pro users  
✅ Higher video ad engagement  
✅ Lower click fraud risk  

The updated model creates a win-win: engaged free users can earn 100-175 files/month (more than old passive 100), while passive users have clear incentive to engage or upgrade. Pro users get unlimited value, and the business has sustainable economics.
