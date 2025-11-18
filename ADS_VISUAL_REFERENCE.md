# Microsoft Ads - Visual Layout Reference

## Overview
This document provides a visual reference for how Microsoft Ads are integrated into the SyncMedia WinUI application.

## Main Window Layout

```
┌─────────────────────────────────────────────────────────────────┐
│                     SyncMedia Window Title Bar                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────┬──────────────────────────────────────────────┐   │
│  │          │                                               │   │
│  │          │                                               │   │
│  │  Navigation    Content Area (NavigationView Frame)      │   │
│  │   Menu   │                                               │   │
│  │          │     - Home Page                               │   │
│  │  • Home  │     - Folder Configuration                    │   │
│  │  ------  │     - File Types                              │   │
│  │  • Folders     - Naming List                             │   │
│  │  • File Types  - Sync Page                               │   │
│  │  • Naming      - Files Page                              │   │
│  │  ------  │     - Statistics                              │   │
│  │  • Sync  │                                               │   │
│  │  • Files │                                               │   │
│  │  ------  │                                               │   │
│  │  • Stats │                                               │   │
│  │          │                                               │   │
│  └──────────┴──────────────────────────────────────────────┘   │
│                                                                   │
├───────────────────────────────────────────────────────────────────┤
│                    FREE VERSION - AD BANNER                      │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │                                                           │   │
│  │          [      728 x 90 Microsoft Ad Banner      ]      │   │
│  │                                                           │   │
│  └─────────────────────────────────────────────────────────┘   │
├───────────────────────────────────────────────────────────────────┤
└───────────────────────────────────────────────────────────────────┘
```

## Free Version (with Ads)

### Window Structure
```
Grid (rows: *, Auto)
├── Row 0: NavigationView (takes all available space)
│   └── Content: Dynamic pages
└── Row 1: Ad Banner (fixed height)
    └── Border (with AdControl)
        ├── Background: Card background
        ├── Border: 1px top border
        ├── Padding: 8px
        └── AdControl (728x90)
```

### Ad Banner Details
- **Size**: 728 pixels wide × 90 pixels high
- **Position**: Bottom of window, horizontally centered
- **Background**: Theme-aware card background
- **Border**: 1px top border with card stroke color
- **Padding**: 8px around the ad
- **Visibility**: Visible (default)

## Pro Version (without Ads)

```
┌─────────────────────────────────────────────────────────────────┐
│                     SyncMedia Window Title Bar                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  ┌──────────┬──────────────────────────────────────────────┐   │
│  │          │                                               │   │
│  │          │                                               │   │
│  │  Navigation    Content Area (NavigationView Frame)      │   │
│  │   Menu   │                                               │   │
│  │          │     Full Height - No Ads!                     │   │
│  │  • Home  │                                               │   │
│  │  ------  │                                               │   │
│  │  • Folders                                               │   │
│  │  • File Types                                            │   │
│  │  • Naming │                                              │   │
│  │  ------  │                                               │   │
│  │  • Sync  │                                               │   │
│  │  • Files │                                               │   │
│  │  ------  │                                               │   │
│  │  • Stats │                                               │   │
│  │          │                                               │   │
│  │          │                                               │   │
│  └──────────┴──────────────────────────────────────────────┘   │
│                                                                   │
└───────────────────────────────────────────────────────────────────┘
```

### Pro Version Changes
- **Ad Banner**: Collapsed (Visibility.Collapsed)
- **Content Area**: Takes full height of window
- **No Space Reserved**: Ad container completely hidden

## Responsive Behavior

### Window Sizes

#### Minimum Size (800x600)
```
┌────────────────────────────────┐
│      SyncMedia (Minimum)       │
├────────────────────────────────┤
│                                │
│  ┌────┬─────────────────┐     │
│  │Nav │   Content       │     │
│  │    │                 │     │
│  └────┴─────────────────┘     │
├────────────────────────────────┤
│      [   Ad Banner   ]         │  ← Still centered
└────────────────────────────────┘
```

#### Large Size (1920x1080)
```
┌───────────────────────────────────────────────────────────┐
│                    SyncMedia (Large)                       │
├───────────────────────────────────────────────────────────┤
│                                                             │
│  ┌─────────┬────────────────────────────────────┐         │
│  │         │                                     │         │
│  │   Nav   │         Content Area                │         │
│  │         │                                     │         │
│  └─────────┴────────────────────────────────────┘         │
│                                                             │
├───────────────────────────────────────────────────────────┤
│              [      Ad Banner (Centered)      ]            │
└───────────────────────────────────────────────────────────┘
```

## State Transitions

### Free User → Upgrade to Pro

