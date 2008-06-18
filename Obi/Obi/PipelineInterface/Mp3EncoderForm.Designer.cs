namespace PipelineInterface
{
    partial class Mp3EncoderForm
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
            this.m_lblInputFile = new System.Windows.Forms.Label();
            this.m_txtInputFile = new System.Windows.Forms.TextBox();
            this.m_btnBrowseInputFile = new System.Windows.Forms.Button();
            this.m_lblOutputDirectory = new System.Windows.Forms.Label();
            this.m_txtOutputDirectory = new System.Windows.Forms.TextBox();
            this.m_btnBrowseOutputDirectory = new System.Windows.Forms.Button();
            this.m_lblBitRate = new System.Windows.Forms.Label();
            this.m_comboBitRate = new System.Windows.Forms.ComboBox();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // m_lblInputFile
            // 
            this.m_lblInputFile.AutoSize = true;
            this.m_lblInputFile.Location = new System.Drawing.Point(0, 10);
            this.m_lblInputFile.Name = "m_lblInputFile";
            this.m_lblInputFile.Size = new System.Drawing.Size(72, 13);
            this.m_lblInputFile.TabIndex = 0;
            this.m_lblInputFile.Text = "&Input DTB file";
            // 
            // m_txtInputFile
            // 
            this.m_txtInputFile.AccessibleName = "Input DTB file";
            this.m_txtInputFile.Location = new System.Drawing.Point(80, 10);
            this.m_txtInputFile.Name = "m_txtInputFile";
            this.m_txtInputFile.Size = new System.Drawing.Size(150, 20);
            this.m_txtInputFile.TabIndex = 1;
            // 
            // m_btnBrowseInputFile
            // 
            this.m_btnBrowseInputFile.Location = new System.Drawing.Point(200, 35);
            this.m_btnBrowseInputFile.Name = "m_btnBrowseInputFile";
            this.m_btnBrowseInputFile.Size = new System.Drawing.Size(75, 23);
            this.m_btnBrowseInputFile.TabIndex = 2;
            this.m_btnBrowseInputFile.Text = "Browse";
            this.m_btnBrowseInputFile.UseVisualStyleBackColor = true;
            this.m_btnBrowseInputFile.Click += new System.EventHandler(this.m_btnBrowseInputFile_Click);
            // 
            // m_lblOutputDirectory
            // 
            this.m_lblOutputDirectory.AutoSize = true;
            this.m_lblOutputDirectory.Location = new System.Drawing.Point(0, 65);
            this.m_lblOutputDirectory.Name = "m_lblOutputDirectory";
            this.m_lblOutputDirectory.Size = new System.Drawing.Size(84, 13);
            this.m_lblOutputDirectory.TabIndex = 3;
            this.m_lblOutputDirectory.Text = "&Output Directory";
            // 
            // m_txtOutputDirectory
            // 
            this.m_txtOutputDirectory.AccessibleName = "Output Directory";
            this.m_txtOutputDirectory.Location = new System.Drawing.Point(80, 65);
            this.m_txtOutputDirectory.Name = "m_txtOutputDirectory";
            this.m_txtOutputDirectory.Size = new System.Drawing.Size(150, 20);
            this.m_txtOutputDirectory.TabIndex = 4;
            // 
            // m_btnBrowseOutputDirectory
            // 
            this.m_btnBrowseOutputDirectory.Location = new System.Drawing.Point(200, 90);
            this.m_btnBrowseOutputDirectory.Name = "m_btnBrowseOutputDirectory";
            this.m_btnBrowseOutputDirectory.Size = new System.Drawing.Size(75, 23);
            this.m_btnBrowseOutputDirectory.TabIndex = 5;
            this.m_btnBrowseOutputDirectory.Text = "Browse";
            this.m_btnBrowseOutputDirectory.UseVisualStyleBackColor = true;
            this.m_btnBrowseOutputDirectory.Click += new System.EventHandler(this.m_btnBrowseOutputDirectory_Click);
            // 
            // m_lblBitRate
            // 
            this.m_lblBitRate.AutoSize = true;
            this.m_lblBitRate.Location = new System.Drawing.Point(0, 100);
            this.m_lblBitRate.Name = "m_lblBitRate";
            this.m_lblBitRate.Size = new System.Drawing.Size(40, 13);
            this.m_lblBitRate.TabIndex = 6;
            this.m_lblBitRate.Text = "&Bit rate";
            // 
            // m_comboBitRate
            // 
            this.m_comboBitRate.AccessibleName = "Bit rate";
            this.m_comboBitRate.FormattingEnabled = true;
            this.m_comboBitRate.Location = new System.Drawing.Point(80, 100);
            this.m_comboBitRate.Name = "m_comboBitRate";
            this.m_comboBitRate.Size = new System.Drawing.Size(121, 21);
            this.m_comboBitRate.TabIndex = 7;
            // 
            // m_btnOK
            // 
            this.m_btnOK.Location = new System.Drawing.Point(125, 200);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 8;
            this.m_btnOK.Text = "&OK";
            this.m_btnOK.UseVisualStyleBackColor = true;
            this.m_btnOK.Click += new System.EventHandler(this.m_btnOK_Click);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(200, 200);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 9;
            this.m_btnCancel.Text = "&Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            this.m_btnCancel.Click += new System.EventHandler(this.m_btnCancel_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Title = "Open";
            // 
            // Mp3EncoderForm
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(292, 273);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_comboBitRate);
            this.Controls.Add(this.m_lblBitRate);
            this.Controls.Add(this.m_btnBrowseOutputDirectory);
            this.Controls.Add(this.m_txtOutputDirectory);
            this.Controls.Add(this.m_lblOutputDirectory);
            this.Controls.Add(this.m_btnBrowseInputFile);
            this.Controls.Add(this.m_txtInputFile);
            this.Controls.Add(this.m_lblInputFile);
            this.Name = "Mp3EncoderForm";
            this.Text = "Mp3 encoder";
            this.Load += new System.EventHandler(this.Mp3EncoderForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblInputFile;
        private System.Windows.Forms.TextBox m_txtInputFile;
        private System.Windows.Forms.Button m_btnBrowseInputFile;
        private System.Windows.Forms.Label m_lblOutputDirectory;
        private System.Windows.Forms.TextBox m_txtOutputDirectory;
        private System.Windows.Forms.Button m_btnBrowseOutputDirectory;
        private System.Windows.Forms.Label m_lblBitRate;
        private System.Windows.Forms.ComboBox m_comboBitRate;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}