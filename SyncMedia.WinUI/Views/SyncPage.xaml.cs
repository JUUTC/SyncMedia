using Microsoft.UI.Xaml.Controls;
using SyncMedia.WinUI.ViewModels;

namespace SyncMedia.WinUI.Views
{
    public sealed partial class SyncPage : Page
    {
        public SyncViewModel ViewModel { get; }

        public SyncPage()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<SyncViewModel>();
        }
    }
}
