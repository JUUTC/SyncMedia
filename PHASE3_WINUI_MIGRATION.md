# Phase 3: WinUI 3 Migration Guide

## Overview

This document outlines the step-by-step migration from Windows Forms to WinUI 3, maintaining all existing functionality while modernizing the UI with Fluent Design.

## Migration Strategy

### Approach: Gradual In-Place Migration

We'll use a hybrid approach where we:
1. Create the new WinUI 3 project structure alongside the existing Windows Forms app
2. Migrate shared business logic to a common library
3. Port UI components incrementally
4. Test each component before moving to the next
5. Keep both versions running until WinUI 3 is feature-complete
6. Switch default to WinUI 3, mark Windows Forms as legacy

This allows us to:
- Test incrementally
- Maintain a working app throughout migration
- Roll back if issues arise
- Compare implementations side-by-side

## Project Structure

### Current Structure
```
SyncMedia/
├── SyncMedia/                  # Windows Forms app
│   ├── Constants/
│   ├── Helpers/
│   ├── Models/
│   ├── Services/
│   ├── Properties/
│   ├── SyncMedia.cs           # Main form
│   ├── SyncMedia.Designer.cs
│   └── Program.cs
├── SyncMedia.Package/          # MSIX packaging
└── SyncMedia.Tests/            # Unit tests
```

### Target Structure
```
SyncMedia/
├── SyncMedia.Core/             # NEW: Shared business logic
│   ├── Constants/
│   ├── Helpers/
│   ├── Models/
│   ├── Services/
│   └── Interfaces/
├── SyncMedia.WinForms/         # RENAMED: Legacy Windows Forms
│   ├── UI/
│   ├── Program.cs
│   └── SyncMedia.cs
├── SyncMedia.WinUI/            # NEW: Modern WinUI 3 app
│   ├── ViewModels/
│   ├── Views/
│   ├── Controls/
│   ├── App.xaml
│   ├── App.xaml.cs
│   └── MainWindow.xaml
├── SyncMedia.Package/          # Updated for WinUI 3
└── SyncMedia.Tests/            # Expanded tests
```

## Step-by-Step Migration Plan

### Phase 3.1: Project Setup (Week 1)

#### Task 1: Create Shared Core Library
- [x] Create `SyncMedia.Core` class library project (.NET 9.0)
- [ ] Move business logic from SyncMedia to SyncMedia.Core:
  - [ ] Constants/MediaConstants.cs
  - [ ] Models/ (all model classes)
  - [ ] Services/GamificationService.cs
  - [ ] Helpers/FileHelper.cs
  - [ ] Helpers/GamificationPersistence.cs
  - [ ] Helpers/FilePreviewHelper.cs (adapt for WinUI 3)
  - [ ] XmlData.cs
- [ ] Add necessary NuGet packages to Core
- [ ] Update namespaces
- [ ] Test compilation

#### Task 2: Create WinUI 3 Project
- [ ] Create new WinUI 3 Desktop app project
- [ ] Configure project settings:
  - Target: .NET 9.0
  - Windows SDK: 10.0.19041.0
  - Platform: x64, ARM64
- [ ] Add reference to SyncMedia.Core
- [ ] Install required NuGet packages:
  - Microsoft.WindowsAppSDK
  - CommunityToolkit.Mvvm
  - CommunityToolkit.WinUI.UI.Controls
  - Microsoft.Extensions.DependencyInjection
- [ ] Set up dependency injection container
- [ ] Configure app theme and resources

#### Task 3: Update Solution Structure
- [ ] Rename existing SyncMedia project to SyncMedia.WinForms
- [ ] Add SyncMedia.Core to solution
- [ ] Add SyncMedia.WinUI to solution
- [ ] Update SyncMedia.Package to package WinUI 3 app
- [ ] Update project references
- [ ] Test solution builds

### Phase 3.2: MVVM Infrastructure (Week 1)

#### Set Up MVVM Pattern
- [ ] Create base ViewModel class with INotifyPropertyChanged
- [ ] Create RelayCommand implementation (or use CommunityToolkit)
- [ ] Set up navigation service
- [ ] Create ViewModelLocator
- [ ] Set up dependency injection for ViewModels

