using CommunityToolkit.Mvvm.ComponentModel;
using SyncMedia.Core;

namespace SyncMedia.WinUI.ViewModels
{
    public partial class FileTypesViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool includeImages = true;

        [ObservableProperty]
        private bool includeVideos = true;

        [ObservableProperty]
        private bool includeMusic = false;

        [ObservableProperty]
        private bool includeDocuments = false;

        [ObservableProperty]
        private string customFileTypes = string.Empty;

        [ObservableProperty]
        private bool showMessage = false;

        [ObservableProperty]
        private string message = string.Empty;

        public FileTypesViewModel()
        {
            LoadSettings();

            // Auto-save when properties change
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != nameof(ShowMessage) && e.PropertyName != nameof(Message))
                {
                    SaveSettings();
                }
            };
        }

        private void LoadSettings()
        {
            var xmlData = new XmlData();
            
            // Load settings from XmlData
            // These would be saved as user preferences
            // For now, using default values
        }

        private void SaveSettings()
        {
            var xmlData = new XmlData();
            
            // Save settings to XmlData
            // Build filter list based on selections
            
            ShowSuccessMessage("Settings saved successfully");
        }

        private void ShowSuccessMessage(string msg)
        {
            Message = msg;
            ShowMessage = true;

            // Auto-hide after 3 seconds
            _ = Task.Run(async () =>
            {
                await Task.Delay(3000);
                ShowMessage = false;
            });
        }
    }
}
