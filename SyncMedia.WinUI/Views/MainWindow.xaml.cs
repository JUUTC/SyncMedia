using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SyncMedia.WinUI.ViewModels;
using System;

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

            // Navigate to home page by default
            ContentFrame.Navigate(typeof(HomePage));
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                var tag = args.SelectedItemContainer.Tag?.ToString();
                NavigateToPage(tag);
            }
        }

        private void NavView_SettingsInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            ContentFrame.Navigate(typeof(SettingsPage));
        }

        private void NavigateToPage(string tag)
        {
            Type pageType = tag switch
            {
                "home" => typeof(HomePage),
                "folders" => typeof(FolderConfigurationPage),
                "fileTypes" => typeof(FileTypesPage),
                "namingList" => typeof(NamingListPage),
                "sync" => typeof(SyncPage),
                "files" => typeof(FilesPage),
                "stats" => typeof(StatisticsPage),
                _ => null
            };

            if (pageType != null && ContentFrame.CurrentSourcePageType != pageType)
            {
                ContentFrame.Navigate(pageType);
            }
        }
    }

    // Placeholder page - to be implemented in Week 4
    public sealed partial class StatisticsPage : Page
    {
        public StatisticsPage()
        {
            var text = new TextBlock
            {
                Text = "Statistics Page - Coming in Week 4",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            this.Content = text;
        }
    }
}
