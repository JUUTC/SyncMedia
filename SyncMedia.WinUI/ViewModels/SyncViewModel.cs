using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace SyncMedia.WinUI.ViewModels
{
    public partial class SyncViewModel : ObservableObject
    {
        [ObservableProperty]
        private string statusMessage = "Ready to sync";

        [ObservableProperty]
        private double progressPercentage = 0;

        [ObservableProperty]
        private int filesProcessed = 0;

        [ObservableProperty]
        private int totalFiles = 0;

        [ObservableProperty]
        private string currentFile = "";

        [ObservableProperty]
        private bool isSyncing = false;

        [ObservableProperty]
        private string elapsedTime = "00:00:00";

        [ObservableProperty]
        private string estimatedTimeRemaining = "00:00:00";

        [ObservableProperty]
        private double processingSpeed = 0; // files per second

        public SyncViewModel()
        {
            // Initialize with default values
        }

        [RelayCommand(CanExecute = nameof(CanStartSync))]
        private async Task StartSyncAsync()
        {
            IsSyncing = true;
            StatusMessage = "Sync in progress...";
            
            // TODO: Integrate with SyncMedia.Core sync logic
            // This is a placeholder for the actual sync implementation
            await Task.Delay(100);
        }

        private bool CanStartSync()
        {
            return !IsSyncing;
        }

        [RelayCommand(CanExecute = nameof(CanStopSync))]
        private void StopSync()
        {
            IsSyncing = false;
            StatusMessage = "Sync stopped by user";
            
            // TODO: Cancel sync operation
        }

        private bool CanStopSync()
        {
            return IsSyncing;
        }

        [RelayCommand(CanExecute = nameof(CanPauseSync))]
        private void PauseSync()
        {
            // TODO: Implement pause logic
            StatusMessage = "Sync paused";
        }

        private bool CanPauseSync()
        {
            return IsSyncing;
        }

        public void UpdateProgress(int processed, int total, string currentFileName)
        {
            FilesProcessed = processed;
            TotalFiles = total;
            CurrentFile = currentFileName;
            ProgressPercentage = total > 0 ? (double)processed / total * 100 : 0;
        }
    }
}
