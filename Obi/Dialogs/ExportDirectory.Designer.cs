namespace Obi.Dialogs
{
    partial class ExportDirectory
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportDirectory));
            this.m_lblDirectoryPath = new System.Windows.Forms.Label();
            this.mPathTextBox = new System.Windows.Forms.TextBox();
            this.mSelectButton = new System.Windows.Forms.Button();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.m_lblSelectLevelForAudioFiles = new System.Windows.Forms.Label();
            this.m_ComboSelectLevelForAudioFiles = new System.Windows.Forms.ComboBox();
            this.m_checkBoxMP3Encoder = new System.Windows.Forms.CheckBox();
            this.m_Bitrate = new System.Windows.Forms.Label();
            this.m_ComboBoxBitrate = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // m_lblDirectoryPath
            // 
            resources.ApplyResources(this.m_lblDirectoryPath, "m_lblDirectoryPath");
            this.m_lblDirectoryPath.Name = "m_lblDirectoryPath";
            // 
            // mPathTextBox
            // 
            resources.ApplyResources(this.mPathTextBox, "mPathTextBox");
            this.mPathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPathTextBox.Name = "mPathTextBox";
            // 
            // mSelectButton
            // 
            resources.ApplyResources(this.mSelectButton, "mSelectButton");
            this.mSelectButton.Name = "mSelectButton";
            this.mSelectButton.UseVisualStyleBackColor = true;
            this.mSelectButton.Click += new System.EventHandler(this.mSelectButton_Click);
            // 
            // mOKButton
            // 
            resources.ApplyResources(this.mOKButton, "mOKButton");
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            resources.ApplyResources(this.mCancelButton, "mCancelButton");
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // m_lblSelectLevelForAudioFiles
            // 
            resources.ApplyResources(this.m_lblSelectLevelForAudioFiles, "m_lblSelectLevelForAudioFiles");
            this.m_lblSelectLevelForAudioFiles.Name = "m_lblSelectLevelForAudioFiles";
            // 
            // m_ComboSelectLevelForAudioFiles
            // 
            resources.ApplyResources(this.m_ComboSelectLevelForAudioFiles, "m_ComboSelectLevelForAudioFiles");
            this.m_ComboSelectLevelForAudioFiles.FormattingEnabled = true;
            this.m_ComboSelectLevelForAudioFiles.Name = "m_ComboSelectLevelForAudioFiles";
            // 
            // m_checkBoxMP3Encoder
            // 
            resources.ApplyResources(this.m_checkBoxMP3Encoder, "m_checkBoxMP3Encoder");
            this.m_checkBoxMP3Encoder.Name = "m_checkBoxMP3Encoder";
            this.m_checkBoxMP3Encoder.UseVisualStyleBackColor = true;
            this.m_checkBoxMP3Encoder.CheckedChanged += new System.EventHandler(this.m_checkBoxMP3Encoder_CheckedChanged);
            // 
            // m_Bitrate
            // 
            resources.ApplyResources(this.m_Bitrate, "m_Bitrate");
            this.m_Bitrate.Name = "m_Bitrate";
            // 
            // m_ComboBoxBitrate
            // 
            resources.ApplyResources(this.m_ComboBoxBitrate, "m_ComboBoxBitrate");
            this.m_ComboBoxBitrate.FormattingEnabled = true;
            this.m_ComboBoxBitrate.Items.AddRange(new object[] {
            resources.GetString("m_ComboBoxBitrate.Items"),
            resources.GetString("m_ComboBoxBitrate.Items1"),
            resources.GetString("m_ComboBoxBitrate.Items2"),
            resources.GetString("m_ComboBoxBitrate.Items3")});
            this.m_ComboBoxBitrate.Name = "m_ComboBoxBitrate";
            this.m_ComboBoxBitrate.SelectedIndexChanged += new System.EventHandler(this.m_ComboBoxBitrate_SelectedIndexChanged);
            // 
            // ExportDirectory
            // 
            this.AcceptButton = this.mOKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.Controls.Add(this.m_ComboBoxBitrate);
            this.Controls.Add(this.m_Bitrate);
            this.Controls.Add(this.m_checkBoxMP3Encoder);
            this.Controls.Add(this.m_ComboSelectLevelForAudioFiles);
            this.Controls.Add(this.m_lblSelectLevelForAudioFiles);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mSelectButton);
            this.Controls.Add(this.mPathTextBox);
            this.Controls.Add(this.m_lblDirectoryPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportDirectory";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectDirectoryPath_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label m_lblDirectoryPath;
        private System.Windows.Forms.TextBox mPathTextBox;
        private System.Windows.Forms.Button mSelectButton;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Label m_lblSelectLevelForAudioFiles;
        private System.Windows.Forms.ComboBox m_ComboSelectLevelForAudioFiles;
        private System.Windows.Forms.CheckBox m_checkBoxMP3Encoder;
        private System.Windows.Forms.Label m_Bitrate;
        private System.Windows.Forms.ComboBox m_ComboBoxBitrate;
    }
}