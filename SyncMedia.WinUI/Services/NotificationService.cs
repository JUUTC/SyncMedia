using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace SyncMedia.WinUI.Services
{
    /// <summary>
    /// Service for showing achievement notifications and in-app messages
    /// </summary>
    public class NotificationService
    {
        private static readonly Lazy<NotificationService> _instance =
            new Lazy<NotificationService>(() => new NotificationService());

        public static NotificationService Instance => _instance.Value;

        private NotificationService()
        {
        }

        /// <summary>
        /// Show an achievement unlock notification
        /// </summary>
        public async Task ShowAchievementAsync(string achievementMessage)
        {
            await ShowInfoBarAsync("🏆 Achievement Unlocked!", achievementMessage, InfoBarSeverity.Success);
        }

        /// <summary>
        /// Show a success notification
        /// </summary>
        public async Task ShowSuccessAsync(string title, string message)
        {
            await ShowInfoBarAsync(title, message, InfoBarSeverity.Success);
        }

        /// <summary>
        /// Show an error notification
        /// </summary>
        public async Task ShowErrorAsync(string title, string message)
        {
            await ShowInfoBarAsync(title, message, InfoBarSeverity.Error);
        }

        /// <summary>
        /// Show a warning notification
        /// </summary>
        public async Task ShowWarningAsync(string title, string message)
        {
            await ShowInfoBarAsync(title, message, InfoBarSeverity.Warning);
        }

        /// <summary>
        /// Show an info notification
        /// </summary>
        public async Task ShowInfoAsync(string title, string message)
        {
            await ShowInfoBarAsync(title, message, InfoBarSeverity.Informational);
        }

        private async Task ShowInfoBarAsync(string title, string message, InfoBarSeverity severity)
        {
            // For now, we'll use a content dialog as a simple notification
            // In a full implementation, we could use TeachingTip or custom notification UI
            await App.MainWindow.DispatcherQueue.EnqueueAsync(async () =>
            {
                var dialog = new ContentDialog
                {
                    Title = title,
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = App.MainWindow.Content.XamlRoot
                };

                await dialog.ShowAsync();
            });
        }
    }

    /// <summary>
    /// Extension methods for DispatcherQueue to support async operations
    /// </summary>
    public static class DispatcherQueueExtensions
    {
        public static Task EnqueueAsync(this Microsoft.UI.Dispatching.DispatcherQueue dispatcher, Func<Task> action)
        {
            var tcs = new TaskCompletionSource<bool>();

            dispatcher.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, async () =>
            {
                try
                {
                    await action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }
    }
}
