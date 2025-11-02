using Microsoft.UI.Xaml;
using SyncMedia.WinUI.ViewModels;

namespace SyncMedia.WinUI.Views
{
    public sealed partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }

        public MainWindow(MainViewModel viewModel)
        {
            this.InitializeComponent();
            ViewModel = viewModel;
            
            // Set window size
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1200, 800));
        }
    }
}
