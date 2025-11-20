using SyncMedia.Core.Interfaces;
using System;
using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;

namespace SyncMedia.WinUI.Services
{
    /// <summary>
    /// Service for monitoring network connectivity in WinUI 3
    /// </summary>
    public class ConnectivityService : IConnectivityService
    {
        private bool _isMonitoring;

        public bool IsConnected { get; private set; }

        public event EventHandler<ConnectivityChangedEventArgs> ConnectivityChanged;

        public ConnectivityService()
        {
            IsConnected = CheckConnectivity();
        }

        public bool CheckConnectivity()
        {
            try
            {
                // Check if network is available
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    IsConnected = false;
                    return false;
                }

                // Check Windows connectivity profile
                var profile = NetworkInformation.GetInternetConnectionProfile();
                if (profile == null)
                {
                    IsConnected = false;
                    return false;
                }

                // Check if we have actual internet connectivity
                var level = profile.GetNetworkConnectivityLevel();
                IsConnected = level == NetworkConnectivityLevel.InternetAccess;
                return IsConnected;
            }
            catch
            {
                IsConnected = false;
                return false;
            }
        }

        public void StartMonitoring()
        {
            if (_isMonitoring)
                return;

            // Subscribe to network status change events
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
            _isMonitoring = true;

            // Initial check
            CheckConnectivity();
        }

        public void StopMonitoring()
        {
            if (!_isMonitoring)
                return;

            NetworkInformation.NetworkStatusChanged -= OnNetworkStatusChanged;
            _isMonitoring = false;
        }

        private void OnNetworkStatusChanged(object sender)
        {
            var wasConnected = IsConnected;
            var isNowConnected = CheckConnectivity();

            if (wasConnected != isNowConnected)
            {
                var args = new ConnectivityChangedEventArgs
                {
                    IsConnected = isNowConnected,
                    Reason = isNowConnected ? "Internet connection restored" : "Internet connection lost"
                };

                ConnectivityChanged?.Invoke(this, args);
            }
        }
    }
}
