using SyncMedia.Core.Models;
using System;
using System.IO;
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
        private const int TRIAL_DAYS = 14;
        private readonly string _licenseFilePath;
        private LicenseInfo _currentLicense;

        public LicenseManager()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SyncMedia");
            Directory.CreateDirectory(appDataPath);
            _licenseFilePath = Path.Combine(appDataPath, LICENSE_FILE);
            LoadLicense();
        }

        /// <summary>
        /// Gets the current license information
        /// </summary>
        public LicenseInfo CurrentLicense => _currentLicense;

        /// <summary>
        /// Load license from file or initialize trial
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
                        TrialExpirationDate = ParseDate(root.Element("TrialExpirationDate")?.Value)
                    };
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
        /// Initialize a new license with trial period
        /// </summary>
        private void InitializeNewLicense()
        {
            _currentLicense = new LicenseInfo
            {
                IsPro = false,
                TrialExpirationDate = DateTime.Now.AddDays(TRIAL_DAYS)
            };
            SaveLicense();
        }

        /// <summary>
        /// Save license to file
        /// </summary>
        private void SaveLicense()
        {
            var doc = new XDocument(
                new XElement("License",
                    new XElement("IsPro", _currentLicense.IsPro),
                    new XElement("LicenseKey", _currentLicense.LicenseKey ?? ""),
                    new XElement("ActivationDate", _currentLicense.ActivationDate?.ToString("o") ?? ""),
                    new XElement("ExpirationDate", _currentLicense.ExpirationDate?.ToString("o") ?? ""),
                    new XElement("TrialExpirationDate", _currentLicense.TrialExpirationDate?.ToString("o") ?? "")
                )
            );
            doc.Save(_licenseFilePath);
        }

        /// <summary>
        /// Activate a Pro license with the given license key
        /// </summary>
        /// <param name="licenseKey">The license key to activate</param>
        /// <returns>True if activation was successful, false otherwise</returns>
        public bool ActivateLicense(string licenseKey)
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
    }
}
