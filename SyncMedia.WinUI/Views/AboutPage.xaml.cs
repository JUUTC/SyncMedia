using Microsoft.UI.Xaml.Controls;

namespace SyncMedia.WinUI.Views;

public sealed partial class AboutPage : Page
{
    public AboutViewModel ViewModel { get; }

    public AboutPage()
    {
        ViewModel = new AboutViewModel();
        this.InitializeComponent();
    }
}
