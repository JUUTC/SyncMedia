using Microsoft.UI.Xaml.Controls;
using SyncMedia.WinUI.ViewModels;

namespace SyncMedia.WinUI.Views;

public sealed partial class SettingsPage : Page
{
    public SettingsViewModel ViewModel { get; }

    public SettingsPage()
    {
        ViewModel = App.GetService<SettingsViewModel>();
        this.InitializeComponent();
    }
}
