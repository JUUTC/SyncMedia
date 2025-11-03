using Microsoft.UI.Xaml.Controls;
using SyncMedia.WinUI.ViewModels;

namespace SyncMedia.WinUI.Views;

public sealed partial class AchievementsPage : Page
{
    public AchievementsViewModel ViewModel { get; }

    public AchievementsPage()
    {
        this.InitializeComponent();
        ViewModel = new AchievementsViewModel();
    }
}
