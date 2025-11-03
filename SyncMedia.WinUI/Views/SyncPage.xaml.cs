using Microsoft.UI.Xaml.Controls;
using SyncMedia.WinUI.ViewModels;
using System.ComponentModel;

namespace SyncMedia.WinUI.Views
{
    public sealed partial class SyncPage : Page
    {
        public SyncViewModel ViewModel { get; }

        public SyncPage()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<SyncViewModel>();
            
            // Subscribe to ViewModel property changes to update preview
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Update preview when current file path changes
            if (e.PropertyName == nameof(ViewModel.CurrentFilePath))
            {
                if (!string.IsNullOrEmpty(ViewModel.CurrentFilePath) && ViewModel.IsPreviewEnabled)
                {
                    await FilePreview.ShowPreviewAsync(ViewModel.CurrentFilePath);
                }
            }
            // Update preview enabled state
            else if (e.PropertyName == nameof(ViewModel.IsPreviewEnabled))
            {
                FilePreview.IsPreviewEnabled = ViewModel.IsPreviewEnabled;
            }
        }
    }
}
