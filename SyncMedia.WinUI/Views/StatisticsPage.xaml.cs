using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SyncMedia.WinUI.ViewModels;

namespace SyncMedia.WinUI.Views
{
    public sealed partial class StatisticsPage : Page
    {
        public StatisticsViewModel ViewModel { get; }

        public StatisticsPage()
        {
            this.InitializeComponent();
            ViewModel = App.GetService<StatisticsViewModel>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.LoadStatistics();
        }
    }
}
