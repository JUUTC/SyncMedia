using System;
using System.Collections.Generic;

namespace SyncMedia.Constants
{
    /// <summary>
    /// Constants for media file processing, including supported formats and configuration values
    /// </summary>
    public static class MediaConstants
    {
        // Optimized: Use HashSet for O(1) lookup instead of multiple string comparisons
        // Updated: Added modern formats (WebP, HEIC, AVIF, JPEG XL for images; WebM, MKV, etc. for videos)
        public static readonly HashSet<string> ImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Classic formats
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff",
            // Modern formats (2010+)
            ".webp",        // Google WebP (2010)
            ".heic", ".heif", // Apple HEIC/HEIF (2015)
            ".avif",        // AV1 Image Format (2019)
            ".jxl"          // JPEG XL (2021)
        };
        
        public static readonly HashSet<string> VideoExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Classic formats
            ".mov", ".mp4", ".wmv", ".avi", ".m4v", ".mpg", ".mpeg",
            // Modern/additional formats (2010+)
            ".webm",        // Google WebM (2010)
            ".mkv",         // Matroska (popular 2010+)
            ".flv",         // Flash Video
            ".ts", ".mts",  // MPEG Transport Stream
            ".3gp", ".3g2", // Mobile formats
            ".ogv",         // Ogg Video
            ".vob"          // DVD Video Object
        };
        
        public static readonly HashSet<string> AllMediaExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Images
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff",
            ".webp", ".heic", ".heif", ".avif", ".jxl",
            // Videos
            ".mov", ".mp4", ".wmv", ".avi", ".m4v", ".mpg", ".mpeg",
            ".webm", ".mkv", ".flv", ".ts", ".mts", ".3gp", ".3g2", ".ogv", ".vob"
        };
        
        // UI Update batching configuration
        public const int UI_UPDATE_BATCH_SIZE = 10;
        
        // Points system configuration
        public const int POINTS_PER_FILE = 10;
        public const int POINTS_PER_DUPLICATE = 5;
        public const int POINTS_PER_MB = 1;
        
        // Speed bonus thresholds
        public const double SPEED_QUICK_THRESHOLD = 5.0;
        public const int SPEED_QUICK_BONUS = 50;
        public const double SPEED_DEMON_THRESHOLD = 10.0;
        public const int SPEED_DEMON_BONUS = 100;
        public const double SPEED_SUPER_THRESHOLD = 25.0;
        public const int SPEED_SUPER_BONUS = 250;
        public const double SPEED_LIGHTNING_THRESHOLD = 50.0;
        public const int SPEED_LIGHTNING_BONUS = 500;
    }
}
