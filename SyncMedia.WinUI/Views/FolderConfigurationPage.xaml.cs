using Microsoft.UI.Xaml.Controls;
using SyncMedia.WinUI.ViewModels;

namespace SyncMedia.WinUI.Views;

public sealed partial class FolderConfigurationPage : Page
{
    public FolderConfigurationViewModel ViewModel { get; }

    public FolderConfigurationPage()
    {
        ViewModel = App.GetService<FolderConfigurationViewModel>();
        this.InitializeComponent();
    }
}
