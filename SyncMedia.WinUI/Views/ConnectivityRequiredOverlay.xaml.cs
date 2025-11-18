using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace SyncMedia.WinUI.Views
{
    public sealed partial class ConnectivityRequiredOverlay : Page
    {
        public event EventHandler RetryRequested;
        public event EventHandler UpgradeRequested;

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

        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateStatus(true, "Checking connection...");
            RetryRequested?.Invoke(this, EventArgs.Empty);
        }

        private void UpgradeButton_Click(object sender, RoutedEventArgs e)
        {
            UpgradeRequested?.Invoke(this, EventArgs.Empty);
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
