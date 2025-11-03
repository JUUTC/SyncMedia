using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SyncMedia.Core;
using SyncMedia.Core.Services;
using System;

namespace SyncMedia.WinUI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly LicenseManager _licenseManager;

    [ObservableProperty]
    private bool proVersion = false;

    [ObservableProperty]
    private bool filePreviewEnabled = true;

    [ObservableProperty]
    private bool performanceOptimizationsEnabled = false;

    [ObservableProperty]
    private bool advancedDuplicateDetectionEnabled = false;

    [ObservableProperty]
    private int similarityThreshold = 70;

    [ObservableProperty]
    private bool gpuAccelerationEnabled = false;

    [ObservableProperty]
    private string selectedDetectionMethod = "PHash";

    [ObservableProperty]
    private ElementTheme selectedTheme = ElementTheme.Default;

    [ObservableProperty]
    private string appVersion = "1.0.0";

    [ObservableProperty]
    private string licenseType = "Free";

    [ObservableProperty]
    private string licenseKey = "";

    [ObservableProperty]
    private int trialDaysRemaining = 0;

    [ObservableProperty]
    private bool isInTrial = false;

    [ObservableProperty]
    private string activationDate = "";

    public SettingsViewModel()
    {
        _licenseManager = new LicenseManager();
        LoadSettings();
    }

    private void LoadSettings()
    {
        var data = new XmlData();
        
        // General settings
        FilePreviewEnabled = bool.Parse(data.ReadValue("FilePreviewEnabled") ?? "true");
        PerformanceOptimizationsEnabled = bool.Parse(data.ReadValue("PerformanceOptimizationsEnabled") ?? "false");
        
        // Load license information
        var license = _licenseManager.CurrentLicense;
        ProVersion = license.IsPro;
        IsInTrial = license.IsInTrial;
        TrialDaysRemaining = license.TrialDaysRemaining;
        ActivationDate = license.ActivationDate?.ToString("d") ?? "";
        
        // Pro features
        AdvancedDuplicateDetectionEnabled = bool.Parse(data.ReadValue("AdvancedDuplicateDetectionEnabled") ?? "false");
        SimilarityThreshold = int.Parse(data.ReadValue("SimilarityThreshold") ?? "70");
        GpuAccelerationEnabled = bool.Parse(data.ReadValue("GpuAccelerationEnabled") ?? "false");
        SelectedDetectionMethod = data.ReadValue("DetectionMethod") ?? "PHash";
        
        // Theme
        var theme = data.ReadValue("AppTheme") ?? "Default";
        SelectedTheme = theme switch
        {
            "Light" => ElementTheme.Light,
            "Dark" => ElementTheme.Dark,
            _ => ElementTheme.Default
        };

        // Update license type display
        if (ProVersion)
        {
            LicenseType = "Pro";
        }
        else if (IsInTrial)
        {
            LicenseType = $"Free (Trial: {TrialDaysRemaining} days left)";
        }
        else
        {
            LicenseType = "Free";
        }
    }

    partial void OnFilePreviewEnabledChanged(bool value)
    {
        SaveSetting("FilePreviewEnabled", value.ToString());
    }

    partial void OnPerformanceOptimizationsEnabledChanged(bool value)
    {
        if (!ProVersion && value)
        {
            // Revert if trying to enable Pro feature in free version
            PerformanceOptimizationsEnabled = false;
            return;
        }
        SaveSetting("PerformanceOptimizationsEnabled", value.ToString());
    }

    partial void OnAdvancedDuplicateDetectionEnabledChanged(bool value)
    {
        if (!ProVersion && value)
        {
            AdvancedDuplicateDetectionEnabled = false;
            return;
        }
        SaveSetting("AdvancedDuplicateDetectionEnabled", value.ToString());
    }

    partial void OnSimilarityThresholdChanged(int value)
    {
        SaveSetting("SimilarityThreshold", value.ToString());
    }

    partial void OnGpuAccelerationEnabledChanged(bool value)
    {
        if (!ProVersion && value)
        {
            GpuAccelerationEnabled = false;
            return;
        }
        SaveSetting("GpuAccelerationEnabled", value.ToString());
    }

    partial void OnSelectedDetectionMethodChanged(string value)
    {
        SaveSetting("DetectionMethod", value);
    }

    partial void OnSelectedThemeChanged(ElementTheme value)
    {
        var themeString = value switch
        {
            ElementTheme.Light => "Light",
            ElementTheme.Dark => "Dark",
            _ => "Default"
        };
        SaveSetting("AppTheme", themeString);

        // Apply theme to main window
        if (App.MainWindow.Content is FrameworkElement root)
        {
            root.RequestedTheme = value;
        }
    }

    private void SaveSetting(string key, string value)
    {
        var data = new XmlData();
        data.WriteValue(key, value);
    }

    [RelayCommand]
    private async void Upgrade()
    {
        // Show license key input dialog
        var inputDialog = new ContentDialog
        {
            Title = "Activate Pro License",
            Content = CreateLicenseKeyInput(),
            PrimaryButtonText = "Activate",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = App.MainWindow.Content.XamlRoot
        };

        var result = await inputDialog.ShowAsync();
        
        if (result == ContentDialogResult.Primary)
        {
            var textBox = (inputDialog.Content as StackPanel)?.Children[1] as TextBox;
            var enteredKey = textBox?.Text;
            
            if (_licenseManager.ActivateLicense(enteredKey))
            {
                ProVersion = true;
                LicenseType = "Pro";
                ActivationDate = DateTime.Now.ToString("d");
                
                // Refresh feature flags
                FeatureFlagService.Instance.RefreshFeatureFlags();
                
                // Show success message
                await ShowMessageAsync("Success", "Pro license activated successfully!\n\nAll Pro features are now unlocked.");
            }
            else
            {
                // Show error message
                await ShowMessageAsync("Invalid License Key", "The license key you entered is invalid. Please check and try again.\n\nFormat: XXXX-XXXX-XXXX-XXXX");
            }
        }
    }

    private StackPanel CreateLicenseKeyInput()
    {
        var panel = new StackPanel { Spacing = 12 };
        
        var description = new TextBlock
        {
            Text = "Enter your Pro license key (format: XXXX-XXXX-XXXX-XXXX):",
            TextWrapping = TextWrapping.Wrap
        };
        
        var textBox = new TextBox
        {
            PlaceholderText = "XXXX-XXXX-XXXX-XXXX",
            MaxLength = 19
        };
        
        panel.Children.Add(description);
        panel.Children.Add(textBox);
        
        return panel;
    }

    private async System.Threading.Tasks.Task ShowMessageAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            CloseButtonText = "OK",
            XamlRoot = App.MainWindow.Content.XamlRoot
        };
        
        await dialog.ShowAsync();
    }

    [RelayCommand]
    private void GenerateTestKey()
    {
        // For development/testing only
        var testKey = LicenseManager.GenerateLicenseKey();
        LicenseKey = testKey;
        
        // Show the generated key in a dialog
        _ = ShowMessageAsync("Test License Key", $"Generated test key:\n\n{testKey}\n\nYou can use this to test the Pro upgrade.");
    }

    [RelayCommand]
    private void ResetToDefaults()
    {
        FilePreviewEnabled = true;
        PerformanceOptimizationsEnabled = false;
        AdvancedDuplicateDetectionEnabled = false;
        SimilarityThreshold = 70;
        GpuAccelerationEnabled = false;
        SelectedDetectionMethod = "PHash";
        SelectedTheme = ElementTheme.Default;
    }
}
