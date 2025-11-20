using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SyncMedia.WinUI.Views
{
    public sealed partial class ConnectivityRequiredOverlay : Page
    {
        public event EventHandler RetryRequested;
        public event EventHandler UpgradeRequested;
        public event EventHandler WatchAdRequested;

        public ConnectivityRequiredOverlay()
        {
            this.InitializeComponent();
        }

        public void UpdateStatus(bool isChecking, string statusMessage)
        {
            CheckingProgressRing.IsActive = isChecking;
            StatusText.Text = statusMessage;
        }

        public void ShowAdBlockedMessage()
        {
            MessageText.Text = "Ad blocking software detected.\n\n" +
                              "The free version of SyncMedia requires ads to be displayed. " +
                              "Please disable your ad blocker for this application, or upgrade to Pro for an ad-free experience.";
        }

        public void ShowOfflineMessage()
        {
            MessageText.Text = "The free version of SyncMedia requires an active internet connection to display advertisements.\n\n" +
                              "Please connect to the internet to continue using SyncMedia, or upgrade to Pro for offline access.";
        }

        public void ShowFileLimitReachedMessage(int remaining, int bonusAvailable)
        {
            if (remaining <= 0)
            {
                MessageText.Text = $"Free file limit reached!\n\n" +
                                  $"You've processed all your free files for this period. " +
                                  $"Watch a video ad to earn +20 more files, or upgrade to Pro for unlimited file processing.";
                
                // Show watch ad button
                if (WatchAdButton != null)
                {
                    WatchAdButton.Visibility = Visibility.Visible;
                }
            }
            else
            {
                MessageText.Text = $"Only {remaining} files remaining in free tier.\n\n" +
                                  $"You can earn bonus files by watching video ads (+20 per video) " +
                                  $"or upgrade to Pro for unlimited file processing.";
                
                // Show watch ad button
                if (WatchAdButton != null)
                {
                    WatchAdButton.Visibility = Visibility.Visible;
                }
            }
        }

        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus(true, "Checking connection...");
            RetryRequested?.Invoke(this, EventArgs.Empty);
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            UpgradeRequested?.Invoke(this, EventArgs.Empty);
        }

        private void WatchAdButton_Click(object sender, RoutedEventArgs e)
        {
            WatchAdRequested?.Invoke(this, EventArgs.Empty);
        }

        private void MoreInfoButton_Click(object sender, RoutedEventArgs e)
        {
            // Toggle info panel visibility
            InfoPanel.Visibility = InfoPanel.Visibility == Visibility.Collapsed 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }
    }
}
