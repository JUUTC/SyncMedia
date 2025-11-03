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
        /// Gets or sets the trial expiration date (14 days from first launch)
        /// </summary>
        public DateTime? TrialExpirationDate { get; set; }

        /// <summary>
        /// Gets whether the user is currently in a trial period
        /// </summary>
        public bool IsInTrial => TrialExpirationDate.HasValue && DateTime.Now < TrialExpirationDate.Value;

        /// <summary>
        /// Gets whether the trial has expired
        /// </summary>
        public bool IsTrialExpired => TrialExpirationDate.HasValue && DateTime.Now >= TrialExpirationDate.Value;

        /// <summary>
        /// Gets the number of days remaining in the trial
        /// </summary>
        public int TrialDaysRemaining
        {
            get
            {
                if (!TrialExpirationDate.HasValue) return 0;
                var remaining = (TrialExpirationDate.Value - DateTime.Now).Days;
                return Math.Max(0, remaining);
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
    }
}
