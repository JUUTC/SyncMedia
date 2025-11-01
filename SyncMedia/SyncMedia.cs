using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;



namespace SyncMedia
{

    public partial class SyncMedia : Form
    {
        #region "Public"
        public List<string> l = new List<string>();
        public List<string> dgvl = new List<string>();
        public List<string> StoredHashes = new List<string>();
        public List<string> hashes = new List<string>();
        public string XmlDatabase = string.Empty;
        public int MediaCount;
        public string Device;
        public string User;
        
        #endregion
        #region "Private"
        private static Regex r = new Regex(":");
        string AMFullName = "";
        
        // Optimized: Use HashSet for O(1) lookup instead of multiple string comparisons
        // Updated: Added modern formats (WebP, HEIC, AVIF, JPEG XL for images; WebM, MKV, etc. for videos)
        private static readonly HashSet<string> ImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Classic formats
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff",
            // Modern formats (2010+)
            ".webp",        // Google WebP (2010)
            ".heic", ".heif", // Apple HEIC/HEIF (2015)
            ".avif",        // AV1 Image Format (2019)
            ".jxl"          // JPEG XL (2021)
        };
        
        private static readonly HashSet<string> VideoExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Classic formats
            ".mov", ".mp4", ".wmv", ".avi", ".m4v", ".mpg", ".mpeg",
            // Modern/additional formats (2010+)
            ".webm",        // Google WebM (2010)
            ".mkv",         // Matroska (popular 2010+)
            ".flv",         // Flash Video
            ".ts", ".mts",  // MPEG Transport Stream
            ".3gp", ".3g2", // Mobile formats
            ".ogv",         // Ogg Video
            ".vob"          // DVD Video Object
        };
        
        private static readonly HashSet<string> AllMediaExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            // Images
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff",
            ".webp", ".heic", ".heif", ".avif", ".jxl",
            // Videos
            ".mov", ".mp4", ".wmv", ".avi", ".m4v", ".mpg", ".mpeg",
            ".webm", ".mkv", ".flv", ".ts", ".mts", ".3gp", ".3g2", ".ogv", ".vob"
        };
        
        // Optimized: Batch UI updates to reduce Application.DoEvents() calls
        private int _uiUpdateCounter = 0;
        private const int UI_UPDATE_BATCH_SIZE = 10;
        
        // UX Enhancement: Statistics tracking
        private int _totalFilesProcessed = 0;
        private int _duplicatesFound = 0;
        private int _errorsEncountered = 0;
        private long _totalBytesProcessed = 0;
        private DateTime _syncStartTime;
        private bool _isPaused = false;
        private bool _isCancelled = false;
        
        // UX Enhancement: Filter options
        private bool _filterImages = true;
        private bool _filterVideos = true;
        
        // Gamification: Achievement and points system
        private int _totalPoints = 0;
        private int _sessionPoints = 0;
        private int _totalFilesLifetime = 0;
        private int _totalDuplicatesLifetime = 0;
        private long _totalBytesLifetime = 0;
        private List<string> _achievements = new List<string>();
        #endregion
        
        public SyncMedia()

        {
            InitializeComponent();
            LoadGamificationData();
        }

       

        private void SyncMedia_Load(object sender, EventArgs e)
        {
            Device = Environment.MachineName;
            User = Environment.UserName; ;
            SourceFolderTextbox.Text = XmlData.ReadSetting("SourceFolder");
            if (SourceFolderTextbox.Text != string.Empty)
            {
                CreateDirectory(SourceFolderTextbox);
                ValidateFolder(SourceFolderTextbox.Text, SourceFolderTextbox);
            }

            DestinationFolderTextbox.Text = XmlData.ReadSetting("DestinationFolder");
            if (DestinationFolderTextbox.Text != string.Empty)
            {
                XmlDatabase = @DestinationFolderTextbox.Text + "MediaSync_SaveFile_" + Device + @".xml";
                CreateDirectory(DestinationFolderTextbox);
                ValidateFolder(DestinationFolderTextbox.Text, DestinationFolderTextbox);
                if (File.Exists(@DestinationFolderTextbox.Text + "MediaSync_SaveFile_" + Device + @".xml"))
                {
                    StoredHashes = XmlData.GetHashesList(XmlDatabase).ToList();
                    string ESL = XmlData.ReadSetting("EmergencySave");
                    if (ESL != string.Empty)
                    {
                        StoredHashes = hashes.Union(XmlData.GetHashesList(ESL)).ToList();
                    }
                }
            }

            RejectFolderTextbox.Text = XmlData.ReadSetting("RejectFolder");
            if (RejectFolderTextbox.Text != string.Empty)
            {
                CreateDirectory(RejectFolderTextbox);
                ValidateFolder(RejectFolderTextbox.Text, RejectFolderTextbox);
            }
        }
        
        // Gamification: Load saved progress
        private void LoadGamificationData()
        {
            try
            {
                _totalPoints = int.Parse(XmlData.ReadSetting("TotalPoints") ?? "0");
                _totalFilesLifetime = int.Parse(XmlData.ReadSetting("TotalFilesLifetime") ?? "0");
                _totalDuplicatesLifetime = int.Parse(XmlData.ReadSetting("TotalDuplicatesLifetime") ?? "0");
                _totalBytesLifetime = long.Parse(XmlData.ReadSetting("TotalBytesLifetime") ?? "0");
                
                string achievementsStr = XmlData.ReadSetting("Achievements");
                if (!string.IsNullOrEmpty(achievementsStr))
                {
                    _achievements = achievementsStr.Split('|').ToList();
                }
            }
            catch
            {
                // First time user - initialize with defaults
                _totalPoints = 0;
                _totalFilesLifetime = 0;
                _totalDuplicatesLifetime = 0;
                _totalBytesLifetime = 0;
                _achievements = new List<string>();
            }
        }
        
        // Gamification: Save progress
        private void SaveGamificationData()
        {
            XmlData.AddUpdateAppSettings("TotalPoints", _totalPoints.ToString());
            XmlData.AddUpdateAppSettings("TotalFilesLifetime", _totalFilesLifetime.ToString());
            XmlData.AddUpdateAppSettings("TotalDuplicatesLifetime", _totalDuplicatesLifetime.ToString());
            XmlData.AddUpdateAppSettings("TotalBytesLifetime", _totalBytesLifetime.ToString());
            XmlData.AddUpdateAppSettings("Achievements", string.Join("|", _achievements));
        }
        
        // Gamification: Award points and check achievements
        private void AwardPointsAndCheckAchievements()
        {
            _sessionPoints = 0;
            
            // Points system
            _sessionPoints += _totalFilesProcessed * 10;  // 10 points per file
            _sessionPoints += _duplicatesFound * 5;        // 5 points per duplicate found
            _sessionPoints += (int)(_totalBytesProcessed / (1024 * 1024)); // 1 point per MB
            
            // Speed bonus
            var elapsed = DateTime.Now - _syncStartTime;
            if (elapsed.TotalMinutes > 0)
            {
                var filesPerMinute = _totalFilesProcessed / elapsed.TotalMinutes;
                if (filesPerMinute > 10) _sessionPoints += 50; // Speed demon bonus
            }
            
            _totalPoints += _sessionPoints;
            _totalFilesLifetime += _totalFilesProcessed;
            _totalDuplicatesLifetime += _duplicatesFound;
            _totalBytesLifetime += _totalBytesProcessed;
            
            // Check for new achievements
            List<string> newAchievements = new List<string>();
            
            if (_totalFilesLifetime >= 100 && !_achievements.Contains("Century"))
            {
                _achievements.Add("Century");
                newAchievements.Add("🏆 Century - Synced 100 files!");
            }
            
            if (_totalFilesLifetime >= 1000 && !_achievements.Contains("Millennium"))
            {
                _achievements.Add("Millennium");
                newAchievements.Add("🏆 Millennium - Synced 1,000 files!");
            }
            
            if (_totalFilesLifetime >= 10000 && !_achievements.Contains("Epic"))
            {
                _achievements.Add("Epic");
                newAchievements.Add("🏆 Epic Organizer - Synced 10,000 files!");
            }
            
            if (_totalBytesLifetime > 1024L * 1024L * 1024L && !_achievements.Contains("Gigabyte"))
            {
                _achievements.Add("Gigabyte");
                newAchievements.Add("🏆 Gigabyte Club - Synced 1 GB!");
            }
            
            if (_totalBytesLifetime > 10L * 1024L * 1024L * 1024L && !_achievements.Contains("TenGigs"))
            {
                _achievements.Add("TenGigs");
                newAchievements.Add("🏆 Data Master - Synced 10 GB!");
            }
            
            if (_totalDuplicatesLifetime >= 50 && !_achievements.Contains("DupeHunter"))
            {
                _achievements.Add("DupeHunter");
                newAchievements.Add("🏆 Dupe Hunter - Found 50 duplicates!");
            }
            
            if (_totalFilesProcessed >= 100 && _errorsEncountered == 0 && !_achievements.Contains("Flawless"))
            {
                _achievements.Add("Flawless");
                newAchievements.Add("🏆 Flawless Victory - 100+ files with 0 errors!");
            }
            
            SaveGamificationData();
            
            // Show achievements
            if (newAchievements.Count > 0)
            {
                MessageBox.Show(
                    $"🎉 NEW ACHIEVEMENTS! 🎉\n\n{string.Join("\n", newAchievements)}\n\nTotal Points: {_totalPoints:N0}",
                    "Achievement Unlocked!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void CreateDirectory(TextBox textboxFolder)
        {
            try
            {
                if (!Directory.Exists(textboxFolder.Text))
                {
                    Directory.CreateDirectory(textboxFolder.Text);
                }
            }
            catch (Exception ex)
            {
                ErrorWriteToGrid(ex);
            }
        }

        private byte[] ImageHash(string srcImageFile)
        {
            // UX Enhancement: Check for pause/cancel
            while (_isPaused && !_isCancelled)
            {
                System.Threading.Thread.Sleep(100);
                Application.DoEvents();
            }
            
            if (_isCancelled)
            {
                return new byte[1];
            }
            
            string year = string.Empty;
            string month = string.Empty;
            string vyear = string.Empty;
            string vmonth = string.Empty;
            bool video = false;
            DateTime FileDateTime = DateTime.Today;
            string thisfilename = string.Empty;
            string finalFileName = string.Empty;
            string norootdir = srcImageFile.Replace(SourceFolderTextbox.Text, "");
            var srcHash = new byte[1];
            using (var cryptoSHA1 = SHA1.Create())
            {
                using (var fs = new FileStream(srcImageFile, FileMode.Open, FileAccess.Read))
                {

                    try
                    {
                        FileInfo fsd = new FileInfo(srcImageFile);
                        AMFullName = fsd.Name;
                        long AMLength = fsd.Length;
                        string AMType = fsd.Extension.ToLower();
                        if (AMType == ".db")
                        {
                            var srchash = new byte[1];
                            return srchash;
                        }
                        if (ImageFileTypes(AMType))
                        {
                            video = false;
                            using (Image myImage = Image.FromStream(fs, false, false))
                            {
                                try
                                {
                                    PropertyItem propItem = myImage.GetPropertyItem(36867);
                                    string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                                    FileDateTime = DateTime.Parse(dateTaken);
                                    year = FileDateTime.Year.ToString("0000");
                                    month = FileDateTime.Month.ToString("00");
                                    CheckCreateFolderStructureImage(year, month);
                                }
                                catch (Exception)
                                {
                                    // throw;
                                }
                            }
                        }
                        else if (VideoFileTypes(AMType))
                        {
                            video = true;
                            FileDateTime = fsd.LastWriteTime;
                            vyear = FileDateTime.Year.ToString("0000");
                            vmonth = FileDateTime.Month.ToString("00");
                            try
                            {
                                CheckCreateFolderStructureVideo(vyear, vmonth);
                            }
                            catch (Exception)
                            {

                                // throw;
                            }
                        }
                        else
                        {
                            var srchash = new byte[1];
                            return srchash;
                        }
                        OneFileLabel.Text = norootdir + " hashing...";
                        srcHash = cryptoSHA1.ComputeHash(fs);
                        SyncProgress.PerformStep();
                        
                        if (StoredHashes.Exists(x => x == Convert.ToBase64String(srcHash)) || hashes.Exists(x => x == Convert.ToBase64String(srcHash)))
                        {
                            fs.Close();
                            fsd.MoveTo(RejectFolderTextbox.Text + "\\" + AMFullName);
                            RejectMediaWriteToGrid(AMFullName);
                            var srchash = new byte[1];
                            return srchash;
                        }
                        thisfilename = AddtoList(srcImageFile, true);
                        if (thisfilename.Length != 0)
                        {
                            thisfilename = " " + thisfilename;
                        }
                        fs.Close();
                        if (video)
                        {
                            finalFileName = SessionMediaCountFormat(FileDateTime, thisfilename, fsd);
                            finalFileName = MoveVideoUpdateStatusGrid(vyear, vmonth, FileDateTime, finalFileName, fsd);
                        }
                        else
                        {
                            finalFileName = SessionMediaCountFormat(FileDateTime, thisfilename, fsd);
                            finalFileName = MoveImageUpdateStatusGrid(year, month, FileDateTime, finalFileName, fsd);
                        }

                        // UX Enhancement: Track statistics
                        _totalFilesProcessed++;
                        _totalBytesProcessed += AMLength;
                        
                        hashes.Add(Convert.ToBase64String(srcHash));
                        UpdateProgressInfo();
                        // Optimized: UI updates are now batched in UpdateDataGridView()
                        return srcHash;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            RejectMediaWriteToGrid(AMFullName);
                            return srcHash;
                        }
                        catch (Exception ex)
                        {
                            var srchash = new byte[1];
                            ErrorWriteToGrid(ex);
                            return srchash;
                            // throw;
                        }

                        //throw;
                    }
                }
            }
        }

        // Optimized: Helper method to batch UI updates
        private void UpdateDataGridView(bool forceUpdate = false)
        {
            _uiUpdateCounter++;
            if (forceUpdate || _uiUpdateCounter >= UI_UPDATE_BATCH_SIZE)
            {
                dataGridViewPreview.DataSource = dgvl.Select(x => new { Value = x }).ToList();
                dataGridViewPreview.AutoResizeColumn(0);
                dataGridViewPreview.Refresh();
                if (dataGridViewPreview.RowCount > 1)
                {
                    dataGridViewPreview.CurrentCell = dataGridViewPreview.Rows[dataGridViewPreview.RowCount - 1].Cells[0];
                }
                Application.DoEvents();
                _uiUpdateCounter = 0;
            }
        }
        
        // UX Enhancement: Update progress with statistics and ETA
        private void UpdateProgressInfo()
        {
            if (_syncStartTime == DateTime.MinValue) return;
            
            var elapsed = DateTime.Now - _syncStartTime;
            var filesPerSecond = elapsed.TotalSeconds > 0 ? _totalFilesProcessed / elapsed.TotalSeconds : 0;
            var remaining = MediaCount - SyncProgress.Value;
            var eta = filesPerSecond > 0 ? TimeSpan.FromSeconds(remaining / filesPerSecond) : TimeSpan.Zero;
            
            var sizeInMB = _totalBytesProcessed / (1024.0 * 1024.0);
            var speed = elapsed.TotalSeconds > 0 ? sizeInMB / elapsed.TotalSeconds : 0;
            
            TotalFilesLabel.Text = $"{SyncProgress.Value}/{MediaCount} | {sizeInMB:F2} MB | {speed:F2} MB/s | ETA: {eta:hh\\:mm\\:ss} | Duplicates: {_duplicatesFound} | Errors: {_errorsEncountered}";
        }
        
        // UX Enhancement: Validate folder path
        private bool ValidateFolder(string path, TextBox textBox)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                textBox.BackColor = Color.LightYellow;
                return false;
            }
            
            if (!Directory.Exists(path))
            {
                textBox.BackColor = Color.LightCoral;
                return false;
            }
            
            textBox.BackColor = Color.LightGreen;
            return true;
        }

        private void RejectMediaWriteToGrid(string AMFullName)
        {
            _duplicatesFound++;
            dgvl.Add($"[DUPLICATE] {AMFullName}");
            UpdateDataGridView();
        }

        public void ErrorWriteToGrid(Exception exc)
        {
            _errorsEncountered++;
            // UX Enhancement: Show full error message instead of truncating
            string errorMsg = exc.Message.Length > 100 ? exc.Message.Substring(0, 97) + "..." : exc.Message;
            dgvl.Add($"[ERROR] {errorMsg}");
            UpdateDataGridView();
        }

        private string MoveImageUpdateStatusGrid(string year, string month, DateTime FileDateTime, string finalFileName, FileInfo fsd)
        {
            if (!File.Exists(DestinationFolderTextbox.Text + year + "\\" + year + " " + month + "\\" + finalFileName))
            {
                fsd.MoveTo(DestinationFolderTextbox.Text + year + "\\" + year + " " + month + "\\" + finalFileName);
            }
            else
            {
                finalFileName = finalFileName.Insert(finalFileName.IndexOf("."), "-" + FileDateTime.Second);
                fsd.MoveTo(DestinationFolderTextbox.Text + year + "\\" + year + " " + month + "\\" + finalFileName);
            }

            dgvl.Add(year + "\\" + year + " " + month + "\\" + finalFileName);
            UpdateDataGridView();
            return finalFileName;
        }

        private string MoveVideoUpdateStatusGrid(string vyear, string vmonth, DateTime FileDateTime, string finalFileName, FileInfo fsd)
        {
            if (!File.Exists(DestinationFolderTextbox.Text + vyear + "\\" + vyear + " " + vmonth + "\\" + vyear + " " + vmonth + " Movies\\" + finalFileName))
            {
                fsd.MoveTo(DestinationFolderTextbox.Text + vyear + "\\" + vyear + " " + vmonth + "\\" + vyear + " " + vmonth + " Movies\\" + finalFileName);
            }
            else
            {
                finalFileName = finalFileName.Insert(finalFileName.IndexOf("."), "-" + FileDateTime.Second);
                fsd.MoveTo(DestinationFolderTextbox.Text + vyear + "\\" + vyear + " " + vmonth + "\\" + vyear + " " + vmonth + " Movies\\" + finalFileName);
            }

            dgvl.Add(vyear + "\\" + vyear + " " + vmonth + "\\" + vyear + " " + vmonth + " Movies\\" + finalFileName);
            UpdateDataGridView();
            return finalFileName;
        }

        private string SessionMediaCountFormat(DateTime FileDateTime, string thisfilename, FileInfo fsd)
        {
            string finalFileName;
            switch (MediaCount.ToString().Length)
            {
                case 1:
                    finalFileName = FileNameDT(FileDateTime) + thisfilename + " " + SyncProgress.Value.ToString("000") + fsd.Extension;
                    break;
                case 2:
                    finalFileName = FileNameDT(FileDateTime) + thisfilename + " " + SyncProgress.Value.ToString("000") + fsd.Extension;
                    break;
                case 3:
                    finalFileName = FileNameDT(FileDateTime) + thisfilename + " " + SyncProgress.Value.ToString("000") + fsd.Extension;
                    break;
                case 4:
                    finalFileName = FileNameDT(FileDateTime) + thisfilename + " " + SyncProgress.Value.ToString("0000") + fsd.Extension;
                    break;
                case 5:
                    finalFileName = FileNameDT(FileDateTime) + thisfilename + " " + SyncProgress.Value.ToString("00000") + fsd.Extension;
                    break;
                case 6:
                    finalFileName = FileNameDT(FileDateTime) + thisfilename + " " + SyncProgress.Value.ToString("000000") + fsd.Extension;
                    break;
                case 7:
                    finalFileName = FileNameDT(FileDateTime) + thisfilename + " " + SyncProgress.Value.ToString("0000000") + fsd.Extension;
                    break;
                case 8:
                    finalFileName = FileNameDT(FileDateTime) + thisfilename + " " + SyncProgress.Value.ToString("00000000") + fsd.Extension;
                    break;
                default:
                    finalFileName = FileNameDT(FileDateTime) + thisfilename + " " + SyncProgress.Value.ToString("000000000") + fsd.Extension;
                    break;
            }

            return finalFileName;
        }

        private static bool VideoFileTypes(string AMType)
        {
            return VideoExtensions.Contains(AMType);
        }

        private static bool ImageFileTypes(string AMType)
        {
            return ImageExtensions.Contains(AMType);
        }

        private void CheckCreateFolderStructureVideo(string vyear, string vmonth)
        {
            if (!Directory.Exists(DestinationFolderTextbox.Text + vyear))
            {
                Directory.CreateDirectory(DestinationFolderTextbox.Text + vyear);
            }
            if (!Directory.Exists(DestinationFolderTextbox.Text + vyear + "\\" + vyear + " " + vmonth))
            {
                Directory.CreateDirectory(DestinationFolderTextbox.Text + vyear + "\\" + vyear + " " + vmonth);
            }
            if (!Directory.Exists(DestinationFolderTextbox.Text + vyear + "\\" + vyear + " " + vmonth + "\\" + vyear + " " + vmonth + " Favs"))
            {
                Directory.CreateDirectory(DestinationFolderTextbox.Text + vyear + "\\" + vyear + " " + vmonth + "\\" + vyear + " " + vmonth + " Favs");
            }
            if (!Directory.Exists(DestinationFolderTextbox.Text + vyear + "\\" + vyear + " " + vmonth + "\\" + vyear + " " + vmonth + " Movies"))
            {
                Directory.CreateDirectory(DestinationFolderTextbox.Text + vyear + "\\" + vyear + " " + vmonth + "\\" + vyear + " " + vmonth + " Movies");
            }
        }

        private void CheckCreateFolderStructureImage(string year, string month)
        {
            if (!Directory.Exists(DestinationFolderTextbox.Text + year))
            {
                Directory.CreateDirectory(DestinationFolderTextbox.Text + year);
            }
            if (!Directory.Exists(DestinationFolderTextbox.Text + year + "\\" + year + " " + month))
            {
                Directory.CreateDirectory(DestinationFolderTextbox.Text + year + "\\" + year + " " + month);
            }
            if (!Directory.Exists(DestinationFolderTextbox.Text + year + "\\" + year + " " + month + "\\" + year + " " + month + " Favs"))
            {
                Directory.CreateDirectory(DestinationFolderTextbox.Text + year + "\\" + year + " " + month + "\\" + year + " " + month + " Favs");
            }
            if (!Directory.Exists(DestinationFolderTextbox.Text + year + "\\" + year + " " + month + "\\" + year + " " + month + " Movies"))
            {
                Directory.CreateDirectory(DestinationFolderTextbox.Text + year + "\\" + year + " " + month + "\\" + year + " " + month + " Movies");
            }
        }

       

        private string FileNameDT(DateTime FileDT)
        {
            return FileDT.Year.ToString("0000") + "-" + FileDT.Month.ToString("00") + "-" + FileDT.Day.ToString("00");
        }

        private void HashAll_Click(object sender, EventArgs e)
        {
            // UX Enhancement: Validate folders before starting
            if (!ValidateFolder(SourceFolderTextbox.Text, SourceFolderTextbox))
            {
                MessageBox.Show("Please select a valid source folder.", "Invalid Source", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (!ValidateFolder(DestinationFolderTextbox.Text, DestinationFolderTextbox))
            {
                MessageBox.Show("Please select a valid destination folder.", "Invalid Destination", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            if (!ValidateFolder(RejectFolderTextbox.Text, RejectFolderTextbox))
            {
                MessageBox.Show("Please select a valid reject folder.", "Invalid Reject Folder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // UX Enhancement: Reset statistics
            _totalFilesProcessed = 0;
            _duplicatesFound = 0;
            _errorsEncountered = 0;
            _totalBytesProcessed = 0;
            _syncStartTime = DateTime.Now;
            _isCancelled = false;
            _isPaused = false;
            
            HashAll.Enabled = false;
            PauseButton.Enabled = true;
            CancelButton.Enabled = true;
            
            // UX Enhancement: Apply filters based on checkboxes
            HashSet<string> filteredExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (_filterImages)
            {
                foreach (var ext in ImageExtensions)
                    filteredExtensions.Add(ext);
            }
            if (_filterVideos)
            {
                foreach (var ext in VideoExtensions)
                    filteredExtensions.Add(ext);
            }
            
            if (filteredExtensions.Count == 0)
            {
                MessageBox.Show("Please select at least one file type filter (Images or Videos).", "No Filters", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                HashAll.Enabled = true;
                return;
            }
            
            // Optimized: Use HashSet for file extension checking with filters
            MediaCount = Directory.EnumerateFiles(SourceFolderTextbox.Text, "*.*", SearchOption.AllDirectories)
                .Count(file => filteredExtensions.Contains(Path.GetExtension(file)));
            SyncProgress.Maximum = MediaCount;
            TotalFilesLabel.Text = MediaCount.ToString();
            SyncProgress.Step = 1;
            
            // Optimized: Filter files before processing
            Directory.EnumerateFiles(SourceFolderTextbox.Text, "*.*", SearchOption.AllDirectories)
                .Where(file => filteredExtensions.Contains(Path.GetExtension(file)))
                .OrderBy(Filename => Filename)
                .Select(f => new { FileName = f, FileHash = Convert.ToBase64String(ImageHash(f)) })
                .AsParallel()
                .ToList();
                
            // UX Enhancement: Show final statistics
            UpdateProgressInfo();
            MediaCount = 0;
            hashes = hashes.Union(StoredHashes).ToList();
            XmlData.CreateXmlDoc(XmlDatabase, hashes);
            // Optimized: Force final UI update
            UpdateDataGridView(forceUpdate: true);
            
            var elapsed = DateTime.Now - _syncStartTime;
            
            // Gamification: Award points and check achievements
            AwardPointsAndCheckAchievements();
            
            OneFileLabel.Text = $"Completed! Processed: {_totalFilesProcessed} files ({_totalBytesProcessed / (1024.0 * 1024.0):F2} MB) in {elapsed:hh\\:mm\\:ss} | Duplicates: {_duplicatesFound} | Errors: {_errorsEncountered} | 🎯 Points: +{_sessionPoints:N0} (Total: {_totalPoints:N0})";
            
            // Re-enable controls
            HashAll.Enabled = true;
            PauseButton.Enabled = false;
            CancelButton.Enabled = false;
        }

       

        private string AddtoList(string filename, bool naming = false)
        {
            string newfilename = string.Empty;
            string norootdir = filename.Replace(SourceFolderTextbox.Text, string.Empty);
            string nodate = RemoveFolderPath(norootdir); //, "\\b(?<month>\\d{1,2})/(?<day>\\d{1,2})/(?<year>\\d{2,4})\\b", string.Empty);
            nodate = RemoveFolderStruture(norootdir, nodate);
            nodate = CleanFileName(nodate);

            string[] words = nodate.Split(new char[] { '-', '|', }, StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
            if (!naming)
            {
                foreach (string word in words)
                {
                    if (!l.Exists(x => x == word))
                        l.Add(word);
                }
                return null;
            }
            else
            {
                List<string> checkedNames = FilesNamesToInclude.CheckedItems.Cast<string>().ToList();
                foreach (string word in words)
                {
                    if (checkedNames.Exists(x => x == word))
                    {
                        if (newfilename.Length == 0)
                        {
                            newfilename = word;
                        }
                        else
                        {
                            newfilename += " " + word;
                        }
                    }

                }
                return newfilename;
            }
        }

        private static string RemoveFolderStruture(string norootdir, string nodate)
        {
            if (nodate.Contains(@"\\"))
            {
                nodate = nodate.TrimStart('\\');
            }

            if (nodate.Contains(@"\"))
            {
                nodate = nodate.TrimStart('\\');
            }
            nodate = RemoveFolderPath(norootdir);
            if (nodate.Contains(@"\"))
            {
                nodate = Regex.Replace(nodate, "^[^_]*\\\\", string.Empty);
            }

            return nodate;
        }

        private static string CleanFileName(string nodate)
        {
            // Optimized: Remove duplicates and convert to lower case once
            nodate = Regex.Replace(nodate, @"[\d-]", string.Empty);
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

        private static string RemoveFolderPath(string norootdir)
        {
            return Regex.Replace(norootdir, "\\b(?<year>\\d{2,4})/(?<month>\\d{1,2})/(?<day>\\d{2,4})\\b", string.Empty);
        }

        private void UpdateNamingButton_Click(object sender, EventArgs e)
        {
            FilesNamesToInclude.Items.Clear();
            l.Clear();
            // Optimized: Use HashSet for file extension checking
            var UpdatedFileList = Directory.EnumerateFiles(SourceFolderTextbox.Text, "*.*", SearchOption.AllDirectories)
                .Where(file => AllMediaExtensions.Contains(Path.GetExtension(file)))
                .ToList();
            foreach (var item in UpdatedFileList)
            {
                AddtoList(item);
            }
            foreach (var addtocheckbox in l)
            {
                FilesNamesToInclude.Items.Add(addtocheckbox);
            }
        }

        private void InsertUpdateFolderSetting(TextBox FolderTextbox, string FolderType)
        {
            switch (FolderType)
            {
                case "SourceFolder":
                    FolderTextbox.Text = SourcefolderBrowser.SelectedPath + "\\";
                    break;
                case "DestinationFolder":
                    FolderTextbox.Text = DestinationfolderBrowser.SelectedPath + "\\";
                    break;
                case "RejectFolder":
                    FolderTextbox.Text = RejectfolderBrowser.SelectedPath + "\\";
                    break;
                default:
                    return;
            }
            
            XmlData.AddUpdateAppSettings(FolderType, FolderTextbox.Text);
            
            if (!Directory.Exists(FolderTextbox.Text))
            {
                Directory.CreateDirectory(FolderTextbox.Text);
            }
            
            // UX Enhancement: Validate folder after selection
            ValidateFolder(FolderTextbox.Text, FolderTextbox);
        }

        private void SetFolderSource_Click(object sender, EventArgs e)
        {
            if (SourcefolderBrowser.ShowDialog() == DialogResult.OK)
            {
                InsertUpdateFolderSetting(SourceFolderTextbox, "SourceFolder");
            }
        }

        private void SourceFolderTextbox_Clicked(object sender, EventArgs e)
        {
            if (SourcefolderBrowser.ShowDialog() == DialogResult.OK)
            {
                InsertUpdateFolderSetting(SourceFolderTextbox, "SourceFolder");
            }
        }

        private void SetFolderDestination_Click(object sender, EventArgs e)
        {
            if (DestinationfolderBrowser.ShowDialog() == DialogResult.OK)
            {
                InsertUpdateFolderSetting(DestinationFolderTextbox, "DestinationFolder");
            }
        }

        private void DestinationFolderTextbox_Clicked(object sender, EventArgs e)
        {
            if (DestinationfolderBrowser.ShowDialog() == DialogResult.OK)
            {
                InsertUpdateFolderSetting(DestinationFolderTextbox, "DestinationFolder");
            }
        }

        private void SetFolderReject_Click(object sender, EventArgs e)
        {
            if (RejectfolderBrowser.ShowDialog() == DialogResult.OK)
            {
                InsertUpdateFolderSetting(RejectFolderTextbox, "RejectFolder");
            }
        }

        private void RejectFolderTextbox_Clicked(object sender, EventArgs e)
        {
            if (DestinationfolderBrowser.ShowDialog() == DialogResult.OK)
            {
                InsertUpdateFolderSetting(RejectFolderTextbox, "RejectFolder");
            }
        }
        
        // UX Enhancement: Filter checkbox handlers
        private void FilterImagesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            _filterImages = FilterImagesCheckbox.Checked;
        }
        
        private void FilterVideosCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            _filterVideos = FilterVideosCheckbox.Checked;
        }
        
        // UX Enhancement: Pause button handler
        private void PauseButton_Click(object sender, EventArgs e)
        {
            _isPaused = !_isPaused;
            PauseButton.Text = _isPaused ? "Resume" : "Pause";
            
            if (_isPaused)
            {
                OneFileLabel.Text = "Sync paused. Click Resume to continue.";
            }
        }
        
        // UX Enhancement: Cancel button handler
        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel the sync operation?", 
                "Cancel Sync", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _isCancelled = true;
                OneFileLabel.Text = "Sync cancelled by user.";
                HashAll.Enabled = true;
                PauseButton.Enabled = false;
                CancelButton.Enabled = false;
            }
        }
        
        // UX Enhancement: Search textbox handler
        private void SearchTextbox_TextChanged(object sender, EventArgs e)
        {
            string searchText = SearchTextbox.Text.ToLower();
            
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Show all items
                dataGridViewPreview.DataSource = dgvl.Select(x => new { Value = x }).ToList();
            }
            else
            {
                // Filter items based on search
                var filtered = dgvl.Where(x => x.ToLower().Contains(searchText))
                                  .Select(x => new { Value = x })
                                  .ToList();
                dataGridViewPreview.DataSource = filtered;
            }
            
            dataGridViewPreview.AutoResizeColumn(0);
            dataGridViewPreview.Refresh();
        }
    }


}
