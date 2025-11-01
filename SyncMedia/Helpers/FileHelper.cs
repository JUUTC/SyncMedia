using System;
using System.IO;
using System.Text.RegularExpressions;
using SyncMedia.Constants;

namespace SyncMedia.Helpers
{
    /// <summary>
    /// Helper class for file and folder operations
    /// </summary>
    public static class FileHelper
    {
        private static readonly Regex DateRegex = new Regex(":");
        
        /// <summary>
        /// Check if a file is an image based on its extension
        /// </summary>
        public static bool IsImageFile(string extension)
        {
            return MediaConstants.ImageExtensions.Contains(extension);
        }
        
        /// <summary>
        /// Check if a file is a video based on its extension
        /// </summary>
        public static bool IsVideoFile(string extension)
        {
            return MediaConstants.VideoExtensions.Contains(extension);
        }
        
        /// <summary>
        /// Check if a file is a supported media file
        /// </summary>
        public static bool IsMediaFile(string extension)
        {
            return MediaConstants.AllMediaExtensions.Contains(extension);
        }
        
        /// <summary>
        /// Clean file name by removing dates, extensions, and special characters
        /// </summary>
        public static string CleanFileName(string filename)
        {
            // Optimized: Remove duplicates and convert to lower case once
            string nodate = Regex.Replace(filename, @"[\d-]", string.Empty);
            nodate = nodate.ToLower();
            
            // Remove all file extensions in one pass (classic + modern formats)
            // Classic image formats
            nodate = nodate.Replace(".mp", string.Empty);
            nodate = nodate.Replace(".jpg", string.Empty);
            nodate = nodate.Replace(".jpeg", string.Empty);
            nodate = nodate.Replace(".jepg", string.Empty);
            nodate = nodate.Replace(".png", string.Empty);
            nodate = nodate.Replace(".bmp", string.Empty);
            nodate = nodate.Replace(".gif", string.Empty);
            nodate = nodate.Replace(".tif", string.Empty);
            nodate = nodate.Replace(".tiff", string.Empty);
            // Modern image formats
            nodate = nodate.Replace(".webp", string.Empty);
            nodate = nodate.Replace(".heic", string.Empty);
            nodate = nodate.Replace(".heif", string.Empty);
            nodate = nodate.Replace(".avif", string.Empty);
            nodate = nodate.Replace(".jxl", string.Empty);
            // Classic video formats
            nodate = nodate.Replace(".mov", string.Empty);
            nodate = nodate.Replace(".mp4", string.Empty);
            nodate = nodate.Replace(".wmv", string.Empty);
            nodate = nodate.Replace(".avi", string.Empty);
            nodate = nodate.Replace(".m4v", string.Empty);
            nodate = nodate.Replace(".mpg", string.Empty);
            nodate = nodate.Replace(".mpeg", string.Empty);
            // Modern video formats
            nodate = nodate.Replace(".webm", string.Empty);
            nodate = nodate.Replace(".mkv", string.Empty);
            nodate = nodate.Replace(".flv", string.Empty);
            nodate = nodate.Replace(".ts", string.Empty);
            nodate = nodate.Replace(".mts", string.Empty);
            nodate = nodate.Replace(".3gp", string.Empty);
            nodate = nodate.Replace(".3g2", string.Empty);
            nodate = nodate.Replace(".ogv", string.Empty);
            nodate = nodate.Replace(".vob", string.Empty);
            // Special characters
            nodate = nodate.Replace(",", string.Empty);
            nodate = nodate.Replace(".", string.Empty);
            nodate = nodate.Replace("/", string.Empty);
            
            return nodate;
        }
        
        /// <summary>
        /// Remove folder path patterns from filename
        /// </summary>
        public static string RemoveFolderPath(string filename)
        {
            return Regex.Replace(filename, "\\b(?<year>\\d{2,4})/(?<month>\\d{1,2})/(?<day>\\d{2,4})\\b", string.Empty);
        }
        
        /// <summary>
        /// Remove folder structure from file path
        /// </summary>
        public static string RemoveFolderStructure(string fullPath, string nodate)
        {
            if (nodate.Contains(@"\\"))
            {
                nodate = nodate.TrimStart('\\');
            }

            if (nodate.Contains(@"\"))
            {
                nodate = nodate.TrimStart('\\');
            }
            
            nodate = RemoveFolderPath(fullPath);
            
            if (nodate.Contains(@"\"))
            {
                nodate = Regex.Replace(nodate, "^[^_]*\\\\", string.Empty);
            }

            return nodate;
        }
        
        /// <summary>
        /// Format date time for file naming
        /// </summary>
        public static string FormatFileDateTime(DateTime dateTime)
        {
            return dateTime.Year.ToString("0000") + "-" + dateTime.Month.ToString("00") + "-" + dateTime.Day.ToString("00");
        }
        
        /// <summary>
        /// Validate if a folder path exists and is valid
        /// </summary>
        public static bool ValidateFolderPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;
            
            return Directory.Exists(path);
        }
    }
}
