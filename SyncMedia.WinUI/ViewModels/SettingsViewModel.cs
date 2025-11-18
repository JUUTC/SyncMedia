using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using SyncMedia.Core;
using SyncMedia.Core.Interfaces;
using SyncMedia.Core.Models;
using SyncMedia.Core.Services;
using System;

namespace SyncMedia.WinUI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IAdvertisingService _advertisingService;
    private readonly LicenseManager _licenseManager;
    private readonly FeatureFlagService _featureFlagService;

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
    private int filesProcessed = 0;

    [ObservableProperty]
    private int filesRemaining = 0;

    [ObservableProperty]
    private int bonusFiles = 0;

    [ObservableProperty]
    private bool hasSpeedBoost = false;

    [ObservableProperty]
    private string speedBoostStatus = "";

    public SettingsViewModel(IAdvertisingService advertisingService, LicenseManager licenseManager, FeatureFlagService featureFlagService)
    {
        _advertisingService = advertisingService;
        _licenseManager = licenseManager;
        _featureFlagService = featureFlagService;
        LoadSettings();
    }

    private void LoadSettings()
    {
        var data = new XmlData();
        
        // General settings
        FilePreviewEnabled = bool.Parse(data.ReadValue("FilePreviewEnabled") ?? "true");
        PerformanceOptimizationsEnabled = bool.Parse(data.ReadValue("PerformanceOptimizationsEnabled") ?? "false");
        
        // Pro features
        ProVersion = _licenseManager.CurrentLicense.IsPro;
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

        // Update license type and file counts
        UpdateLicenseInfo();
    }

    private void UpdateLicenseInfo()
    {
        var license = _licenseManager.CurrentLicense;
        LicenseType = license.IsPro ? "Pro" : "Free";
        FilesProcessed = license.FilesProcessedCount;
        FilesRemaining = license.RemainingFreeFiles;
        BonusFiles = license.BonusFilesFromAds;
        HasSpeedBoost = license.HasActiveSpeedBoost;
        
        if (HasSpeedBoost && license.SpeedBoostExpirationDate.HasValue)
        {
            var remaining = license.SpeedBoostExpirationDate.Value - DateTime.Now;
            SpeedBoostStatus = $"Active ({remaining.Minutes} min remaining)";
        }
        else
        {
            SpeedBoostStatus = "Inactive";
        }
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
    private void Upgrade()
    {
        // TODO: Implement in-app purchase flow (Phase 4)
        // For now, just toggle for testing
        ProVersion = true;
        LicenseType = "Pro";
        SaveSetting("ProVersion", "true");
        
        // Activate the license in LicenseManager
        // In production, this would validate a real license key from the store
        var testLicenseKey = LicenseManager.GenerateLicenseKey();
        _licenseManager.ActivateLicense(testLicenseKey);
        
        // Refresh feature flags
        _featureFlagService.RefreshFeatureFlags();
        
        // Update ad visibility - hide ads for Pro users
        _advertisingService.UpdateAdVisibility(_featureFlagService.ShouldShowAds);
        
        // Refresh license info display
        UpdateLicenseInfo();
    }

    [RelayCommand]
    private void RefreshLicenseInfo()
    {
        UpdateLicenseInfo();
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
