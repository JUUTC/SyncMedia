using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SyncMedia.WinUI.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        [RelayCommand]
        private void ConfigureFolders()
        {
            // Navigate to folder configuration page
            if (App.MainWindow?.Content is Microsoft.UI.Xaml.Controls.Frame frame)
            {
                var mainWindow = frame.Content as Views.MainWindow;
                // Navigation will be handled by MainWindow
            }
        }

        [RelayCommand]
        private void StartSync()
        {
            // Navigate to sync page
        }

        [RelayCommand]
        private void ViewStatistics()
        {
            // Navigate to statistics page
        }
    }
}
