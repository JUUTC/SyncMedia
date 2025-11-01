using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SyncMedia.Helpers
{
    /// <summary>
    /// Manages file preview display during sync operations
    /// Shows images for 3 seconds and plays videos for 10 seconds
    /// </summary>
    public class FilePreviewHelper : IDisposable
    {
        private Panel _previewPanel;
        private PictureBox _previewPictureBox;
        private WebBrowser _videoPlayer;
        private System.Windows.Forms.Timer _previewTimer;
        private bool _isPreviewEnabled;
        private bool _isShowingVideo;
        private const int IMAGE_PREVIEW_DURATION = 3000; // 3 seconds
        private const int VIDEO_PREVIEW_DURATION = 10000; // 10 seconds
        
        public bool IsPreviewEnabled
        {
            get => _isPreviewEnabled;
            set
            {
                _isPreviewEnabled = value;
                if (!_isPreviewEnabled)
                {
                    ClearPreview();
                }
            }
        }
        
        public FilePreviewHelper(Panel previewPanel)
        {
            _previewPanel = previewPanel ?? throw new ArgumentNullException(nameof(previewPanel));
            
            // Create PictureBox for images
            _previewPictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.Black,
                Visible = true
            };
            _previewPanel.Controls.Add(_previewPictureBox);
            
            // Create WebBrowser for videos
            _videoPlayer = new WebBrowser
            {
                Dock = DockStyle.Fill,
                ScriptErrorsSuppressed = true,
                ScrollBarsEnabled = false,
                Visible = false
            };
            _previewPanel.Controls.Add(_videoPlayer);
            
            _previewTimer = new System.Windows.Forms.Timer();
            _previewTimer.Tick += PreviewTimer_Tick;
            _isPreviewEnabled = false;
            _isShowingVideo = false;
        }
        
        /// <summary>
        /// Show preview of a file (image or video playback)
        /// </summary>
        public void ShowPreview(string filePath, bool isVideo)
        {
            if (!_isPreviewEnabled || string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                return;
            }
            
            try
            {
                // Stop existing timer
                _previewTimer.Stop();
                
                if (isVideo)
                {
                    // Play video
                    ShowVideo(filePath);
                    _previewTimer.Interval = VIDEO_PREVIEW_DURATION;
                    _isShowingVideo = true;
                }
                else
                {
                    // Show image
                    ShowImage(filePath);
                    _previewTimer.Interval = IMAGE_PREVIEW_DURATION;
                    _isShowingVideo = false;
                }
                
                // Start timer to clear preview after duration
                _previewTimer.Start();
            }
            catch (Exception ex)
            {
                // If preview fails, just clear it and continue
                System.Diagnostics.Debug.WriteLine($"Preview error: {ex.Message}");
                ClearPreview();
            }
        }
        
        private void ShowImage(string imagePath)
        {
            try
            {
                // Hide video player, show picture box
                _videoPlayer.Visible = false;
                _previewPictureBox.Visible = true;
                
                // Dispose previous image
                if (_previewPictureBox.Image != null)
                {
                    var oldImage = _previewPictureBox.Image;
                    _previewPictureBox.Image = null;
                    oldImage.Dispose();
                }
                
                // Load image from file
                using (var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    var image = Image.FromStream(fileStream);
                    
                    // Resize to fit PictureBox while maintaining aspect ratio
                    var resized = ResizeImage(image, _previewPictureBox.Width, _previewPictureBox.Height);
                    image.Dispose();
                    
                    _previewPictureBox.Image = resized;
                }
            }
            catch
            {
                // If image loading fails, show placeholder
                ShowPlaceholder("Image preview unavailable");
            }
        }
        
        private void ShowVideo(string videoPath)
        {
            try
            {
                // Hide picture box, show video player
                _previewPictureBox.Visible = false;
                _videoPlayer.Visible = true;
                
                // Create HTML5 video player
                string videoUrl = "file:///" + videoPath.Replace("\\", "/");
                string html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ margin: 0; padding: 0; background-color: black; overflow: hidden; }}
        video {{ width: 100%; height: 100%; object-fit: contain; }}
    </style>
</head>
<body>
    <video autoplay muted loop>
        <source src=""{videoUrl}"" type=""video/mp4"">
        <source src=""{videoUrl}"" type=""video/webm"">
        <source src=""{videoUrl}"" type=""video/ogg"">
        Your browser does not support the video tag.
    </video>
</body>
</html>";
                
                _videoPlayer.DocumentText = html;
            }
            catch
            {
                // If video loading fails, show placeholder in PictureBox
                _videoPlayer.Visible = false;
                _previewPictureBox.Visible = true;
                ShowPlaceholder("Video preview unavailable");
            }
        }
        
        private void ShowPlaceholder(string message)
        {
            _videoPlayer.Visible = false;
            _previewPictureBox.Visible = true;
            
            if (_previewPictureBox.Image != null)
            {
                var oldImage = _previewPictureBox.Image;
                _previewPictureBox.Image = null;
                oldImage.Dispose();
            }
            
            var bitmap = new Bitmap(_previewPanel.Width, _previewPanel.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(Brushes.LightGray, 0, 0, bitmap.Width, bitmap.Height);
                using (var font = new Font("Arial", 10))
                {
                    var textSize = g.MeasureString(message, font);
                    g.DrawString(message, font, Brushes.DarkGray,
                        (bitmap.Width - textSize.Width) / 2,
                        (bitmap.Height - textSize.Height) / 2);
                }
            }
            _previewPictureBox.Image = bitmap;
        }
        
        private Image ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            // Calculate scaling factor while maintaining aspect ratio
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);
            
            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);
            
            var resized = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(resized))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            
            return resized;
        }
        
        private void PreviewTimer_Tick(object sender, EventArgs e)
        {
            _previewTimer.Stop();
            ClearPreview();
        }
        
        public void ClearPreview()
        {
            _previewTimer.Stop();
            
            // Clear image
            if (_previewPictureBox.Image != null)
            {
                var oldImage = _previewPictureBox.Image;
                _previewPictureBox.Image = null;
                oldImage.Dispose();
            }
            
            // Clear video
            if (_isShowingVideo && _videoPlayer.Visible)
            {
                _videoPlayer.DocumentText = "<html><body style='background-color:black;'></body></html>";
                _isShowingVideo = false;
            }
            
            // Show picture box by default
            _videoPlayer.Visible = false;
            _previewPictureBox.Visible = true;
        }
        
        public void Dispose()
        {
            _previewTimer?.Stop();
            _previewTimer?.Dispose();
            ClearPreview();
            _videoPlayer?.Dispose();
        }
    }
}
