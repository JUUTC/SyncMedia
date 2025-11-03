using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SyncMedia.Core.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace SyncMedia.WinUI.ViewModels
{
    public partial class FilesViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<SyncFileResult> syncResults = new();

        [ObservableProperty]
        private SyncFileResult? selectedResult;

        [ObservableProperty]
        private string searchText = "";

        [ObservableProperty]
        private string filterStatus = "All";

        [ObservableProperty]
        private int totalFiles;

        [ObservableProperty]
        private int successCount;

        [ObservableProperty]
        private int errorCount;

        [ObservableProperty]
        private int skippedCount;

        [ObservableProperty]
        private string totalSizeProcessed = "0 bytes";

        public FilesViewModel()
        {
            // Initialize with empty collection
        }

        public void AddResult(FileProcessResult result)
        {
            var syncResult = new SyncFileResult
            {
                FileName = result.FileName,
                FilePath = result.FilePath,
                Status = result.Status,
                Action = result.Action,
                FileSize = result.SizeBytes,
                Timestamp = result.LastModified.ToString("yyyy-MM-dd HH:mm:ss"),
                FileType = Path.GetExtension(result.FileName),
                ErrorMessage = result.ErrorMessage
            };

            SyncResults.Add(syncResult);
            UpdateStatistics();
        }

        [RelayCommand]
        private void Search()
        {
            // Filter sync results based on search text
            if (string.IsNullOrWhiteSpace(SearchText))
                return;

            var filtered = SyncResults.Where(r =>
                r.FileName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                r.FilePath.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            // Update collection (in real implementation, would use CollectionView filtering)
        }

        [RelayCommand]
        public void ClearResults()
        {
            SyncResults.Clear();
            UpdateStatistics();
        }

        [RelayCommand]
        private async void ExportResults()
        {
            try
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hwnd);

                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("CSV File", new[] { ".csv" });
                savePicker.SuggestedFileName = $"SyncResults_{DateTime.Now:yyyyMMdd_HHmmss}";

                var file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    var csv = new StringBuilder();
                    csv.AppendLine("File Name,Status,Action,Size,Type,Timestamp,Path");

                    foreach (var result in SyncResults)
                    {
                        csv.AppendLine($"\"{result.FileName}\",\"{result.Status}\",\"{result.Action}\",\"{result.FileSizeFormatted}\",\"{result.FileType}\",\"{result.Timestamp}\",\"{result.FilePath}\"");
                    }

                    await Windows.Storage.FileIO.WriteTextAsync(file, csv.ToString());
                }
            }
            catch (Exception ex)
            {
                // Handle export error
                System.Diagnostics.Debug.WriteLine($"Export failed: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ViewFileDetails()
        {
            if (SelectedResult != null)
            {
                // TODO: Show file details dialog with full information
            }
        }

        private void UpdateStatistics()
        {
            TotalFiles = SyncResults.Count;
            SuccessCount = SyncResults.Count(r => r.Status == "Success");
            ErrorCount = SyncResults.Count(r => r.Status == "Error");
            SkippedCount = SyncResults.Count(r => r.Status == "Skipped");

            long totalBytes = SyncResults.Where(r => r.Status == "Success").Sum(r => r.FileSize);
            TotalSizeProcessed = FormatFileSize(totalBytes);
        }

        private static string FormatFileSize(long bytes)
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;

            if (bytes >= GB)
                return $"{bytes / (double)GB:F2} GB";
            if (bytes >= MB)
                return $"{bytes / (double)MB:F2} MB";
            if (bytes >= KB)
                return $"{bytes / (double)KB:F2} KB";

            return $"{bytes} bytes";
        }
    }

    // Model for sync file results
    public class SyncFileResult
    {
        public string FileName { get; set; } = "";
        public string FilePath { get; set; } = "";
        public string Status { get; set; } = "";
        public string Action { get; set; } = "";
        public long FileSize { get; set; }
        public string FileSizeFormatted => FormatFileSize(FileSize);
        public string FileType { get; set; } = "";
        public string Timestamp { get; set; } = "";
        public string ErrorMessage { get; set; } = "";
        
        public string StatusIcon => Status switch
        {
            "Success" => "\uE73E", // Checkmark
            "Error" => "\uE783",   // Error
            "Skipped" => "\uE711", // Forward
            _ => "\uE9CE"          // Status
        };

        private static string FormatFileSize(long bytes)
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;

            if (bytes >= GB)
                return $"{bytes / (double)GB:F2} GB";
            if (bytes >= MB)
                return $"{bytes / (double)MB:F2} MB";
            if (bytes >= KB)
                return $"{bytes / (double)KB:F2} KB";

            return $"{bytes} bytes";
        }
    }
}
