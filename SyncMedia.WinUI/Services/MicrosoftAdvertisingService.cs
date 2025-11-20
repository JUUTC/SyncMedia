using Microsoft.Advertising.WinRT.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SyncMedia.Core.Interfaces;
using System;

namespace SyncMedia.WinUI.Services
{
    /// <summary>
    /// Service for managing Microsoft Advertising in WinUI 3
    /// </summary>
    public class MicrosoftAdvertisingService : IAdvertisingService
    {
        private AdControl _adControl;
        private InterstitialAd _interstitialAd;
        private bool _initialized;
        private int _consecutiveAdFailures;
        private DateTime _lastAdLoadAttempt;
        private const int AD_BLOCK_THRESHOLD = 3; // Consider blocked after 3 failures

        // Microsoft Advertising test IDs (replace with real IDs for production)
        // Get your Application ID and Ad Unit ID from https://apps.microsoft.com
        private const string APPLICATION_ID = "9nblggh5ggsx"; // Test Application ID
        private const string AD_UNIT_ID = "test"; // Test Ad Unit ID
        private const string VIDEO_AD_UNIT_ID = "11389925"; // Test video ad unit ID
        
        public bool AdsEnabled { get; private set; }
        public bool AdsBlocked { get; private set; }
        public bool IsVideoAdReady { get; private set; }

        public event EventHandler<Core.Interfaces.AdBlockedEventArgs> AdBlockingDetected;
        public event EventHandler AdLoaded;
        public event EventHandler<Core.Interfaces.AdErrorEventArgs> AdFailed;
        public event EventHandler<Core.Interfaces.VideoAdCompletedEventArgs> VideoAdCompleted;
        public event EventHandler AdClicked;

        public MicrosoftAdvertisingService()
        {
            _initialized = false;
            AdsEnabled = false;
            AdsBlocked = false;
            _consecutiveAdFailures = 0;
            IsVideoAdReady = false;
            
            // Initialize interstitial ad for video ads
            InitializeInterstitialAd();
        }

        /// <summary>
        /// Initialize interstitial ad for video advertising
        /// </summary>
        private void InitializeInterstitialAd()
        {
            _interstitialAd = new InterstitialAd();
            _interstitialAd.AdReady += OnInterstitialAdReady;
            _interstitialAd.ErrorOccurred += OnInterstitialAdError;
            _interstitialAd.Completed += OnInterstitialAdCompleted;
            _interstitialAd.Cancelled += OnInterstitialAdCancelled;
            
            // Request a video ad
            _interstitialAd.RequestAd(AdType.Video, APPLICATION_ID, VIDEO_AD_UNIT_ID);
        }

        /// <summary>
        /// Initialize the advertising service with an AdControl
        /// </summary>
        /// <param name="adControl">The AdControl from XAML</param>
        public void InitializeWithControl(AdControl adControl)
        {
            if (_initialized)
                return;

            _adControl = adControl;
            
            if (_adControl != null)
            {
                // Configure the AdControl
                _adControl.ApplicationId = APPLICATION_ID;
                _adControl.AdUnitId = AD_UNIT_ID;
                _adControl.Height = 90; // Standard banner height
                _adControl.Width = 728; // Standard banner width
                
                // Subscribe to ad events
                _adControl.AdRefreshed += OnAdRefreshed;
                _adControl.ErrorOccurred += OnAdErrorOccurred;
                
                _initialized = true;
            }
        }

        public void Initialize()
        {
            // This method is called from DI but actual initialization happens in InitializeWithControl
            // when the AdControl from XAML is available
        }

        public void ShowAds()
        {
            if (_adControl != null)
            {
                _adControl.Visibility = Visibility.Visible;
                AdsEnabled = true;
                
                // Refresh the ad to start showing
                _lastAdLoadAttempt = DateTime.Now;
                _adControl.Refresh();
            }
        }

        public void HideAds()
        {
            if (_adControl != null)
            {
                _adControl.Visibility = Visibility.Collapsed;
                AdsEnabled = false;
            }
        }