**Before (Free)**
```
┌─────────────────────────┐
│  Content Area           │
│                         │
│  [Settings Page]        │
│  License: Free          │
│  [Upgrade to Pro]  ←──── Click!
└─────────────────────────┘
├─────────────────────────┤
│   [  Ad Banner  ]       │  ← Visible
└─────────────────────────┘
```

**After (Pro)**
```
┌─────────────────────────┐
│  Content Area           │
│                         │
│  [Settings Page]        │
│  License: Pro           │
│  Pro features unlocked! │
│                         │
└─────────────────────────┘
                             ← Ad banner hidden immediately
```

### Trial Period

**During Trial (Days 1-14)**
```
Settings Page shows:
- Trial Days Remaining: X
- Ad Banner: Hidden
- Pro Features: Enabled
```

**After Trial Expires (Day 15+)**
```
Settings Page shows:
- Trial Expired
- Ad Banner: Visible (reappears)
- Pro Features: Disabled
```

## XAML Structure

```xml
<Window>
    <Grid>
        <!-- Row 0: Main Content -->
        <NavigationView Grid.Row="0">
            <Frame x:Name="ContentFrame"/>
        </NavigationView>
        
        <!-- Row 1: Ad Banner -->
        <Border Grid.Row="1" 
                x:Name="AdBorder"
                Visibility="Collapsed">  <!-- Default collapsed -->
            <advertising:AdControl 
                x:Name="AdControlBanner"
                Width="728"
                Height="90"/>
        </Border>
    </Grid>
</Window>
```

## Color Schemes

### Light Theme
- **Ad Border**: Light gray (#E4E4E4)
- **Ad Background**: White (#FFFFFF)
- **Border Top**: 1px solid light gray

### Dark Theme
- **Ad Border**: Dark gray (#2C2C2C)
- **Ad Background**: Dark card (#1E1E1E)
- **Border Top**: 1px solid dark gray

## Accessibility

### Screen Reader Announcements
```
Free Version Launch:
"SyncMedia window. Advertisement banner visible at bottom."

Upgrade to Pro:
"Advertisement banner hidden. Pro version activated."

Trial Started:
"Trial period started. Advertisement banner hidden for 14 days."
```

### Keyboard Navigation
- Tab order: Navigation → Content → (Ad Banner skipped when collapsed)
- Focus indicators: Standard WinUI 3 focus visuals
- No keyboard trap: Users can navigate past ads

## Performance Metrics

### Load Times
- **Ad Load**: 500ms - 2s (async, non-blocking)
- **Window Render**: No delay (ad loads separately)
- **Visibility Toggle**: <50ms (instant feel)

### Memory Usage
- **Free (with ads)**: +5-10 MB
- **Pro (no ads)**: 0 MB overhead
- **Ad Error**: 0 MB (control hidden)

## Testing Checklist

### Visual Tests
- [ ] Ad banner appears at bottom in free version
- [ ] Ad is centered horizontally
- [ ] Border and padding look correct
- [ ] Colors match theme (light/dark)
- [ ] No layout shift when ad loads
- [ ] Content area resizes properly
- [ ] Window minimum size respected

### Functional Tests
- [ ] Ad rotates/refreshes automatically
- [ ] "Upgrade to Pro" hides ad immediately
- [ ] Trial period hides ad
- [ ] Trial expiration shows ad
- [ ] Window resize doesn't break layout
- [ ] Theme change updates ad styling

### Edge Cases
- [ ] No internet: Ad area collapses gracefully
- [ ] Ad load error: No blank space shown
- [ ] Multiple upgrades: Ad stays hidden
- [ ] App restart: Ad state persists correctly

## Future Enhancements

### Responsive Ad Sizes
```
Window Width    Ad Size
< 800px         300×250 (medium rectangle)
800-1200px      728×90 (current)
> 1200px        970×90 (large banner)
```

### Multiple Placements
```
┌─────────────────────┐
│  [Square Ad]  Title │  ← Header banner
├─────────────────────┤
│  Content            │
├─────────────────────┤
│  [Banner Ad]        │  ← Bottom banner (current)
└─────────────────────┘
```

### Native Ads
```
[Statistics Page]
─────────────────
Stat 1: 1,234
Stat 2: 5,678
─────────────────
[ Sponsored ]     ← Native ad in content
─────────────────
Stat 3: 9,012
```

## References

- Microsoft Advertising SDK: https://learn.microsoft.com/en-us/windows/uwp/monetize/
- WinUI 3 Layout: https://learn.microsoft.com/en-us/windows/apps/design/layout/
- Ad Sizes: https://www.iab.com/guidelines/iab-display-advertising-guidelines/
