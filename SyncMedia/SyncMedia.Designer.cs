namespace SyncMedia
{
    partial class SyncMedia
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UpdateNamingButton = new System.Windows.Forms.Button();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.dataGridViewPreview = new System.Windows.Forms.DataGridView();
            this.SyncProgress = new System.Windows.Forms.ProgressBar();
            this.OneFileLabel = new System.Windows.Forms.Label();
            this.TotalFilesLabel = new System.Windows.Forms.Label();
            this.HashAll = new System.Windows.Forms.Button();
            this.FilesNamesToInclude = new System.Windows.Forms.CheckedListBox();
            this.FileNameLabel = new System.Windows.Forms.Label();
            this.SourcefolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.DestinationfolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.SourceFolderTextbox = new System.Windows.Forms.TextBox();
            this.DestinationFolderTextbox = new System.Windows.Forms.TextBox();
            this.SourceLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SetFolderSource = new System.Windows.Forms.Button();
            this.SetFolderDestination = new System.Windows.Forms.Button();
            this.SetFolderReject = new System.Windows.Forms.Button();
            this.RejectLabel = new System.Windows.Forms.Label();
            this.RejectFolderTextbox = new System.Windows.Forms.TextBox();
            this.RejectfolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.MainPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // UpdateNamingButton
            // 
            this.UpdateNamingButton.Location = new System.Drawing.Point(12, 128);
            this.UpdateNamingButton.Name = "UpdateNamingButton";
            this.UpdateNamingButton.Size = new System.Drawing.Size(170, 23);
            this.UpdateNamingButton.TabIndex = 0;
            this.UpdateNamingButton.Text = "Update Naming List";
            this.UpdateNamingButton.UseVisualStyleBackColor = true;
            this.UpdateNamingButton.Click += new System.EventHandler(this.UpdateNamingButton_Click);
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.dataGridViewPreview);
            this.MainPanel.Location = new System.Drawing.Point(185, 129);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(420, 267);
            this.MainPanel.TabIndex = 2;
            // 
            // dataGridViewPreview
            // 
            this.dataGridViewPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPreview.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewPreview.Name = "dataGridViewPreview";
            this.dataGridViewPreview.Size = new System.Drawing.Size(413, 261);
            this.dataGridViewPreview.TabIndex = 8;
            // 
            // SyncProgress
            // 
            this.SyncProgress.Location = new System.Drawing.Point(12, 402);
            this.SyncProgress.Name = "SyncProgress";
            this.SyncProgress.Size = new System.Drawing.Size(593, 13);
            this.SyncProgress.TabIndex = 3;
            // 
            // OneFileLabel
            // 
            this.OneFileLabel.AutoSize = true;
            this.OneFileLabel.Location = new System.Drawing.Point(12, 421);
            this.OneFileLabel.Name = "OneFileLabel";
            this.OneFileLabel.Size = new System.Drawing.Size(13, 13);
            this.OneFileLabel.TabIndex = 4;
            this.OneFileLabel.Text = "0";
            // 
            // TotalFilesLabel
            // 
            this.TotalFilesLabel.AutoSize = true;
            this.TotalFilesLabel.Location = new System.Drawing.Point(570, 421);
            this.TotalFilesLabel.Name = "TotalFilesLabel";
            this.TotalFilesLabel.Size = new System.Drawing.Size(0, 13);
            this.TotalFilesLabel.TabIndex = 5;
            // 
            // HashAll
            // 
            this.HashAll.Location = new System.Drawing.Point(245, 441);
            this.HashAll.Name = "HashAll";
            this.HashAll.Size = new System.Drawing.Size(131, 23);
            this.HashAll.TabIndex = 7;
            this.HashAll.Text = "Sync Media";
            this.HashAll.UseVisualStyleBackColor = true;
            this.HashAll.Click += new System.EventHandler(this.HashAll_Click);
            // 
            // FilesNamesToInclude
            // 
            this.FilesNamesToInclude.CheckOnClick = true;
            this.FilesNamesToInclude.FormattingEnabled = true;
            this.FilesNamesToInclude.Location = new System.Drawing.Point(12, 167);
            this.FilesNamesToInclude.Name = "FilesNamesToInclude";
            this.FilesNamesToInclude.Size = new System.Drawing.Size(170, 229);
            this.FilesNamesToInclude.TabIndex = 8;
            // 
            // FileNameLabel
            // 
            this.FileNameLabel.AutoSize = true;
            this.FileNameLabel.Location = new System.Drawing.Point(66, 152);
            this.FileNameLabel.Name = "FileNameLabel";
            this.FileNameLabel.Size = new System.Drawing.Size(62, 13);
            this.FileNameLabel.TabIndex = 9;
            this.FileNameLabel.Text = "Naming List";
            // 
            // SourceFolderTextbox
            // 
            this.SourceFolderTextbox.Location = new System.Drawing.Point(12, 18);
            this.SourceFolderTextbox.Name = "SourceFolderTextbox";
            this.SourceFolderTextbox.Size = new System.Drawing.Size(542, 20);
            this.SourceFolderTextbox.TabIndex = 10;
            this.SourceFolderTextbox.Click += new System.EventHandler(this.SourceFolderTextbox_Clicked);
            // 
            // DestinationFolderTextbox
            // 
            this.DestinationFolderTextbox.Location = new System.Drawing.Point(12, 58);
            this.DestinationFolderTextbox.Name = "DestinationFolderTextbox";
            this.DestinationFolderTextbox.Size = new System.Drawing.Size(542, 20);
            this.DestinationFolderTextbox.TabIndex = 11;
            this.DestinationFolderTextbox.Click += new System.EventHandler(this.DestinationFolderTextbox_Clicked);
            // 
            // SourceLabel
            // 
            this.SourceLabel.AutoSize = true;
            this.SourceLabel.Location = new System.Drawing.Point(6, 4);
            this.SourceLabel.Name = "SourceLabel";
            this.SourceLabel.Size = new System.Drawing.Size(73, 13);
            this.SourceLabel.TabIndex = 12;
            this.SourceLabel.Text = "Source Folder";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(156, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Destination Folder [My Pictures]";
            // 
            // SetFolderSource
            // 
            this.SetFolderSource.Location = new System.Drawing.Point(556, 17);
            this.SetFolderSource.Name = "SetFolderSource";
            this.SetFolderSource.Size = new System.Drawing.Size(49, 22);
            this.SetFolderSource.TabIndex = 16;
            this.SetFolderSource.Text = "Set";
            this.SetFolderSource.UseVisualStyleBackColor = true;
            this.SetFolderSource.Click += new System.EventHandler(this.SetFolderSource_Click);
            // 
            // SetFolderDestination
            // 
            this.SetFolderDestination.Location = new System.Drawing.Point(556, 57);
            this.SetFolderDestination.Name = "SetFolderDestination";
            this.SetFolderDestination.Size = new System.Drawing.Size(49, 22);
            this.SetFolderDestination.TabIndex = 17;
            this.SetFolderDestination.Text = "Set";
            this.SetFolderDestination.UseVisualStyleBackColor = true;
            this.SetFolderDestination.Click += new System.EventHandler(this.SetFolderDestination_Click);
            // 
            // SetFolderReject
            // 
            this.SetFolderReject.Location = new System.Drawing.Point(556, 98);
            this.SetFolderReject.Name = "SetFolderReject";
            this.SetFolderReject.Size = new System.Drawing.Size(49, 22);
            this.SetFolderReject.TabIndex = 20;
            this.SetFolderReject.Text = "Set";
            this.SetFolderReject.UseVisualStyleBackColor = true;
            this.SetFolderReject.Click += new System.EventHandler(this.SetFolderReject_Click);
            // 
            // RejectLabel
            // 
            this.RejectLabel.AutoSize = true;
            this.RejectLabel.Location = new System.Drawing.Point(5, 84);
            this.RejectLabel.Name = "RejectLabel";
            this.RejectLabel.Size = new System.Drawing.Size(142, 13);
            this.RejectLabel.TabIndex = 19;
            this.RejectLabel.Text = "Reject Duplicate Files Folder";
            // 
            // RejectFolderTextbox
            // 
            this.RejectFolderTextbox.Location = new System.Drawing.Point(12, 99);
            this.RejectFolderTextbox.Name = "RejectFolderTextbox";
            this.RejectFolderTextbox.Size = new System.Drawing.Size(542, 20);
            this.RejectFolderTextbox.TabIndex = 18;
            this.RejectFolderTextbox.Click += new System.EventHandler(this.RejectFolderTextbox_Clicked);
            // 
            // SyncMedia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 476);
            this.Controls.Add(this.SetFolderReject);
            this.Controls.Add(this.RejectLabel);
            this.Controls.Add(this.RejectFolderTextbox);
            this.Controls.Add(this.SetFolderDestination);
            this.Controls.Add(this.SetFolderSource);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SourceLabel);
            this.Controls.Add(this.DestinationFolderTextbox);
            this.Controls.Add(this.SourceFolderTextbox);
            this.Controls.Add(this.FileNameLabel);
            this.Controls.Add(this.FilesNamesToInclude);
            this.Controls.Add(this.HashAll);
            this.Controls.Add(this.TotalFilesLabel);
            this.Controls.Add(this.OneFileLabel);
            this.Controls.Add(this.SyncProgress);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.UpdateNamingButton);
            this.Name = "SyncMedia";
            this.Text = "Sync Media";
            this.Load += new System.EventHandler(this.SyncMedia_Load);
            this.MainPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button UpdateNamingButton;
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.DataGridView dataGridViewPreview;
        private System.Windows.Forms.ProgressBar SyncProgress;
        private System.Windows.Forms.Label OneFileLabel;
        private System.Windows.Forms.Label TotalFilesLabel;
        private System.Windows.Forms.Button HashAll;
        private System.Windows.Forms.CheckedListBox FilesNamesToInclude;
        private System.Windows.Forms.Label FileNameLabel;
        private System.Windows.Forms.FolderBrowserDialog SourcefolderBrowser;
        private System.Windows.Forms.FolderBrowserDialog DestinationfolderBrowser;
        private System.Windows.Forms.TextBox SourceFolderTextbox;
        private System.Windows.Forms.TextBox DestinationFolderTextbox;
        private System.Windows.Forms.Label SourceLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SetFolderSource;
        private System.Windows.Forms.Button SetFolderDestination;
        private System.Windows.Forms.Button SetFolderReject;
        private System.Windows.Forms.Label RejectLabel;
        private System.Windows.Forms.TextBox RejectFolderTextbox;
        private System.Windows.Forms.FolderBrowserDialog RejectfolderBrowser;
    }
}

