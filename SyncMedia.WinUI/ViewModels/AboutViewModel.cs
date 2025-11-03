using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.IO;
using System.Reflection;
using Windows.System;

namespace SyncMedia.WinUI.ViewModels;

public partial class AboutViewModel : ObservableObject
{
    [ObservableProperty]
    private string appVersion;

    public AboutViewModel()
    {
        // Get version from assembly
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        AppVersion = $"Version {version.Major}.{version.Minor}.{version.Build}";
    }

    [RelayCommand]
    private async void ViewLicenses()
    {
        try
        {
            // Try to open the licenses folder
            var licensePath = Path.Combine(AppContext.BaseDirectory, "Licenses");
            
            if (Directory.Exists(licensePath))
            {
                await Launcher.LaunchFolderPathAsync(licensePath);
            }
            else
            {
                // Fallback: show message
                var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
                {
                    Title = "License Files",
                    Content = "License files are included in the application installation directory.\n\nPath: " + licensePath,
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };
                
                await dialog.ShowAsync();
            }
        }
        catch (Exception ex)
        {
            // Show error dialog
            var dialog = new Microsoft.UI.Xaml.Controls.ContentDialog
            {
                Title = "Error",
                Content = $"Could not open licenses folder: {ex.Message}",
                CloseButtonText = "OK",
                XamlRoot = App.MainWindow.Content.XamlRoot
            };
            
            await dialog.ShowAsync();
        }
    }
}