#### Create Core ViewModels
- [ ] MainViewModel
- [ ] FolderConfigurationViewModel
- [ ] SyncOperationViewModel
- [ ] SettingsViewModel
- [ ] GamificationViewModel
- [ ] PreviewViewModel

### Phase 3.3: UI Component Migration (Weeks 2-3)

#### Main Window Layout
- [ ] Create MainWindow.xaml with NavigationView
- [ ] Implement hamburger menu structure:
  - Home (main sync interface)
  - Settings
  - Achievements/Gamification
  - About
- [ ] Add acrylic background
- [ ] Implement title bar customization
- [ ] Add window state management

#### Home/Sync Page
- [ ] Create SyncPage.xaml
- [ ] Port folder selection UI:
  - Source folder picker (with validation)
  - Destination folder picker (with validation)
  - Reject folder picker (with validation)
- [ ] Port file type filters:
  - Image types checkbox with modern toggle
  - Video types checkbox with modern toggle
- [ ] Port naming list:
  - CheckedListBox → ListView with checkboxes
  - "Update Naming List" button
- [ ] Port sync controls:
  - "Sync Media" button (accent style)
  - Pause button
  - Cancel button
- [ ] Port progress UI:
  - ProgressBar → ProgressRing/ProgressBar
  - Status labels with modern typography
  - Statistics panel with cards

#### Preview Panel Integration
- [ ] Create PreviewControl.xaml (user control)
- [ ] Implement image preview:
  - Use Image control with BitmapImage
  - Implement 3-second timer
  - Smooth transitions
- [ ] Implement video preview:
  - Use MediaPlayerElement
  - 10-second playback with loop
  - Muted playback
- [ ] Add preview toggle switch
- [ ] Position in adaptive layout

#### File Results Grid
- [ ] Replace DataGridView with DataGrid or ListView
- [ ] Implement search/filter functionality
- [ ] Add column sorting
- [ ] Implement item templates for visual appeal
- [ ] Add context menu for file operations

### Phase 3.4: Feature Parity (Week 3)

#### Core Functionality
- [ ] Folder selection and validation
- [ ] File enumeration and filtering
- [ ] Hashing and duplicate detection
- [ ] File copying and organization
- [ ] Error handling and logging
- [ ] Settings persistence
- [ ] Gamification system
- [ ] Achievement notifications

#### Pro Features Integration (Prepared)
- [ ] Add feature flag UI in Settings
- [ ] Create placeholder for Advanced Duplicate Detection
- [ ] Design UI for similarity threshold slider
- [ ] Design UI for detection method selection
- [ ] Add GPU status indicator

### Phase 3.5: Modern UX Enhancements (Week 4)

#### Fluent Design System
- [ ] Implement acrylic materials
- [ ] Add reveal highlight effects
- [ ] Implement smooth animations:
  - Page transitions
  - Button hover states
  - Progress indicators
- [ ] Add shadows and depth
- [ ] Implement responsive layouts

#### Accessibility
- [ ] Keyboard navigation
- [ ] Screen reader support
- [ ] High contrast theme support
- [ ] Focus indicators
- [ ] Accessible labels and descriptions

#### Touch Support
- [ ] Increase touch target sizes
- [ ] Implement touch gestures
- [ ] Add touch-friendly spacing
- [ ] Test on touch devices

### Phase 3.6: Testing & Polish (Week 4)

#### Testing
- [ ] Unit tests for ViewModels
- [ ] Integration tests for sync operations
- [ ] UI automation tests
- [ ] Performance testing
- [ ] Memory leak testing
- [ ] Accessibility testing

#### Polish
- [ ] Animations tuning
- [ ] Performance optimization
- [ ] Bug fixes
- [ ] Documentation updates
- [ ] User guide creation

## Technical Details

### WinUI 3 Project Configuration

