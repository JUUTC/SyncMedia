using System;

namespace SyncMedia.Models
{
    /// <summary>
    /// Statistics for a sync session
    /// </summary>
    public class SyncStatistics
    {
        public int TotalFilesProcessed { get; set; }
        public int DuplicatesFound { get; set; }
        public int ErrorsEncountered { get; set; }
        public long TotalBytesProcessed { get; set; }
        public DateTime SyncStartTime { get; set; }
        public DateTime SyncEndTime { get; set; }
        public TimeSpan ElapsedTime => SyncEndTime - SyncStartTime;
        
        public double TotalMBProcessed => TotalBytesProcessed / (1024.0 * 1024.0);
        public double ProcessingSpeedMBPerSecond => ElapsedTime.TotalSeconds > 0 
            ? TotalMBProcessed / ElapsedTime.TotalSeconds 
            : 0;
        public double FilesPerMinute => ElapsedTime.TotalMinutes > 0 
            ? TotalFilesProcessed / ElapsedTime.TotalMinutes 
            : 0;
        
        public void Reset()
        {
            TotalFilesProcessed = 0;
            DuplicatesFound = 0;
            ErrorsEncountered = 0;
            TotalBytesProcessed = 0;
            SyncStartTime = DateTime.Now;
            SyncEndTime = DateTime.Now;
        }
    }
}
