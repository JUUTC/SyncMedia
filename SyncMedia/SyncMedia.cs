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
        private static readonly HashSet<string> ImageExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff"
        };
        
        private static readonly HashSet<string> VideoExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".mov", ".mp4", ".wmv", ".avi", ".m4v", ".mpg", ".mpeg"
        };
        
        private static readonly HashSet<string> AllMediaExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tif", ".tiff",
            ".mov", ".mp4", ".wmv", ".avi", ".m4v", ".mpg", ".mpeg"
        };
        
        // Optimized: Batch UI updates to reduce Application.DoEvents() calls
        private int _uiUpdateCounter = 0;
        private const int UI_UPDATE_BATCH_SIZE = 10;
        #endregion
        public SyncMedia()

        {
            InitializeComponent();
        }

       

        private void SyncMedia_Load(object sender, EventArgs e)
        {
            Device = Environment.MachineName;
            User = Environment.UserName; ;
            SourceFolderTextbox.Text = XmlData.ReadSetting("SourceFolder");
            if (SourceFolderTextbox.Text != string.Empty)
            {
                CreateDirectory(SourceFolderTextbox);
            }

            DestinationFolderTextbox.Text = XmlData.ReadSetting("DestinationFolder");
            if (DestinationFolderTextbox.Text != string.Empty)
            {
                XmlDatabase = @DestinationFolderTextbox.Text + "MediaSync_SaveFile_" + Device + @".xml";
                CreateDirectory(DestinationFolderTextbox);
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

                        hashes.Add(Convert.ToBase64String(srcHash));
                        TotalFilesLabel.Text = SyncProgress.Value + "/" + MediaCount.ToString();
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

        private void RejectMediaWriteToGrid(string AMFullName)
        {
            dgvl.Add(AMFullName + " moved to REJECT FOLDER");
            UpdateDataGridView();
        }

        public void ErrorWriteToGrid(Exception exc)
        {
            dgvl.Add(exc.Message.Substring(0,16) + "...");
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
            HashAll.Enabled = false;
            // Optimized: Use HashSet for file extension checking
            MediaCount = Directory.EnumerateFiles(SourceFolderTextbox.Text, "*.*", SearchOption.AllDirectories)
                .Count(file => AllMediaExtensions.Contains(Path.GetExtension(file)));
            SyncProgress.Maximum = MediaCount;
            TotalFilesLabel.Text = MediaCount.ToString();
            SyncProgress.Step = 1;
            // Optimized: Filter files before processing
            Directory.EnumerateFiles(SourceFolderTextbox.Text, "*.*", SearchOption.AllDirectories)
                .Where(file => AllMediaExtensions.Contains(Path.GetExtension(file)))
                .OrderBy(Filename => Filename)
                .Select(f => new { FileName = f, FileHash = Convert.ToBase64String(ImageHash(f)) })
                .AsParallel()
                .ToList();
            TotalFilesLabel.Text = MediaCount.ToString() + "/" + MediaCount.ToString();
            MediaCount = 0;
            hashes = hashes.Union(StoredHashes).ToList();
            XmlData.CreateXmlDoc(XmlDatabase, hashes);
            // Optimized: Force final UI update
            UpdateDataGridView(forceUpdate: true);
            OneFileLabel.Text = "All Media Checked and Moved.  Close and re-open the application to sync more files";
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
            
            // Remove all file extensions in one pass
            nodate = nodate.Replace(".mp", string.Empty);
            nodate = nodate.Replace(".jpg", string.Empty);
            nodate = nodate.Replace(".jpeg", string.Empty);
            nodate = nodate.Replace(".jepg", string.Empty);
            nodate = nodate.Replace(".png", string.Empty);
            nodate = nodate.Replace(".bmp", string.Empty);
            nodate = nodate.Replace(".gif", string.Empty);
            nodate = nodate.Replace(".tif", string.Empty);
            nodate = nodate.Replace(".tiff", string.Empty);
            nodate = nodate.Replace(".mov", string.Empty);
            nodate = nodate.Replace(".mp4", string.Empty);
            nodate = nodate.Replace(".wmv", string.Empty);
            nodate = nodate.Replace(".avi", string.Empty);
            nodate = nodate.Replace(".m4v", string.Empty);
            nodate = nodate.Replace(".mpg", string.Empty);
            nodate = nodate.Replace(".mpeg", string.Empty);
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
            //if (string.Join("", Data.CheckForDevice(Device).Take(1)) == string.Empty)
            //{
            //   Data.InsertDevice(Device, FolderType, FolderTextbox.Text);
            //}
            //else
            //{
            //    Data.UpdateDevice(Device, FolderType, FolderTextbox.Text);
            //}
            if (!Directory.Exists(FolderTextbox.Text))
            {
                Directory.CreateDirectory(FolderTextbox.Text);
            }
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
    }


}