**SyncMedia.WinUI.csproj:**
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net9.0-windows10.0.19041.0</TargetFramework>
    <TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>
    <RootNamespace>SyncMedia.WinUI</RootNamespace>
    <ApplicationManifest>app.manifest</ApplicationManifest>
    <Platforms>x64;ARM64</Platforms>
    <RuntimeIdentifiers>win-x64;win-arm64</RuntimeIdentifiers>
    <PublishProfile>win-$(Platform).pubxml</PublishProfile>
    <UseWinUI>true</UseWinUI>
    <EnableMsixTooling>true</EnableMsixTooling>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.WindowsAppSDK" Version="1.5.0" />
    <PackageReference Include="Microsoft.Windows.SDK.BuildTools" Version="10.0.22621.0" />
    <PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
    <PackageReference Include="CommunityToolkit.WinUI.UI.Controls" Version="7.1.2" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\SyncMedia.Core\SyncMedia.Core.csproj" />
  </ItemGroup>
</Project>
```

### MVVM Pattern Implementation

**BaseViewModel.cs:**
```csharp
using CommunityToolkit.Mvvm.ComponentModel;

namespace SyncMedia.WinUI.ViewModels
{
    public class BaseViewModel : ObservableObject
    {
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _title = string.Empty;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
```

**MainViewModel.cs:**
```csharp
using CommunityToolkit.Mvvm.Input;
using SyncMedia.Core.Services;
using SyncMedia.Core.Models;

namespace SyncMedia.WinUI.ViewModels
{
    public partial class MainViewModel : BaseViewModel
    {
        private readonly IGamificationService _gamificationService;
        private readonly IFileService _fileService;

        [ObservableProperty]
        private string _sourceFolder = string.Empty;

        [ObservableProperty]
        private string _destinationFolder = string.Empty;

        [ObservableProperty]
        private bool _isPreviewEnabled;

        [ObservableProperty]
        private int _syncProgress;

        public MainViewModel(
            IGamificationService gamificationService,
            IFileService fileService)
        {
            _gamificationService = gamificationService;
            _fileService = fileService;
            Title = "SyncMedia";
        }

        [RelayCommand]
        private async Task SelectSourceFolder()
        {
            // Folder picker logic
        }

        [RelayCommand(CanExecute = nameof(CanStartSync))]
        private async Task StartSync()
        {
            IsBusy = true;
            try
            {
                // Sync logic
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanStartSync() =>
            !string.IsNullOrEmpty(SourceFolder) &&
            !string.IsNullOrEmpty(DestinationFolder) &&
            !IsBusy;
    }
}
```

### UI Layout Examples

**MainWindow.xaml:**
```xaml
<Window
    x:Class="SyncMedia.WinUI.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:controls="using:CommunityToolkit.WinUI.UI.Controls">
    
    <Grid>
        <NavigationView
            x:Name="NavView"
            IsBackButtonVisible="Collapsed"
            IsSettingsVisible="True"
            PaneDisplayMode="Left"
            SelectionChanged="NavView_SelectionChanged">
            
            <NavigationView.MenuItems>
                <NavigationViewItem Icon="Home" Content="Sync" Tag="sync"/>
                <NavigationViewItem Icon="Trophy" Content="Achievements" Tag="achievements"/>
            </NavigationView.MenuItems>
            
            <Frame x:Name="ContentFrame"/>
        </NavigationView>
    </Grid>
</Window>
```

**SyncPage.xaml (partial):**
```xaml
<Page
    x:Class="SyncMedia.WinUI.Views.SyncPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:controls="using:CommunityToolkit.WinUI.UI.Controls">
    
    <Grid Padding="24" RowSpacing="16">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <!-- Folder Configuration -->
        <controls:SettingsExpander Grid.Row="0"
            Header="Folders"
            Description="Configure source, destination, and reject folders">
            
            <StackPanel Spacing="12">
                <!-- Source Folder -->
                <Grid ColumnSpacing="8">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition Width="100"/>
                        <ColumnDefinition Width="*"/>
                        <ColumnDefinition Width="Auto"/>
                    </Grid.ColumnDefinitions>
                    
                    <TextBlock Grid.Column="0" 
                        Text="Source:" 
                        VerticalAlignment="Center"/>
                    <TextBox Grid.Column="1" 
                        Text="{x:Bind ViewModel.SourceFolder, Mode=TwoWay}"
                        PlaceholderText="Select source folder..."/>
                    <Button Grid.Column="2" 
                        Content="Browse..."
                        Command="{x:Bind ViewModel.SelectSourceFolderCommand}"/>
                </Grid>

