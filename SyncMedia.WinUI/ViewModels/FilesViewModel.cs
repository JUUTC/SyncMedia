using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

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

        public FilesViewModel()
        {
            // Initialize with empty collection
            // TODO: Load actual sync results from SyncMedia.Core
        }

        [RelayCommand]
        private void Search()
        {
            // TODO: Implement search filtering
        }

        [RelayCommand]
        private void ClearResults()
        {
            SyncResults.Clear();
        }

        [RelayCommand]
        private void ExportResults()
        {
            // TODO: Implement CSV export
        }

        [RelayCommand]
        private void ViewFileDetails()
        {
            // TODO: Show file details dialog
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
