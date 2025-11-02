using Microsoft.UI.Xaml.Controls;
using SyncMedia.WinUI.ViewModels;

namespace SyncMedia.WinUI.Views
{
    public sealed partial class FileTypesPage : Page
    {
        public FileTypesViewModel ViewModel { get; }

        public FileTypesPage(FileTypesViewModel viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }
    }
}
