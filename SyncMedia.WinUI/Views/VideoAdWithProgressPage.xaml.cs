using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SyncMedia.Core.Interfaces;
using SyncMedia.Core.Services;
using System;
using System.Threading.Tasks;

namespace SyncMedia.WinUI.Views
{
    public sealed partial class VideoAdWithProgressPage : Page
    {
        private readonly IAdvertisingService _advertisingService;
        private readonly LicenseManager _licenseManager;
        private readonly FeatureFlagService _featureFlagService;
        private int _filesProcessed;
        private int _totalFiles;
        private int _secondsWatched;
        private bool _adCompleted;

        public event EventHandler AdWatchCompleted;
        public event EventHandler AdWatchSkipped;
        public event EventHandler UpgradeRequested;

        public VideoAdWithProgressPage(IAdvertisingService advertisingService, 
            LicenseManager licenseManager, 
            FeatureFlagService featureFlagService)
        {
            this.InitializeComponent();
            _advertisingService = advertisingService;
            _licenseManager = licenseManager;
            _featureFlagService = featureFlagService;
            _secondsWatched = 0;
            _adCompleted = false;

            // Subscribe to ad events
            _advertisingService.VideoAdCompleted += OnVideoAdCompleted;

            // Start skip button timer (enable after 15 seconds)
            StartSkipButtonTimer();
        }

        public void UpdateSyncProgress(int filesProcessed, int totalFiles, string currentFile)
        {
            _filesProcessed = filesProcessed;
            _totalFiles = totalFiles;

            FilesProcessedText.Text = filesProcessed.ToString();
            FilesRemainingText.Text = totalFiles.ToString();
            CurrentFileText.Text = $"Current: {currentFile}";

            double percentage = totalFiles > 0 ? (double)filesProcessed / totalFiles * 100 : 0;
            SyncProgressBar.Value = percentage;
            ProgressPercentText.Text = $"{percentage:F0}%";
        }

        public void ShowVideoAd()
        {
            LoadingRing.IsActive = true;

            if (_advertisingService.IsVideoAdReady)
            {
                // Show the video ad
                bool shown = _advertisingService.ShowInterstitialVideoAd();
                
                if (!shown)
                {
                    ShowError();
                }
            }
            else
            {
                // Ad not ready, show error
                ShowError();
            }
        }

        private void ShowError()
        {
            LoadingRing.IsActive = false;
            ErrorOverlay.Visibility = Visibility.Visible;
        }

        private void OnVideoAdCompleted(object sender, VideoAdCompletedEventArgs e)
        {
            _adCompleted = true;

            // Run on UI thread
            DispatcherQueue.TryEnqueue(() =>
            {
                if (e.WatchedCompletely)
                {
                    // Full reward: bonus files + speed boost
                    _licenseManager.AddBonusFilesFromVideoAd();
                    _licenseManager.ActivateSpeedBoost(60);
                    
                    StatusText.Text = "✓ Ad watched! You earned +20 files and 60min speed boost";
                }
                else
                {
                    // Partial reward: fewer bonus files
                    _licenseManager.AddBonusFilesFromClick(); // +10 files
                    
                    StatusText.Text = "✓ Ad partially watched! You earned +10 files";
                }

                // Notify completion
                AdWatchCompleted?.Invoke(this, EventArgs.Empty);
            });
        }

        private async void StartSkipButtonTimer()
        {
            // Enable skip after 15 seconds
            await Task.Delay(TimeSpan.FromSeconds(15));
            
            if (!_adCompleted)
            {
                SkipButton.IsEnabled = true;
                SkipButton.Content = "Skip (Get +5 files)";
            }
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            // Give small reward for partial watch
            _licenseManager.AddBonusFilesFromClick();
            _licenseManager.AddBonusFilesFromClick(); // Total +5 files (half of click reward)
            
            StatusText.Text = "Ad skipped. You earned +5 files";
            
            AdWatchSkipped?.Invoke(this, EventArgs.Empty);
        }

        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorOverlay.Visibility = Visibility.Collapsed;
            ShowVideoAd();
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            UpgradeRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
