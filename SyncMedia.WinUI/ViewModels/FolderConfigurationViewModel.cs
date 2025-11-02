using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncMedia.Core;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace SyncMedia.WinUI.ViewModels;

public partial class FolderConfigurationViewModel : ObservableObject
{
    [ObservableProperty]
    private string sourceFolder = string.Empty;

    [ObservableProperty]
    private string destinationFolder = string.Empty;

    [ObservableProperty]
    private string statusMessage = string.Empty;

    public FolderConfigurationViewModel()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        var data = new XmlData();
        SourceFolder = data.ReadValue("SourceFolder") ?? string.Empty;
        DestinationFolder = data.ReadValue("DestinationFolder") ?? string.Empty;
    }

    [RelayCommand]
    private async Task BrowseSourceAsync()
    {
        var folder = await PickFolderAsync();
        if (!string.IsNullOrEmpty(folder))
        {
            SourceFolder = folder;
            SaveSettings();
            StatusMessage = "Source folder updated";
        }
    }

    [RelayCommand]
    private async Task BrowseDestinationAsync()
    {
        var folder = await PickFolderAsync();
        if (!string.IsNullOrEmpty(folder))
        {
            DestinationFolder = folder;
            SaveSettings();
            StatusMessage = "Destination folder updated";
        }
    }

    private async Task<string?> PickFolderAsync()
    {
        var picker = new FolderPicker();
        picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        picker.FileTypeFilter.Add("*");

        // Get the current window handle
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

        var folder = await picker.PickSingleFolderAsync();
        return folder?.Path;
    }

    private void SaveSettings()
    {
        var data = new XmlData();
        data.WriteValue("SourceFolder", SourceFolder);
        data.WriteValue("DestinationFolder", DestinationFolder);
    }

    [RelayCommand]
    private void Validate()
    {
        if (string.IsNullOrEmpty(SourceFolder) || string.IsNullOrEmpty(DestinationFolder))
        {
            StatusMessage = "⚠️ Both folders must be selected";
            return;
        }

        if (SourceFolder.Equals(DestinationFolder, System.StringComparison.OrdinalIgnoreCase))
        {
            StatusMessage = "⚠️ Source and destination cannot be the same";
            return;
        }

        if (!System.IO.Directory.Exists(SourceFolder))
        {
            StatusMessage = "⚠️ Source folder does not exist";
            return;
        }

        if (!System.IO.Directory.Exists(DestinationFolder))
        {
            StatusMessage = "⚠️ Destination folder does not exist";
            return;
        }

        StatusMessage = "✅ Configuration is valid";
    }
}
