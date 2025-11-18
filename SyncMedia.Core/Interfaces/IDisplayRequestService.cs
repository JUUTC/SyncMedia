using System;

namespace SyncMedia.Core.Interfaces
{
    /// <summary>
    /// Service for managing display requests to prevent screen sleep
    /// </summary>
    public interface IDisplayRequestService
    {
        /// <summary>
        /// Request to keep the display active
        /// Call this before starting long-running operations or video playback
        /// </summary>
        void RequestActive();

        /// <summary>
        /// Release the display request
        /// Call this when operation completes or is cancelled
        /// </summary>
        void RequestRelease();

        /// <summary>
        /// Gets whether a display request is currently active
        /// </summary>
        bool IsActive { get; }
    }
}