                <!-- Similar for Destination and Reject folders -->
            </StackPanel>
        </controls:SettingsExpander>

        <!-- File Type Filters -->
        <controls:SettingsExpander Grid.Row="1"
            Header="File Types"
            Description="Select which media types to sync">
            
            <StackPanel Orientation="Horizontal" Spacing="16">
                <CheckBox Content="Images" 
                    IsChecked="{x:Bind ViewModel.FilterImages, Mode=TwoWay}"/>
                <CheckBox Content="Videos" 
                    IsChecked="{x:Bind ViewModel.FilterVideos, Mode=TwoWay}"/>
            </StackPanel>
        </controls:SettingsExpander>

        <!-- Preview Panel & Results Grid -->
        <Grid Grid.Row="2" ColumnSpacing="16">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="200"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>

            <!-- Preview -->
            <Border Grid.Column="0" 
                Background="{ThemeResource CardBackgroundFillColorDefaultBrush}"
                CornerRadius="8"
                Padding="12">
                <local:PreviewControl/>
            </Border>

            <!-- Results -->
            <ListView Grid.Column="1"
                ItemsSource="{x:Bind ViewModel.Results}"
                SelectionMode="Extended"/>
        </Grid>

        <!-- Sync Controls -->
        <CommandBar Grid.Row="3" DefaultLabelPosition="Right">
            <AppBarButton Icon="Play" Label="Sync Media" 
                Command="{x:Bind ViewModel.StartSyncCommand}"/>
            <AppBarButton Icon="Pause" Label="Pause" 
                Command="{x:Bind ViewModel.PauseSyncCommand}"/>
            <AppBarButton Icon="Cancel" Label="Cancel" 
                Command="{x:Bind ViewModel.CancelSyncCommand}"/>
        </CommandBar>
    </Grid>
</Page>
```

## Migration Checklist

### Pre-Migration
- [x] Complete Phase 1 (MSIX Packaging)
- [x] Complete Phase 2 (File Preview)
- [x] Design Phase 4 (Pro Features)
- [ ] Back up current codebase
- [ ] Create feature branch for WinUI 3 migration

### Core Migration
- [ ] Create SyncMedia.Core library
- [ ] Create SyncMedia.WinUI project
- [ ] Set up MVVM infrastructure
- [ ] Migrate main UI
- [ ] Migrate all features
- [ ] Achieve feature parity

### Testing & Quality
- [ ] All existing features work in WinUI 3
- [ ] Performance meets or exceeds Windows Forms version
- [ ] No memory leaks
- [ ] Accessibility compliance
- [ ] Touch functionality verified

### Deployment
- [ ] Update MSIX package to use WinUI 3
- [ ] Test installation and upgrades
- [ ] Verify settings migration
- [ ] Update documentation
- [ ] Release to beta testers

## Risk Mitigation

### Potential Issues

1. **Learning Curve**: Team may be unfamiliar with WinUI 3
   - **Mitigation**: Start with simple components, use Microsoft docs extensively

2. **Performance**: WinUI 3 may have different performance characteristics
   - **Mitigation**: Benchmark early and often, optimize hot paths

3. **Breaking Changes**: APIs may differ from Windows Forms
   - **Mitigation**: Abstract platform-specific code, use interfaces

4. **Third-party Dependencies**: Some may not support WinUI 3
   - **Mitigation**: Find alternatives early, consider creating wrappers

## Success Criteria

- ✅ All Windows Forms features work in WinUI 3
- ✅ Modern Fluent Design UI implemented
- ✅ Performance equal or better than Windows Forms
- ✅ No regressions in functionality
- ✅ Accessibility standards met
- ✅ Touch support fully functional
- ✅ Settings migrate seamlessly
- ✅ Ready for Pro feature integration (Phase 4)

## Timeline

- **Week 1**: Project setup, Core library, MVVM infrastructure
- **Week 2**: Main UI migration, folder configuration, filters
- **Week 3**: Sync operation, progress tracking, results display
- **Week 4**: Preview panel, gamification, testing, polish

**Total**: 4 weeks for Phase 3

After completion, ready to proceed with Phase 4 implementation (2-3 weeks).
