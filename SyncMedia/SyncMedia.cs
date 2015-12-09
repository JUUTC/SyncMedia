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
        #endregion
        public SyncMedia()

        {
            InitializeComponent();
        }

       

        private void SyncMedia_Load(object sender, EventArgs e)
        {
            Device = Environment.MachineName;
            User = Environment.UserName;

            SourceFolderTextbox.Text = XmlData.ReadSetting("SourceFolder");
            if (SourceFolderTextbox.Text != string.Empty)
            {
                CreateDirectory(SourceFolderTextbox);
            }

            DestinationFolderTextbox.Text = XmlData.ReadSetting("DestinationFolder");
            if (DestinationFolderTextbox.Text != string.Empty)
            {
                XmlDatabase = @DestinationFolderTextbox.Text + Device + @".xml";
                CreateDirectory(DestinationFolderTextbox);
                if (File.Exists(@DestinationFolderTextbox.Text + Device + @".xml"))
                {
                    StoredHashes = XmlData.GetHashesList(XmlDatabase).ToList();
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
            using (SHA1CryptoServiceProvider cryptoSHA1 = new SHA1CryptoServiceProvider())
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
                        Application.DoEvents();
                        srcHash = cryptoSHA1.ComputeHash(fs);
                        SyncProgress.PerformStep();
                        Application.DoEvents();
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

                        Application.DoEvents();
                        hashes.Add(Convert.ToBase64String(srcHash));
                        TotalFilesLabel.Text = SyncProgress.Value + "/" + MediaCount.ToString();
                        Application.DoEvents();
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

        private void RejectMediaWriteToGrid(string AMFullName)
        {
            dgvl.Add(AMFullName + " moved to REJECT FOLDER");
            dataGridViewPreview.DataSource = dgvl.Select(x => new { Value = x }).ToList();
            dataGridViewPreview.AutoResizeColumn(0);
            dataGridViewPreview.Refresh();
            if (dataGridViewPreview.RowCount > 1)
            {
                dataGridViewPreview.CurrentCell = dataGridViewPreview.Rows[dataGridViewPreview.RowCount - 1].Cells[0];
            }
            Application.DoEvents();
        }

        public void ErrorWriteToGrid(Exception exc)
        {
            dgvl.Add(exc.Message.Substring(0,16) + "...");
            dataGridViewPreview.DataSource = dgvl.Select(x => new { Value = x }).ToList();
            dataGridViewPreview.AutoResizeColumn(0);
            dataGridViewPreview.Refresh();
            if (dataGridViewPreview.RowCount > 1)
            {
                dataGridViewPreview.CurrentCell = dataGridViewPreview.Rows[dataGridViewPreview.RowCount - 1].Cells[0];
            }
            Application.DoEvents();
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
            dataGridViewPreview.DataSource = dgvl.Select(x => new { Value = x }).ToList();
            dataGridViewPreview.AutoResizeColumn(0);
            dataGridViewPreview.Refresh();
            if (dataGridViewPreview.RowCount > 1)
            {
                dataGridViewPreview.CurrentCell = dataGridViewPreview.Rows[dataGridViewPreview.RowCount - 1].Cells[0];
            }
            Application.DoEvents();
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
            dataGridViewPreview.DataSource = dgvl.Select(x => new { Value = x }).ToList();
            dataGridViewPreview.AutoResizeColumn(0);
            dataGridViewPreview.Refresh();
            if (dataGridViewPreview.RowCount > 1)
            {
                dataGridViewPreview.CurrentCell = dataGridViewPreview.Rows[dataGridViewPreview.RowCount - 1].Cells[0];
            }
            Application.DoEvents();
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
            return AMType == ".mov" || AMType == ".mp4" || AMType == ".wmv" || AMType == ".avi" || AMType == ".m4v" || AMType == ".mpg" || AMType == ".mpeg";
        }

        private static bool ImageFileTypes(string AMType)
        {
            return AMType == ".jpg" || AMType == ".png" || AMType == ".bmp" || AMType == ".jpeg" || AMType == ".gif" || AMType == ".tif" || AMType == ".tiff";
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
            MediaCount = Directory.EnumerateFiles(SourceFolderTextbox.Text, "*.*", SearchOption.AllDirectories).Where(file => file.ToLower().EndsWith(".jpg") || file.ToLower().EndsWith(".png") || file.ToLower().EndsWith(".bmp") || file.ToLower().EndsWith(".jpeg") || file.ToLower().EndsWith(".gif") || file.ToLower().EndsWith(".tif") || file.ToLower().EndsWith(".tiff") || file.ToLower().EndsWith(".mov") || file.ToLower().EndsWith(".mp4") || file.ToLower().EndsWith(".wmv") || file.ToLower().EndsWith(".avi") || file.ToLower().EndsWith(".m4v") || file.ToLower().EndsWith(".mpg") || file.ToLower().EndsWith(".mpeg")).Count();
            SyncProgress.Maximum = MediaCount;
            TotalFilesLabel.Text = MediaCount.ToString();
            SyncProgress.Step = 1;
            Directory.EnumerateFiles(SourceFolderTextbox.Text, "*.*", SearchOption.AllDirectories).OrderBy(Filename => Filename).Select(f => new { FileName = f, FileHash = Convert.ToBase64String(ImageHash(f)) }).AsParallel().ToList();
            TotalFilesLabel.Text = MediaCount.ToString() + "/" + MediaCount.ToString();
            MediaCount = 0;
            OneFileLabel.Text = "All Media Checked and Moved.  Close and re-open the application to sync more files";
            XmlData.CreateXmlDoc(XmlDatabase, hashes.Union(StoredHashes).ToList());
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
            //Clean this up.  
            nodate = Regex.Replace(nodate, @"[\d-]", string.Empty);
            nodate = nodate.ToLower().Replace(@".mp", string.Empty);
            nodate = nodate.ToLower().Replace(@".jpg", string.Empty);
            nodate = nodate.ToLower().Replace(@".jepg", string.Empty);
            nodate = nodate.ToLower().Replace(@".mpeg", string.Empty);
            nodate = nodate.ToLower().Replace(@".mpg", string.Empty);
            nodate = nodate.ToLower().Replace(@".png", string.Empty);
            nodate = nodate.ToLower().Replace(@".mov", string.Empty);
            nodate = nodate.ToLower().Replace(".mov", string.Empty);
            nodate = nodate.ToLower().Replace(".mp4", string.Empty);
            nodate = nodate.ToLower().Replace(".wmv", string.Empty);
            nodate = nodate.ToLower().Replace(".avi", string.Empty);
            nodate = nodate.ToLower().Replace(".m4v", string.Empty);
            nodate = nodate.ToLower().Replace(".mpg", string.Empty);
            nodate = nodate.ToLower().Replace(".mpeg", string.Empty);
            nodate = nodate.ToLower().Replace(".jpg", string.Empty);
            nodate = nodate.ToLower().Replace(".png", string.Empty);
            nodate = nodate.ToLower().Replace(".bmp", string.Empty);
            nodate = nodate.ToLower().Replace(".jpeg", string.Empty);
            nodate = nodate.ToLower().Replace(".gif", string.Empty);
            nodate = nodate.ToLower().Replace(".tif", string.Empty);
            nodate = nodate.ToLower().Replace(".tiff", string.Empty);
            nodate = nodate.ToLower().Replace(@",", string.Empty);
            nodate = nodate.ToLower().Replace(@".", string.Empty);
            nodate = nodate.ToLower().Replace(@"/", string.Empty);
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
            var UpdatedFileList = Directory.EnumerateFiles(SourceFolderTextbox.Text, "*.*", SearchOption.AllDirectories).Where(file => file.ToLower().EndsWith(".jpg") || file.ToLower().EndsWith(".png") || file.ToLower().EndsWith(".bmp") || file.ToLower().EndsWith(".jpeg") || file.ToLower().EndsWith(".gif") || file.ToLower().EndsWith(".tif") || file.ToLower().EndsWith(".tiff") || file.ToLower().EndsWith(".mov") || file.ToLower().EndsWith(".mp4") || file.ToLower().EndsWith(".wmv") || file.ToLower().EndsWith(".avi") || file.ToLower().EndsWith(".m4v") || file.ToLower().EndsWith(".mpg") || file.ToLower().EndsWith(".mpeg")).ToList();
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
