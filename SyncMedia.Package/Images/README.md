# Placeholder Images - Replace Before Publishing

These are minimal placeholder PNG files for the Windows Store package. **You must replace these with proper branded images before publishing to the Microsoft Store.**

## Required Images

| File | Size | Description |
|------|------|-------------|
| Square44x44Logo.png | 44×44 | App list icon |
| Square150x150Logo.png | 150×150 | Medium tile |
| Wide310x150Logo.png | 310×150 | Wide tile |
| SplashScreen.png | 620×300 | Splash screen |
| StoreLogo.png | 50×50 | Store listing |

## Design Guidelines

- Use PNG format with transparency
- Follow [Microsoft's asset guidelines](https://docs.microsoft.com/windows/apps/design/style/app-icons-and-logos)
- Maintain consistent branding across all sizes
- Test on both light and dark backgrounds
- Consider high-DPI displays (provide 1.25x, 1.5x, 2x, 4x versions if possible)

## Tools for Creating Images

- **Adobe Photoshop/Illustrator** - Professional design tools
- **Figma** - Free online design tool
- **Inkscape** - Free vector graphics editor
- **GIMP** - Free image editor
- **Asset Generator Tools** - Visual Studio includes asset generation wizard

## Quick Generation

Visual Studio can help generate assets:

1. Right-click `Package.appxmanifest` in Solution Explorer
2. Select "Open With" → "Package Manifest Designer"
3. Go to "Visual Assets" tab
4. Upload a single source image (ideally 400×400 or larger)
5. Click "Generate" to create all required sizes

## Current Placeholders

The current placeholder images are minimal 1×1 transparent PNGs duplicated for each required size. They will work for testing but should be replaced with proper branding for production release.
