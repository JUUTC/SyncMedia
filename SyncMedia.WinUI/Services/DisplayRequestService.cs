using SyncMedia.Core.Interfaces;
using System;
using Windows.System.Display;

namespace SyncMedia.WinUI.Services
{
    /// <summary>
    /// WinUI implementation of display request service to prevent screen sleep
    /// </summary>
    public class DisplayRequestService : IDisplayRequestService
    {
        private DisplayRequest _displayRequest;
        private int _requestCount;
        private readonly object _lock = new object();

        public bool IsActive => _requestCount > 0;

        /// <summary>
        /// Request to keep the display active
        /// </summary>
        public void RequestActive()
        {
            lock (_lock)
            {
                try
                {
                    if (_displayRequest == null)
                    {
                        _displayRequest = new DisplayRequest();
                    }

                    if (_requestCount == 0)
                    {
                        _displayRequest.RequestActive();
                    }

                    _requestCount++;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DisplayRequest.RequestActive failed: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Release the display request
        /// </summary>
        public void RequestRelease()
        {
            lock (_lock)
            {
                try
                {
                    if (_requestCount > 0)
                    {
                        _requestCount--;

                        if (_requestCount == 0 && _displayRequest != null)
                        {
                            _displayRequest.RequestRelease();
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"DisplayRequest.RequestRelease failed: {ex.Message}");
                }
            }
        }
    }
}
