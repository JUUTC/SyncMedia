using Microsoft.UI.Xaml.Controls;
using SyncMedia.WinUI.ViewModels;

namespace SyncMedia.WinUI.Views
{
    public sealed partial class NamingListPage : Page
    {
        public NamingListViewModel ViewModel { get; }

        public NamingListPage(NamingListViewModel viewModel)
        {
            ViewModel = viewModel;
            this.InitializeComponent();
        }
    }
}
