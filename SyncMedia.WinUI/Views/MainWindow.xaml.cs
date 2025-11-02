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

    // Placeholder pages - to be implemented
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            var root = new Grid();
            var stack = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 16
            };

            var title = new TextBlock
            {
                Text = "Welcome to SyncMedia",
                Style = Application.Current.Resources["TitleTextBlockStyle"] as Style
            };

            var subtitle = new TextBlock
            {
                Text = "Modern media synchronization with AI-powered duplicate detection",
                Style = Application.Current.Resources["SubtitleTextBlockStyle"] as Style,
                Opacity = 0.7
            };

            stack.Children.Add(title);
            stack.Children.Add(subtitle);
            root.Children.Add(stack);
            this.Content = root;
        }
    }

    public sealed partial class SyncPage : Page
    {
        public SyncPage()
        {
            var text = new TextBlock
            {
                Text = "Sync Page - Coming in Week 3",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            this.Content = text;
        }
    }

    public sealed partial class FilesPage : Page
    {
        public FilesPage()
        {
            var text = new TextBlock
            {
                Text = "Files Page - Coming in Week 3",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            this.Content = text;
        }
    }

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
