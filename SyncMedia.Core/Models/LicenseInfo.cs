using System;

namespace SyncMedia.Core.Models
{
    /// <summary>
    /// Represents license information for the application
    /// </summary>
    public class LicenseInfo
    {
        /// <summary>
        /// Gets or sets whether the user has a Pro license
        /// </summary>
        public bool IsPro { get; set; }

        /// <summary>
        /// Gets or sets the license key
        /// </summary>
        public string LicenseKey { get; set; }

        /// <summary>
        /// Gets or sets the license activation date
        /// </summary>
        public DateTime? ActivationDate { get; set; }

        /// <summary>
        /// Gets or sets the license expiration date (null for lifetime licenses)
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the number of files processed in current period (free tier)
        /// </summary>
        public int FilesProcessedCount { get; set; }

        /// <summary>
        /// Gets or sets the start date of current processing period
        /// </summary>
        public DateTime? PeriodStartDate { get; set; }

        /// <summary>
        /// Gets or sets bonus files earned through ad interactions
        /// </summary>
        public int BonusFilesFromAds { get; set; }

        /// <summary>
        /// Gets or sets the expiration date for ad-earned speed boost
        /// </summary>
        public DateTime? SpeedBoostExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the machine ID (for hardware-bound licenses)
        /// </summary>
        public string MachineId { get; set; }

        /// <summary>
        /// Gets or sets whether this is a store-purchased license (requires online validation)
        /// </summary>
        public bool IsStoreLicense { get; set; }

        /// <summary>
        /// Gets the maximum free files allowed per period (30 days)
        /// Lower limit creates sustainable ad-supported model
        /// </summary>
        public const int FREE_FILES_PER_PERIOD = 25;

        /// <summary>
        /// Gets the number of bonus files earned per video ad watched
        /// Higher reward incentivizes video ad engagement
        /// </summary>
        public const int BONUS_FILES_PER_VIDEO_AD = 50;

        /// <summary>
        /// Gets the number of bonus files earned per ad click-through
        /// Lower than video to prioritize video ad engagement
        /// </summary>
        public const int BONUS_FILES_PER_CLICK = 5;

        /// <summary>
        /// Gets whether the user has reached the free file limit
        /// </summary>
        public bool HasReachedFreeLimit
        {
            get
            {
                if (IsPro) return false;
                return FilesProcessedCount >= (FREE_FILES_PER_PERIOD + BonusFilesFromAds);
            }
        }

        /// <summary>
        /// Gets the number of files remaining in free tier
        /// </summary>
        public int RemainingFreeFiles
        {
            get
            {
                if (IsPro) return int.MaxValue;
                var total = FREE_FILES_PER_PERIOD + BonusFilesFromAds;
                return Math.Max(0, total - FilesProcessedCount);
            }
        }

        /// <summary>
        /// Gets whether the user has an active speed boost from watching ads
        /// </summary>
        public bool HasActiveSpeedBoost
        {
            get
            {
                return SpeedBoostExpirationDate.HasValue && DateTime.Now < SpeedBoostExpirationDate.Value;
            }
        }

        /// <summary>
        /// Gets whether the license is valid
        /// </summary>
        public bool IsValid
        {
            get
            {
                if (IsPro && !string.IsNullOrEmpty(LicenseKey))
                {
                    // Check if license has expired
                    if (ExpirationDate.HasValue && DateTime.Now > ExpirationDate.Value)
                        return false;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Resets the processing period if 30 days have passed
        /// </summary>
        public void CheckAndResetPeriod()
        {
            if (!PeriodStartDate.HasValue || (DateTime.Now - PeriodStartDate.Value).TotalDays >= 30)
            {
                FilesProcessedCount = 0;
                BonusFilesFromAds = 0;
                PeriodStartDate = DateTime.Now;
            }
        }
    }
}
