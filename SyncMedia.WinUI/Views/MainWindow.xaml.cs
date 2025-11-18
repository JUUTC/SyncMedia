using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SyncMedia.Core.Interfaces;
using SyncMedia.Core.Services;
using SyncMedia.WinUI.Services;
using SyncMedia.WinUI.ViewModels;
using System;

namespace SyncMedia.WinUI.Views
{
    public sealed partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }
        private readonly IAdvertisingService _advertisingService;
        private readonly FeatureFlagService _featureFlagService;

        public MainWindow(MainViewModel viewModel, IAdvertisingService advertisingService, FeatureFlagService featureFlagService)
        {
            this.InitializeComponent();
            ViewModel = viewModel;
            _advertisingService = advertisingService;
            _featureFlagService = featureFlagService;
            
            // Set window size
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1200, 800));

            // Initialize advertising
            InitializeAdvertising();

            // Navigate to home page by default
            ContentFrame.Navigate(typeof(HomePage));
        }

        private void InitializeAdvertising()
        {
            // Initialize the advertising service with the AdControl
            if (_advertisingService is MicrosoftAdvertisingService msAdService)
            {
                msAdService.InitializeWithControl(AdControlBanner);
            }

            // Show or hide ads based on license status
            UpdateAdVisibility();
        }

        private void UpdateAdVisibility()
        {
            bool shouldShowAds = _featureFlagService.ShouldShowAds;
            
            // Update ad visibility
            _advertisingService.UpdateAdVisibility(shouldShowAds);
            
            // Also update the border visibility
            AdBorder.Visibility = shouldShowAds ? Visibility.Visible : Visibility.Collapsed;
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
