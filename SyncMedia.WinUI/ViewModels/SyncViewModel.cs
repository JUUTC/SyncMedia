using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncMedia.Core;
using SyncMedia.Core.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SyncMedia.WinUI.ViewModels
{
    public partial class SyncViewModel : ObservableObject
    {
        private readonly SyncService _syncService;
        private CancellationTokenSource _cancellationTokenSource;
        private DateTime _startTime;
        private readonly FilesViewModel _filesViewModel;

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

        public SyncViewModel(FilesViewModel filesViewModel)
        {
            _filesViewModel = filesViewModel;
            _syncService = new SyncService();
            
            // Subscribe to sync service events
            _syncService.ProgressChanged += OnProgressChanged;
            _syncService.FileProcessed += OnFileProcessed;
            _syncService.SyncCompleted += OnSyncCompleted;
        }

        [RelayCommand(CanExecute = nameof(CanStartSync))]
        private async Task StartSyncAsync()
        {
            IsSyncing = true;
            _startTime = DateTime.Now;
            StatusMessage = "Sync in progress...";
            
            // Reset values
            ProgressPercentage = 0;
            FilesProcessed = 0;
            TotalFiles = 0;
            CurrentFile = "";
            _filesViewModel.ClearResults();

            // Create cancellation token
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Get sync settings
                string sourceFolder = XmlData.ReadSetting("SourceFolder");
                string destFolder = XmlData.ReadSetting("DestinationFolder");

                if (string.IsNullOrEmpty(sourceFolder) || string.IsNullOrEmpty(destFolder))
                {
                    StatusMessage = "Error: Source and destination folders must be configured";
                    IsSyncing = false;
                    return;
                }

                // Get file type settings
                var options = new SyncOptions
                {
                    IncludeImages = bool.TryParse(XmlData.ReadSetting("IncludeImages"), out bool incImg) ? incImg : true,
                    IncludeVideos = bool.TryParse(XmlData.ReadSetting("IncludeVideos"), out bool incVid) ? incVid : true,
                    IncludeMusic = bool.TryParse(XmlData.ReadSetting("IncludeMusic"), out bool incMus) ? incMus : false,
                    IncludeDocuments = bool.TryParse(XmlData.ReadSetting("IncludeDocuments"), out bool incDoc) ? incDoc : false
                };

                // Get custom file types
                string customTypes = XmlData.ReadSetting("CustomFileTypes");
                if (!string.IsNullOrWhiteSpace(customTypes))
                {
                    options.CustomExtensions = customTypes.Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToHashSet();
                }

                // Start sync operation
                var result = await _syncService.StartSyncAsync(
                    sourceFolder,
                    destFolder,
                    options,
                    _cancellationTokenSource.Token);

                if (result.Success)
                {
                    StatusMessage = result.WasCancelled ? "Sync cancelled" : "Sync completed successfully";
                }
                else
                {
                    StatusMessage = $"Sync failed: {result.ErrorMessage}";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsSyncing = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        private bool CanStartSync()
        {
            return !IsSyncing;
        }

        [RelayCommand(CanExecute = nameof(CanStopSync))]
        private void StopSync()
        {
            _cancellationTokenSource?.Cancel();
            StatusMessage = "Stopping sync...";
        }

        private bool CanStopSync()
        {
            return IsSyncing;
        }

        private void OnProgressChanged(object sender, SyncProgressEventArgs e)
        {
            // Update UI on UI thread
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ProgressPercentage = e.ProgressPercentage;
                FilesProcessed = e.FilesProcessed;
                TotalFiles = e.TotalFiles;
                CurrentFile = e.CurrentFile;
                ElapsedTime = e.ElapsedTime.ToString(@"hh\:mm\:ss");

                // Calculate processing speed
                if (e.ElapsedTime.TotalSeconds > 0)
                {
                    ProcessingSpeed = e.FilesProcessed / e.ElapsedTime.TotalSeconds;

                    // Estimate time remaining
                    if (ProcessingSpeed > 0)
                    {
                        int remaining = e.TotalFiles - e.FilesProcessed;
                        double secondsRemaining = remaining / ProcessingSpeed;
                        EstimatedTimeRemaining = TimeSpan.FromSeconds(secondsRemaining).ToString(@"hh\:mm\:ss");
                    }
                }
            });
        }

        private void OnFileProcessed(object sender, FileProcessedEventArgs e)
        {
            // Add result to FilesViewModel
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                _filesViewModel.AddResult(e.Result);
            });
        }

        private void OnSyncCompleted(object sender, SyncCompletedEventArgs e)
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                var stats = e.Statistics;
                StatusMessage = e.WasCancelled
                    ? $"Sync cancelled. Processed: {stats.ProcessedFiles}, Success: {stats.SuccessfulFiles}, Skipped: {stats.SkippedFiles}, Errors: {stats.ErrorFiles}"
                    : $"Sync complete! Processed: {stats.ProcessedFiles}, Success: {stats.SuccessfulFiles}, Skipped: {stats.SkippedFiles}, Errors: {stats.ErrorFiles}";
            });
        }
    }
}
