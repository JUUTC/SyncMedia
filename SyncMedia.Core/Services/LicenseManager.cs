using SyncMedia.Core.Models;
using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace SyncMedia.Core.Services
{
    /// <summary>
    /// Service for managing license validation and activation
    /// </summary>
    public class LicenseManager
    {
        private const string LICENSE_FILE = "license.xml";
        private const string ENCRYPTION_KEY = "SyncMedia2024SecureKeyV1"; // In production, use secure key management
        private readonly string _licenseFilePath;
        private LicenseInfo _currentLicense;
        private readonly string _machineId;

        public LicenseManager()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SyncMedia");
            Directory.CreateDirectory(appDataPath);
            _licenseFilePath = Path.Combine(appDataPath, LICENSE_FILE);
            _machineId = GetMachineId();
            LoadLicense();
        }

        /// <summary>
        /// Constructor for testing with custom license file path
        /// </summary>
        /// <param name="customLicenseFilePath">Custom path for license file (for testing)</param>
        internal LicenseManager(string customLicenseFilePath)
        {
            var directory = Path.GetDirectoryName(customLicenseFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
            _licenseFilePath = customLicenseFilePath;
            _machineId = GetMachineId();
            LoadLicense();
        }

        /// <summary>
        /// Gets the current license information
        /// </summary>
        public LicenseInfo CurrentLicense => _currentLicense;

        /// <summary>
        /// Load license from file or initialize new free tier license
        /// </summary>
        private void LoadLicense()
        {
            if (File.Exists(_licenseFilePath))
            {
                try
                {
                    var doc = XDocument.Load(_licenseFilePath);
                    var root = doc.Element("License");
                    
                    _currentLicense = new LicenseInfo
                    {
                        IsPro = bool.Parse(root.Element("IsPro")?.Value ?? "false"),
                        LicenseKey = root.Element("LicenseKey")?.Value,
                        ActivationDate = ParseDate(root.Element("ActivationDate")?.Value),
                        ExpirationDate = ParseDate(root.Element("ExpirationDate")?.Value),
                        FilesProcessedCount = int.Parse(root.Element("FilesProcessedCount")?.Value ?? "0"),
                        PeriodStartDate = ParseDate(root.Element("PeriodStartDate")?.Value),
                        SpeedBoostExpirationDate = ParseDate(root.Element("SpeedBoostExpirationDate")?.Value),
                        MachineId = root.Element("MachineId")?.Value,
                        IsStoreLicense = bool.Parse(root.Element("IsStoreLicense")?.Value ?? "false")
                    };

                    // Validate hardware binding for store licenses
                    if (_currentLicense.IsPro && _currentLicense.IsStoreLicense)
                    {
                        if (_currentLicense.MachineId != _machineId)
                        {
                            // License is bound to different machine - invalidate
                            _currentLicense.IsPro = false;
                            _currentLicense.LicenseKey = null;
                        }
                    }

                    // Check if period needs to be reset
                    _currentLicense.CheckAndResetPeriod();
                }
                catch
                {
                    // If file is corrupted, initialize new license
                    InitializeNewLicense();
                }
            }
            else
            {
                InitializeNewLicense();
            }
        }

        /// <summary>
        /// Initialize a new free tier license
        /// </summary>
        private void InitializeNewLicense()
        {
            _currentLicense = new LicenseInfo
            {
                IsPro = false,
                FilesProcessedCount = 0,
                PeriodStartDate = DateTime.Now
            };
            SaveLicense();
        }

        /// <summary>
        /// Save license to file with encryption
        /// </summary>
        private void SaveLicense()
        {
            var doc = new XDocument(
                new XElement("License",
                    new XElement("IsPro", _currentLicense.IsPro),
                    new XElement("LicenseKey", _currentLicense.LicenseKey ?? ""),
                    new XElement("ActivationDate", _currentLicense.ActivationDate?.ToString("o") ?? ""),
                    new XElement("ExpirationDate", _currentLicense.ExpirationDate?.ToString("o") ?? ""),
                    new XElement("FilesProcessedCount", _currentLicense.FilesProcessedCount),
                    new XElement("PeriodStartDate", _currentLicense.PeriodStartDate?.ToString("o") ?? ""),
                    new XElement("SpeedBoostExpirationDate", _currentLicense.SpeedBoostExpirationDate?.ToString("o") ?? ""),
                    new XElement("MachineId", _currentLicense.MachineId ?? ""),
                    new XElement("IsStoreLicense", _currentLicense.IsStoreLicense)
                )
            );
            
            // Add integrity check
            var xmlContent = doc.ToString();
            var signature = CalculateSignature(xmlContent);
            doc.Root.Add(new XElement("Signature", signature));
            
            doc.Save(_licenseFilePath);
        }

        /// <summary>
        /// Increment the files processed counter
        /// </summary>
        /// <param name="count">Number of files processed</param>
        public void IncrementFilesProcessed(int count = 1)
        {
            _currentLicense.CheckAndResetPeriod();
            _currentLicense.FilesProcessedCount += count;
            SaveLicense();
        }

        /// <summary>
        /// Reset the files processed counter (called when user watches ad or clicks)
        /// This removes throttling and gives the user a fresh start
        /// </summary>
        public void ResetFilesProcessedCounter()
        {
            _currentLicense.FilesProcessedCount = 0;
            SaveLicense();
        }

        /// <summary>
        /// Activate speed boost from watching video ad
        /// </summary>
        /// <param name="durationMinutes">Duration of speed boost in minutes</param>
        public void ActivateSpeedBoost(int durationMinutes = 60)
        {
            _currentLicense.SpeedBoostExpirationDate = DateTime.Now.AddMinutes(durationMinutes);
            SaveLicense();
        }

        /// <summary>
        /// Activate a Pro license with the given license key
        /// </summary>
        /// <param name="licenseKey">The license key to activate</param>
        /// <param name="isStoreLicense">Whether this is a Microsoft Store license</param>
        /// <returns>True if activation was successful, false otherwise</returns>
        public bool ActivateLicense(string licenseKey, bool isStoreLicense = false)
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
                return false;

            // Validate license key format and signature
            if (!ValidateLicenseKey(licenseKey))
                return false;

            _currentLicense.IsPro = true;
            _currentLicense.LicenseKey = licenseKey;
            _currentLicense.ActivationDate = DateTime.Now;
            _currentLicense.ExpirationDate = null; // Lifetime license
            _currentLicense.IsStoreLicense = isStoreLicense;
            
            // Bind to machine ID for store licenses (prevents copying to other machines)
            if (isStoreLicense)
            {
                _currentLicense.MachineId = _machineId;
            }
            
            SaveLicense();
            return true;
        }

        /// <summary>
        /// Deactivate the Pro license
        /// </summary>
        public void DeactivateLicense()
        {
            _currentLicense.IsPro = false;
            _currentLicense.LicenseKey = null;
            _currentLicense.ActivationDate = null;
            _currentLicense.ExpirationDate = null;
            SaveLicense();
        }

        /// <summary>
        /// Validate license key format and checksum
        /// </summary>
        /// <param name="licenseKey">The license key to validate</param>
        /// <returns>True if the license key is valid, false otherwise</returns>
        private bool ValidateLicenseKey(string licenseKey)
        {
            // License key format: XXXX-XXXX-XXXX-XXXX (16 characters + 3 hyphens)
            if (string.IsNullOrWhiteSpace(licenseKey) || licenseKey.Length != 19)
                return false;

            var parts = licenseKey.Split('-');
            if (parts.Length != 4)
                return false;

            foreach (var part in parts)
            {
                if (part.Length != 4)
                    return false;
            }

            // Simple checksum validation (last 4 characters are checksum of first 12)
            var dataSegment = string.Join("", parts[0], parts[1], parts[2]);
            var checksumSegment = parts[3];
            var calculatedChecksum = CalculateChecksum(dataSegment);

            return checksumSegment.Equals(calculatedChecksum, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Calculate checksum for license key validation
        /// </summary>
        private string CalculateChecksum(string data)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
                // Take first 2 bytes and convert to 4-character hex string
                return BitConverter.ToString(hash, 0, 2).Replace("-", "");
            }
        }

        /// <summary>
        /// Parse date string to nullable DateTime
        /// </summary>
        private DateTime? ParseDate(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            if (DateTime.TryParse(dateString, out var result))
                return result;

            return null;
        }

        /// <summary>
        /// Generate a new license key (for development/testing purposes)
        /// </summary>
        public static string GenerateLicenseKey()
        {
            var random = new Random();
            var part1 = GenerateRandomPart(random);
            var part2 = GenerateRandomPart(random);
            var part3 = GenerateRandomPart(random);
            
            var dataSegment = part1 + part2 + part3;
            var checksum = new LicenseManager().CalculateChecksum(dataSegment);

            return $"{part1}-{part2}-{part3}-{checksum}";
        }

        private static string GenerateRandomPart(Random random)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Exclude ambiguous characters
            var result = new char[4];
            for (int i = 0; i < 4; i++)
            {
                result[i] = chars[random.Next(chars.Length)];
            }
            return new string(result);
        }

        /// <summary>
        /// Get unique machine identifier
        /// </summary>
        private string GetMachineId()
        {
            try
            {
                // Combine multiple hardware identifiers for robust machine binding
                var processors = string.Empty;
                var motherboard = string.Empty;

                try
                {
                    // Get processor ID
                    var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                    ManagementObjectCollection mbsList = mbs.Get();
                    foreach (ManagementObject mo in mbsList)
                    {
                        processors = mo["ProcessorId"]?.ToString() ?? "";
                        break;
                    }

                    // Get motherboard serial
                    mbs = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BaseBoard");
                    mbsList = mbs.Get();
                    foreach (ManagementObject mo in mbsList)
                    {
                        motherboard = mo["SerialNumber"]?.ToString() ?? "";
                        break;
                    }
                }
                catch
                {
                    // WMI might not be available, fallback to machine name
                    processors = Environment.MachineName;
                    motherboard = Environment.UserName;
                }

                var combined = $"{processors}-{motherboard}-{Environment.MachineName}";
                
                // Hash to create consistent ID
                using (var sha256 = SHA256.Create())
                {
                    var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                    return BitConverter.ToString(hash).Replace("-", "").Substring(0, 32);
                }
            }
            catch
            {
                // Fallback to machine name hash
                using (var sha256 = SHA256.Create())
                {
                    var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(Environment.MachineName));
                    return BitConverter.ToString(hash).Replace("-", "").Substring(0, 32);
                }
            }
        }

        /// <summary>
        /// Calculate signature for license file integrity
        /// </summary>
        private string CalculateSignature(string content)
        {
            using (var sha256 = SHA256.Create())
            {
                var combined = content + ENCRYPTION_KEY + _machineId;
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
