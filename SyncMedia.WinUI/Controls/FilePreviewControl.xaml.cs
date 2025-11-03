using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Storage;

namespace SyncMedia.WinUI.Controls
{
    public sealed partial class FilePreviewControl : UserControl
    {
        private DispatcherTimer _imageTimer;
        private DispatcherTimer _videoTimer;
        private bool _isPreviewEnabled = true;

        // File type icon glyphs (Segoe MDL2 Assets)
        private const string MusicNoteGlyph = "\uEC4F";
        private const string DocumentGlyph = "\uE8A5";
        private const string SpreadsheetGlyph = "\uE9F9";

        // Media extension sets
        private static readonly string[] ImageExtensions = new[]
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff",
            ".webp", ".heic", ".heif", ".avif"
        };

        private static readonly string[] VideoExtensions = new[]
        {
            ".mp4", ".mov", ".wmv", ".avi", ".m4v", ".mpg", ".mpeg",
            ".webm", ".mkv", ".flv"
        };

        public FilePreviewControl()
        {
            this.InitializeComponent();
            InitializeTimers();
        }

        private void InitializeTimers()
        {
            // Image timer: 3 seconds
            _imageTimer = new DispatcherTimer();
            _imageTimer.Interval = TimeSpan.FromSeconds(3);
            _imageTimer.Tick += (s, e) => ClearPreview();

            // Video timer: 10 seconds
            _videoTimer = new DispatcherTimer();
            _videoTimer.Interval = TimeSpan.FromSeconds(10);
            _videoTimer.Tick += (s, e) => ClearPreview();
        }

        public bool IsPreviewEnabled
        {
            get => _isPreviewEnabled;
            set
            {
                _isPreviewEnabled = value;
                if (!value)
                {
                    ClearPreview();
                }
            }
        }

        public async Task ShowPreviewAsync(string filePath)
        {
            if (!_isPreviewEnabled || string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                ClearPreview();
                return;
            }

            try
            {
                var extension = Path.GetExtension(filePath).ToLowerInvariant();

                // Check if it's an image
                if (Array.Exists(ImageExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    await ShowImagePreviewAsync(filePath);
                }
                // Check if it's a video
                else if (Array.Exists(VideoExtensions, ext => ext.Equals(extension, StringComparison.OrdinalIgnoreCase)))
                {
                    await ShowVideoPreviewAsync(filePath);
                }
                else
                {
                    // Show file type indicator for other files
                    ShowFileTypeIndicator(extension);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Preview failed: {ex.Message}");
                ClearPreview();
            }
        }

        private async Task ShowImagePreviewAsync(string filePath)
        {
            ClearPreview();

            try
            {
                var bitmap = new BitmapImage();
                var file = await StorageFile.GetFileFromPathAsync(filePath);
                using var stream = await file.OpenAsync(FileAccessMode.Read);
                await bitmap.SetSourceAsync(stream);

                ImagePreview.Source = bitmap;
                ImagePreview.Visibility = Visibility.Visible;
                PlaceholderText.Visibility = Visibility.Collapsed;

                // Start 3-second timer
                _imageTimer.Stop();
                _imageTimer.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Image preview failed: {ex.Message}");
                ClearPreview();
            }
        }

        private async Task ShowVideoPreviewAsync(string filePath)
        {
            ClearPreview();

            try
            {
                var file = await StorageFile.GetFileFromPathAsync(filePath);
                var mediaSource = MediaSource.CreateFromStorageFile(file);

                var mediaPlayer = new Windows.Media.Playback.MediaPlayer();
                mediaPlayer.Source = mediaSource;
                mediaPlayer.IsMuted = true; // Mute for preview
                mediaPlayer.IsLoopingEnabled = true;

                VideoPreview.SetMediaPlayer(mediaPlayer);
                VideoPreview.Visibility = Visibility.Visible;
                PlaceholderText.Visibility = Visibility.Collapsed;

                mediaPlayer.Play();

                // Start 10-second timer
                _videoTimer.Stop();
                _videoTimer.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Video preview failed: {ex.Message}");
                ClearPreview();
            }
        }

        private void ShowFileTypeIndicator(string extension)
        {
            ClearPreview();

            // Set icon based on file type
            string glyph = extension.ToLowerInvariant() switch
            {
                ".mp3" or ".wav" or ".flac" or ".aac" => MusicNoteGlyph,
                ".pdf" => DocumentGlyph,
                ".doc" or ".docx" => DocumentGlyph,
                ".xls" or ".xlsx" => SpreadsheetGlyph,
                _ => DocumentGlyph
            };

            FileTypeIcon.Glyph = glyph;
            FileTypeText.Text = $"{extension.ToUpperInvariant()} file";
            FileTypeIndicator.Visibility = Visibility.Visible;
            PlaceholderText.Visibility = Visibility.Collapsed;

            // Show for 3 seconds
            _imageTimer.Stop();
            _imageTimer.Start();
        }

        private void ClearPreview()
        {
            // Stop timers
            _imageTimer?.Stop();
            _videoTimer?.Stop();

            // Clear image preview
            ImagePreview.Source = null;
            ImagePreview.Visibility = Visibility.Collapsed;

            // Clear video preview
            var mediaPlayer = VideoPreview.MediaPlayer;
            if (mediaPlayer != null)
            {
                mediaPlayer.Pause();
                mediaPlayer.Source = null;
                VideoPreview.SetMediaPlayer(null);
                mediaPlayer.Dispose();
            }
            VideoPreview.Visibility = Visibility.Collapsed;

            // Clear file type indicator
            FileTypeIndicator.Visibility = Visibility.Collapsed;

            // Show placeholder
            PlaceholderText.Visibility = Visibility.Visible;
        }

        // Cleanup when control is unloaded
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ClearPreview();
            _imageTimer?.Stop();
            _videoTimer?.Stop();
        }
    }
}