        public void UpdateAdVisibility(bool shouldShowAds)
        {
            if (shouldShowAds)
            {
                ShowAds();
            }
            else
            {
                HideAds();
            }
        }

        public bool ShowInterstitialVideoAd()
        {
            if (IsVideoAdReady && _interstitialAd.State == InterstitialAdState.Ready)
            {
                _interstitialAd.Show();
                return true;
            }
            return false;
        }

        private void OnAdRefreshed(object sender, RoutedEventArgs e)
        {
            // Ad successfully loaded and refreshed
            System.Diagnostics.Debug.WriteLine("Ad refreshed successfully");
            
            // Reset failure counter on success
            _consecutiveAdFailures = 0;
            AdsBlocked = false;
            
            // Raise success event
            AdLoaded?.Invoke(this, EventArgs.Empty);
        }

        private void OnAdErrorOccurred(object sender, Microsoft.Advertising.WinRT.UI.AdErrorEventArgs e)
        {
            // Ad failed to load
            System.Diagnostics.Debug.WriteLine($"Ad error occurred: {e.ErrorMessage} (Code: {e.ErrorCode})");
            
            _consecutiveAdFailures++;
            
            // Check if we've exceeded the threshold for ad blocking detection
            if (_consecutiveAdFailures >= AD_BLOCK_THRESHOLD)
            {
                AdsBlocked = true;
                
                // Determine if this is a network issue or likely ad-blocking
                bool isNetworkIssue = e.ErrorCode == Microsoft.Advertising.WinRT.UI.ErrorCode.NetworkConnectionFailure ||
                                     e.ErrorCode == Microsoft.Advertising.WinRT.UI.ErrorCode.ServerSideError;
                
                var args = new Core.Interfaces.AdBlockedEventArgs
                {
                    Reason = e.ErrorMessage,
                    IsNetworkIssue = isNetworkIssue
                };
                
                AdBlockingDetected?.Invoke(this, args);
            }
            
            // Raise error event
            var errorArgs = new Core.Interfaces.AdErrorEventArgs
            {
                ErrorMessage = e.ErrorMessage,
                ErrorCode = (int)e.ErrorCode
            };
            AdFailed?.Invoke(this, errorArgs);
            
            // Hide the ad control if there's an error
            if (_adControl != null)
            {
                _adControl.Visibility = Visibility.Collapsed;
            }
        }

        private void OnInterstitialAdReady(object sender, object e)
        {
            System.Diagnostics.Debug.WriteLine("Interstitial video ad ready");
            IsVideoAdReady = true;
        }

        private void OnInterstitialAdError(object sender, AdErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Interstitial ad error: {e.ErrorMessage}");
            IsVideoAdReady = false;
            
            // Request a new video ad
            _interstitialAd.RequestAd(AdType.Video, APPLICATION_ID, VIDEO_AD_UNIT_ID);
        }

        private void OnInterstitialAdCompleted(object sender, object e)
        {
            System.Diagnostics.Debug.WriteLine("Interstitial video ad completed");
            IsVideoAdReady = false;
            
            // Raise completed event with 100% watched
            var args = new Core.Interfaces.VideoAdCompletedEventArgs
            {
                WatchedCompletely = true,
                PercentageWatched = 100
            };
            VideoAdCompleted?.Invoke(this, args);
            
            // Request a new video ad for next time
            _interstitialAd.RequestAd(AdType.Video, APPLICATION_ID, VIDEO_AD_UNIT_ID);
        }

        private void OnInterstitialAdCancelled(object sender, object e)
        {
            System.Diagnostics.Debug.WriteLine("Interstitial video ad cancelled");
            IsVideoAdReady = false;
            
            // Raise completed event with partial watch
            var args = new Core.Interfaces.VideoAdCompletedEventArgs
            {
                WatchedCompletely = false,
                PercentageWatched = 50 // Estimate
            };
            VideoAdCompleted?.Invoke(this, args);
            
            // Request a new video ad
            _interstitialAd.RequestAd(AdType.Video, APPLICATION_ID, VIDEO_AD_UNIT_ID);
        }
    }
}
