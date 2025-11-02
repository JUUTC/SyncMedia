using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SyncMedia.Core;

namespace SyncMedia.WinUI.ViewModels
{
    public partial class NamingListViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<string> namingList = new();

        [ObservableProperty]
        private string newPatternText = string.Empty;

        [ObservableProperty]
        private string? selectedPattern;

        [ObservableProperty]
        private bool showMessage = false;

        [ObservableProperty]
        private string message = string.Empty;

        [ObservableProperty]
        private InfoBarSeverity messageSeverity = InfoBarSeverity.Success;

        [ObservableProperty]
        private bool canAddPattern = false;

        [ObservableProperty]
        private bool hasSelection = false;

        [ObservableProperty]
        private bool hasPatterns = false;

        public NamingListViewModel()
        {
            LoadSettings();

            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(NewPatternText))
                {
                    CanAddPattern = !string.IsNullOrWhiteSpace(NewPatternText);
                }
                else if (e.PropertyName == nameof(SelectedPattern))
                {
                    HasSelection = SelectedPattern != null;
                }
            };

            NamingList.CollectionChanged += (s, e) =>
            {
                HasPatterns = NamingList.Count > 0;
            };
        }

        [RelayCommand]
        private void AddPattern()
        {
            var pattern = NewPatternText.Trim();

            if (string.IsNullOrWhiteSpace(pattern))
                return;

            if (NamingList.Contains(pattern))
            {
                ShowErrorMessage("This pattern already exists");
                return;
            }

            NamingList.Add(pattern);
            NewPatternText = string.Empty;
            SaveSettings();
            ShowSuccessMessage("Pattern added successfully");
        }

        [RelayCommand]
        private void RemoveSelected()
        {
            if (SelectedPattern != null)
            {
                NamingList.Remove(SelectedPattern);
                SaveSettings();
                ShowSuccessMessage("Pattern removed successfully");
            }
        }

        [RelayCommand]
        private async Task ClearAll()
        {
            // In a real implementation, show a confirmation dialog
            NamingList.Clear();
            SaveSettings();
            ShowSuccessMessage("All patterns cleared");
        }

        private void LoadSettings()
        {
            var xmlData = new XmlData();
            
            // Load naming list from XmlData
            // For now, add some example patterns
            NamingList.Add("temp*");
            NamingList.Add("*.tmp");
            NamingList.Add("desktop.ini");
            
            HasPatterns = NamingList.Count > 0;
        }

        private void SaveSettings()
        {
            var xmlData = new XmlData();
            
            // Save naming list to XmlData
            // Convert ObservableCollection to list and save
        }

        private void ShowSuccessMessage(string msg)
        {
            Message = msg;
            MessageSeverity = InfoBarSeverity.Success;
            ShowMessage = true;
            AutoHideMessage();
        }

        private void ShowErrorMessage(string msg)
        {
            Message = msg;
            MessageSeverity = InfoBarSeverity.Error;
            ShowMessage = true;
            AutoHideMessage();
        }

        private async void AutoHideMessage()
        {
            await Task.Delay(3000);
            ShowMessage = false;
        }
    }
}
