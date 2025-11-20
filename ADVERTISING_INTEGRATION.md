# Microsoft Advertising Integration

This document describes the Microsoft Advertising integration in SyncMedia for the free version.

## Overview

SyncMedia uses the Microsoft Advertising SDK to display banner ads in the free version of the application. Users with a Pro license or active trial period do not see ads.

## Architecture

### Components

1. **IAdvertisingService** (`SyncMedia.Core/Interfaces/IAdvertisingService.cs`)
   - Platform-agnostic interface for advertising functionality
   - Allows for different advertising implementations across platforms

2. **MicrosoftAdvertisingService** (`SyncMedia.WinUI/Services/MicrosoftAdvertisingService.cs`)
   - WinUI 3 implementation of `IAdvertisingService`
   - Manages the `AdControl` from Microsoft Advertising SDK
   - Handles ad refresh, error handling, and visibility

3. **FeatureFlagService** (`SyncMedia.Core/Services/FeatureFlagService.cs`)
   - Determines whether ads should be shown based on license status
   - `ShouldShowAds` property returns `true` for free users not in trial

### Ad Placement

The ad banner is placed at the bottom of the main window (`MainWindow.xaml`):
- **Size**: 728x90 pixels (standard banner size)
- **Position**: Bottom of the window, centered horizontally
- **Visibility**: Controlled by license status via `FeatureFlagService`

### License Integration

Ads are shown/hidden based on the license status:
- **Free Version**: Ads are visible
- **Pro Version**: Ads are hidden (AdFree feature)
- **Trial Period (14 days)**: Ads are hidden during trial
- **Expired Trial**: Ads are shown unless user upgrades to Pro

## Configuration

### Test Mode

The implementation currently uses Microsoft Advertising test IDs:
- **Application ID**: `9nblggh5ggsx`
- **Ad Unit ID**: `test`

### Production Setup

To use real ads in production:

1. **Register your app** at https://apps.microsoft.com
2. **Create an ad unit** in the Microsoft Partner Center
3. **Update the IDs** in `MicrosoftAdvertisingService.cs`:
   ```csharp
   private const string APPLICATION_ID = "your-app-id";
   private const string AD_UNIT_ID = "your-ad-unit-id";
   ```

### Required Package

The integration requires the Microsoft Advertising SDK:
```xml
<PackageReference Include="Microsoft.Advertising.Xaml" Version="10.1811.1" />
```

## Usage

### Initialization

The advertising service is registered in the DI container in `App.xaml.cs`:
```csharp
services.AddSingleton<IAdvertisingService, MicrosoftAdvertisingService>();
```

The service is initialized in `MainWindow.xaml.cs`:
```csharp
private void InitializeAdvertising()
{
    if (_advertisingService is MicrosoftAdvertisingService msAdService)
    {
        msAdService.InitializeWithControl(AdControlBanner);
    }
    UpdateAdVisibility();
}
```

### Updating Ad Visibility

Ad visibility is updated when:
1. **App launches**: Based on current license status
2. **User upgrades to Pro**: Ads are hidden immediately
3. **Trial starts/expires**: Ads shown/hidden accordingly

To manually update ad visibility:
```csharp
_advertisingService.UpdateAdVisibility(_featureFlagService.ShouldShowAds);
```

## Event Handling

The service handles two ad events:

1. **AdRefreshed**: Called when ad successfully loads
   - Currently logs to debug output
   - Can be extended for analytics

2. **ErrorOccurred**: Called when ad fails to load
   - Logs error message and code
   - Hides the ad control to prevent showing empty space

## Testing

### Free Version
1. Launch the app with no license
2. Ad banner should be visible at the bottom
3. Test ads should rotate automatically

### Pro Version
1. Go to Settings
2. Click "Upgrade to Pro"
3. Ad banner should immediately disappear

### Trial Period
1. Fresh install has 14-day trial
2. During trial, ads should not be shown
3. After trial expires, ads should appear

## Privacy and Compliance

The Microsoft Advertising SDK complies with:
- **GDPR** (General Data Protection Regulation)
- **CCPA** (California Consumer Privacy Act)
- **COPPA** (Children's Online Privacy Protection Act)

No additional configuration is required as Microsoft handles privacy consent through the SDK.

## Performance Considerations

- Ads are loaded asynchronously to prevent UI blocking
- Ad control is hidden on error to maintain clean UI
- Ad refresh is handled automatically by the SDK
- Minimal memory footprint when ads are hidden

## Future Enhancements

Potential improvements for Phase 4.2:
- [ ] Multiple ad placements (e.g., interstitial ads)
- [ ] Ad frequency capping
- [ ] Analytics integration for ad performance tracking
- [ ] A/B testing different ad placements
- [ ] Custom ad sizes based on window size

## Troubleshooting

### Ads not showing
1. Check that `APPLICATION_ID` and `AD_UNIT_ID` are correct
2. Verify internet connection (ads require network)
3. Check `FeatureFlagService.ShouldShowAds` returns `true`
4. Look for error messages in debug output

### Ads showing for Pro users
1. Verify license activation in Settings
2. Check `LicenseManager.CurrentLicense.IsValid`
3. Ensure `FeatureFlagService.RefreshFeatureFlags()` was called after upgrade

## References

- [Microsoft Advertising SDK Documentation](https://learn.microsoft.com/en-us/windows/uwp/monetize/)
- [Windows App SDK](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/)
- [Microsoft Partner Center](https://partner.microsoft.com/)
