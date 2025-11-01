using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SyncMedia.Helpers
{
    /// <summary>
    /// Manages file preview display during sync operations
    /// Shows images for 3 seconds and videos for 10 seconds
    /// </summary>
    public class FilePreviewHelper : IDisposable
    {
        private PictureBox _previewPictureBox;
        private System.Windows.Forms.Timer _previewTimer;
        private bool _isPreviewEnabled;
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
        
        public FilePreviewHelper(PictureBox previewPictureBox)
        {
            _previewPictureBox = previewPictureBox ?? throw new ArgumentNullException(nameof(previewPictureBox));
            _previewTimer = new System.Windows.Forms.Timer();
            _previewTimer.Tick += PreviewTimer_Tick;
            _isPreviewEnabled = false;
        }
        
        /// <summary>
        /// Show preview of a file (image or video thumbnail)
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
                
                // Dispose previous image
                if (_previewPictureBox.Image != null)
                {
                    var oldImage = _previewPictureBox.Image;
                    _previewPictureBox.Image = null;
                    oldImage.Dispose();
                }
                
                if (isVideo)
                {
                    // For videos, show a placeholder with file info
                    ShowVideoPlaceholder(filePath);
                    _previewTimer.Interval = VIDEO_PREVIEW_DURATION;
                }
                else
                {
                    // For images, load and display the image
                    ShowImage(filePath);
                    _previewTimer.Interval = IMAGE_PREVIEW_DURATION;
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
                // Load image from file
                using (var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    var image = Image.FromStream(fileStream);
                    
                    // Resize to fit PictureBox while maintaining aspect ratio
                    var resized = ResizeImage(image, _previewPictureBox.Width, _previewPictureBox.Height);
                    image.Dispose();
                    
                    _previewPictureBox.Image = resized;
                    _previewPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
                }
            }
            catch
            {
                // If image loading fails, show placeholder
                ShowPlaceholder("Image preview unavailable");
            }
        }
        
        private void ShowVideoPlaceholder(string videoPath)
        {
            try
            {
                // Create a simple placeholder bitmap for videos
                var bitmap = new Bitmap(_previewPictureBox.Width, _previewPictureBox.Height);
                using (var g = Graphics.FromImage(bitmap))
                {
                    // Draw background
                    g.FillRectangle(Brushes.Black, 0, 0, bitmap.Width, bitmap.Height);
                    
                    // Draw video icon (simple triangle play button)
                    var centerX = bitmap.Width / 2;
                    var centerY = bitmap.Height / 2;
                    var size = Math.Min(bitmap.Width, bitmap.Height) / 4;
                    
                    Point[] triangle = new Point[]
                    {
                        new Point(centerX - size/2, centerY - size/2),
                        new Point(centerX - size/2, centerY + size/2),
                        new Point(centerX + size/2, centerY)
                    };
                    g.FillPolygon(Brushes.White, triangle);
                    
                    // Draw file name
                    string fileName = Path.GetFileName(videoPath);
                    if (fileName.Length > 30) fileName = fileName.Substring(0, 27) + "...";
                    
                    using (var font = new Font("Arial", 10))
                    {
                        var textSize = g.MeasureString(fileName, font);
                        g.DrawString(fileName, font, Brushes.White, 
                            (bitmap.Width - textSize.Width) / 2, 
                            bitmap.Height - textSize.Height - 10);
                    }
                }
                
                _previewPictureBox.Image = bitmap;
                _previewPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            catch
            {
                ShowPlaceholder("Video preview unavailable");
            }
        }
        
        private void ShowPlaceholder(string message)
        {
            var bitmap = new Bitmap(_previewPictureBox.Width, _previewPictureBox.Height);
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
            if (_previewPictureBox.Image != null)
            {
                var oldImage = _previewPictureBox.Image;
                _previewPictureBox.Image = null;
                oldImage.Dispose();
            }
        }
        
        public void Dispose()
        {
            _previewTimer?.Stop();
            _previewTimer?.Dispose();
            ClearPreview();
        }
    }
}
