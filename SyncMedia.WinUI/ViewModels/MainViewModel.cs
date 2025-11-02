using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncMedia.Core;
using System.Collections.ObjectModel;

namespace SyncMedia.WinUI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _statusMessage = "Ready";

        [ObservableProperty]
        private bool _isBusy = false;

        [ObservableProperty]
        private int _progressValue = 0;

        [ObservableProperty]
        private string _sourceFolder = string.Empty;

        [ObservableProperty]
        private string _targetFolder = string.Empty;

        public ObservableCollection<string> FileResults { get; } = new ObservableCollection<string>();

        public MainViewModel()
        {
            // Initialize with Core library services
            StatusMessage = "SyncMedia WinUI 3 - Ready to sync media files";
        }

        [RelayCommand]
        private async void StartSync()
        {
            IsBusy = true;
            StatusMessage = "Synchronization started...";
            
            try
            {
                // TODO: Implement sync logic using SyncMedia.Core
                FileResults.Add("Sync operation will be implemented here");
                
                StatusMessage = "Synchronization completed successfully";
            }
            catch (System.Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private void SelectSourceFolder()
        {
            // TODO: Implement folder picker
            StatusMessage = "Folder picker will be implemented";
        }

        [RelayCommand]
        private void SelectTargetFolder()
        {
            // TODO: Implement folder picker
            StatusMessage = "Folder picker will be implemented";
        }
    }
}
