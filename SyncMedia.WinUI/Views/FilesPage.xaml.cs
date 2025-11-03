using Microsoft.UI.Xaml.Controls;
using SyncMedia.WinUI.ViewModels;

namespace SyncMedia.WinUI.Views
{
    public sealed partial class FilesPage : Page
    {
        public FilesViewModel ViewModel { get; }

        public FilesPage()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<FilesViewModel>();
        }
    }
}
