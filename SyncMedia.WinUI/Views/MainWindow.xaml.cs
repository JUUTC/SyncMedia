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
        private readonly IConnectivityService _connectivityService;
        private readonly FeatureFlagService _featureFlagService;
        private bool _isAppPaused;

        public MainWindow(MainViewModel viewModel, IAdvertisingService advertisingService, 
            IConnectivityService connectivityService, FeatureFlagService featureFlagService)
        {
            this.InitializeComponent();
            ViewModel = viewModel;
            _advertisingService = advertisingService;
            _connectivityService = connectivityService;
            _featureFlagService = featureFlagService;
            _isAppPaused = false;
            
            // Set window size
            this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1200, 800));

            // Initialize advertising
            InitializeAdvertising();

            // Subscribe to connectivity events
            SubscribeToConnectivityEvents();

            // Subscribe to overlay events
            ConnectivityOverlay.RetryRequested += OnRetryRequested;
            ConnectivityOverlay.UpgradeRequested += OnUpgradeRequested;

            // Start connectivity monitoring
            _connectivityService.StartMonitoring();

            // Check initial connectivity state
            CheckConnectivityAndAds();

            // Navigate to home page by default (if not paused)
            if (!_isAppPaused)
            {
                ContentFrame.Navigate(typeof(HomePage));
            }
        }

        private void InitializeAdvertising()
        {
            // Initialize the advertising service with the AdControl
            if (_advertisingService is MicrosoftAdvertisingService msAdService)
            {
                msAdService.InitializeWithControl(AdControlBanner);
            }

            // Subscribe to ad events
            _advertisingService.AdLoaded += OnAdLoaded;
            _advertisingService.AdFailed += OnAdFailed;
            _advertisingService.AdBlockingDetected += OnAdBlockingDetected;
        }

        private void SubscribeToConnectivityEvents()
        {
            _connectivityService.ConnectivityChanged += OnConnectivityChanged;
        }

        private void CheckConnectivityAndAds()
        {
            bool shouldShowAds = _featureFlagService.ShouldShowAds;
            
            if (!shouldShowAds)
            {
                // Pro user or in trial - no need to check connectivity
                HideConnectivityOverlay();
                UpdateAdVisibility();
                return;
            }

            // Free user - check if we have internet
            bool hasInternet = _connectivityService.CheckConnectivity();
            
            if (!hasInternet)
            {
                // No internet - pause app and show overlay
                PauseAppForConnectivity("No internet connection detected");
            }
            else
            {
                // Has internet - allow app to continue
                ResumeApp();
                UpdateAdVisibility();
            }
        }

        private void UpdateAdVisibility()
        {
            bool shouldShowAds = _featureFlagService.ShouldShowAds;
            
            // Update ad visibility
            _advertisingService.UpdateAdVisibility(shouldShowAds);
            
            // Also update the border visibility
            AdBorder.Visibility = shouldShowAds ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PauseAppForConnectivity(string reason)
        {
            _isAppPaused = true;
            
            // Show overlay
            ConnectivityOverlayContainer.Visibility = Visibility.Visible;
            
            // Update overlay message based on reason
            if (reason.Contains("blocked", StringComparison.OrdinalIgnoreCase) || 
                reason.Contains("ad block", StringComparison.OrdinalIgnoreCase))
            {
                ConnectivityOverlay.ShowAdBlockedMessage();
            }
            else
            {
                ConnectivityOverlay.ShowOfflineMessage();
            }
            
            ConnectivityOverlay.UpdateStatus(false, reason);
            
            // Disable navigation
            NavView.IsEnabled = false;
        }

        private void HideConnectivityOverlay()
        {
            ConnectivityOverlayContainer.Visibility = Visibility.Collapsed;
        }

        private void ResumeApp()
        {
            _isAppPaused = false;
            
            // Hide overlay
            HideConnectivityOverlay();
            
            // Enable navigation
            NavView.IsEnabled = true;
        }

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            // Run on UI thread
            DispatcherQueue.TryEnqueue(() =>
            {
                if (_featureFlagService.ShouldShowAds)
                {
                    if (e.IsConnected)
                    {
                        // Internet restored - resume app
                        ResumeApp();
                        UpdateAdVisibility();
                    }
                    else
                    {
                        // Internet lost - pause app
                        PauseAppForConnectivity(e.Reason);
                    }
                }
            });
        }

        private void OnAdLoaded(object sender, EventArgs e)
        {
            // Ad loaded successfully - ensure app is not paused
            DispatcherQueue.TryEnqueue(() =>
            {
                if (_isAppPaused && _connectivityService.IsConnected)
                {
                    ResumeApp();
                }
            });
        }

        private void OnAdFailed(object sender, Core.Interfaces.AdErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Ad failed: {e.ErrorMessage}");
            // Ad failure is handled by the ad blocking detection
        }

        private void OnAdBlockingDetected(object sender, Core.Interfaces.AdBlockedEventArgs e)
        {
            // Ad blocking detected - pause app for free users
            DispatcherQueue.TryEnqueue(() =>
            {
                if (_featureFlagService.ShouldShowAds)
                {
                    string reason = e.IsNetworkIssue 
                        ? "Unable to load advertisements due to network issues" 
                        : "Ad blocking software detected";
                    
                    PauseAppForConnectivity(reason);
                }
            });
        }

        private void OnRetryRequested(object sender, EventArgs e)
        {
            // User clicked retry - check connectivity again
            ConnectivityOverlay.UpdateStatus(true, "Checking connection...");
            
            System.Threading.Tasks.Task.Delay(1000).ContinueWith(_ =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    CheckConnectivityAndAds();
                });
            });
        }

        private void OnUpgradeRequested(object sender, EventArgs e)
        {
            // User wants to upgrade - navigate to settings
            ResumeApp();
            ContentFrame.Navigate(typeof(SettingsPage));
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
