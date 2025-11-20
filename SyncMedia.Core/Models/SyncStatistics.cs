using System;

namespace SyncMedia.Core.Models
{
    /// <summary>
    /// Statistics for a sync session
    /// </summary>
    public class SyncStatistics
    {
        public int TotalFiles { get; set; }
        public int ProcessedFiles { get; set; }
        public int SuccessfulFiles { get; set; }
        public int SkippedFiles { get; set; }
        public int ErrorFiles { get; set; }
        public int DuplicatesFound { get; set; }
        public long TotalBytesProcessed { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        
        // Legacy properties for backward compatibility
        public int TotalFilesProcessed
        {
            get => ProcessedFiles;
            set => ProcessedFiles = value;
        }
        
        public int ErrorsEncountered
        {
            get => ErrorFiles;
            set => ErrorFiles = value;
        }
        
        public DateTime SyncStartTime
        {
            get => StartTime;
            set => StartTime = value;
        }
        
        public DateTime SyncEndTime
        {
            get => EndTime;
            set => EndTime = value;
        }
        
        public TimeSpan ElapsedTime => Duration > TimeSpan.Zero ? Duration : EndTime - StartTime;
        
        public double TotalMBProcessed => TotalBytesProcessed / (1024.0 * 1024.0);
        public double ProcessingSpeedMBPerSecond => ElapsedTime.TotalSeconds > 0 
            ? TotalMBProcessed / ElapsedTime.TotalSeconds 
            : 0;
        public double FilesPerMinute => ElapsedTime.TotalMinutes > 0 
            ? ProcessedFiles / ElapsedTime.TotalMinutes 
            : 0;
        
        public void Reset()
        {
            TotalFiles = 0;
            ProcessedFiles = 0;
            SuccessfulFiles = 0;
            SkippedFiles = 0;
            ErrorFiles = 0;
            DuplicatesFound = 0;
            TotalBytesProcessed = 0;
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            Duration = TimeSpan.Zero;
            Status = string.Empty;
            ErrorMessage = string.Empty;
        }
    }
}
