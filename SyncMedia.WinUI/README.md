# SyncMedia.WinUI - Modern WinUI 3 Application

This is the WinUI 3 version of SyncMedia, providing a modern, fluent design experience for Windows 10/11.

## Project Structure

```
SyncMedia.WinUI/
├── App.xaml                 # Application definition
├── App.xaml.cs             # Application startup and DI configuration
├── Views/                   # XAML views
│   ├── MainWindow.xaml     # Main application window
│   └── MainWindow.xaml.cs
├── ViewModels/              # MVVM ViewModels
│   └── MainViewModel.cs    # Main window ViewModel
├── Controls/                # Custom WinUI controls (to be added)
├── Services/                # UI-specific services (to be added)
└── Assets/                  # Images, icons, resources

```

## Features

- **Modern Fluent Design**: Uses Windows 11 design language
- **MVVM Pattern**: Clean separation with CommunityToolkit.Mvvm
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Shared Business Logic**: References SyncMedia.Core for all business logic
- **NavigationView**: Modern navigation with icons

## Architecture

- **UI Layer**: WinUI 3 with XAML
- **ViewModel Layer**: CommunityToolkit.Mvvm ObservableObject
- **Business Logic**: SyncMedia.Core (shared with Windows Forms)

## Dependencies

- Microsoft.WindowsAppSDK 1.5
- CommunityToolkit.Mvvm 8.2.2
- CommunityToolkit.WinUI.UI.Controls 7.1.2
- Microsoft.Extensions.DependencyInjection 8.0.0
- SyncMedia.Core (project reference)

## Building

Requires:
- .NET 9.0 SDK
- Windows App SDK 1.5
- Windows 10 SDK (10.0.19041.0 or later)

```bash
dotnet restore
dotnet build
dotnet run
```

## Status

🚧 **Phase 3 Week 1 - In Progress**

- ✅ Project structure created
- ✅ MVVM infrastructure set up
- ✅ Dependency injection configured
- ✅ Main window skeleton created
- 🚧 UI migration from Windows Forms (Week 2-3)
- 📋 Feature implementation (Week 3-4)

