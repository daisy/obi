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
            this.m_checkBoxEncoder = new System.Windows.Forms.CheckBox();
            this.m_Bitrate = new System.Windows.Forms.Label();
            this.m_ComboBoxBitrate = new System.Windows.Forms.ComboBox();
            this.m_checkBoxAddSectionNameToAudioFileName = new System.Windows.Forms.CheckBox();
            this.m_grpBoxMP3Encoding = new System.Windows.Forms.GroupBox();
            this.m_comboBoxEncodingType = new System.Windows.Forms.ComboBox();
            this.m_btnAdvance = new System.Windows.Forms.Button();
            this.m_grpBoxSectionNameOperation = new System.Windows.Forms.GroupBox();
            this.m_chkBoxFilenameLengthLimit = new System.Windows.Forms.CheckBox();
            this.m_numericUpDownFilenameLengthLimit = new System.Windows.Forms.NumericUpDown();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_chkBoxDummyTextHTMLfiles = new System.Windows.Forms.CheckBox();
            this.m_EpubFileNamegroupBox = new System.Windows.Forms.GroupBox();
            this.m_numericUpDownEpubFilenameLengthLimit = new System.Windows.Forms.NumericUpDown();
            this.m_chkBoxEpubFilenameLengthLimit = new System.Windows.Forms.CheckBox();
            this.m_grpBoxMP3Encoding.SuspendLayout();
            this.m_grpBoxSectionNameOperation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDownFilenameLengthLimit)).BeginInit();
            this.m_EpubFileNamegroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDownEpubFilenameLengthLimit)).BeginInit();
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
            // m_checkBoxEncoder
            // 
            resources.ApplyResources(this.m_checkBoxEncoder, "m_checkBoxEncoder");
            this.m_checkBoxEncoder.Name = "m_checkBoxEncoder";
            this.m_checkBoxEncoder.UseVisualStyleBackColor = true;
            this.m_checkBoxEncoder.CheckedChanged += new System.EventHandler(this.m_checkBoxMP3Encoder_CheckedChanged);
            // 
            // m_Bitrate
            // 
            resources.ApplyResources(this.m_Bitrate, "m_Bitrate");
            this.m_Bitrate.Name = "m_Bitrate";
            // 
            // m_ComboBoxBitrate
            // 
            this.m_ComboBoxBitrate.FormattingEnabled = true;
            this.m_ComboBoxBitrate.Items.AddRange(new object[] {
            resources.GetString("m_ComboBoxBitrate.Items"),
            resources.GetString("m_ComboBoxBitrate.Items1"),
            resources.GetString("m_ComboBoxBitrate.Items2"),
            resources.GetString("m_ComboBoxBitrate.Items3"),
            resources.GetString("m_ComboBoxBitrate.Items4"),
            resources.GetString("m_ComboBoxBitrate.Items5"),
            resources.GetString("m_ComboBoxBitrate.Items6"),
            resources.GetString("m_ComboBoxBitrate.Items7")});
            resources.ApplyResources(this.m_ComboBoxBitrate, "m_ComboBoxBitrate");
            this.m_ComboBoxBitrate.Name = "m_ComboBoxBitrate";
            this.m_ComboBoxBitrate.SelectedIndexChanged += new System.EventHandler(this.m_ComboBoxBitrate_SelectedIndexChanged);
            // 
            // m_checkBoxAddSectionNameToAudioFileName
            // 
            resources.ApplyResources(this.m_checkBoxAddSectionNameToAudioFileName, "m_checkBoxAddSectionNameToAudioFileName");
            this.m_checkBoxAddSectionNameToAudioFileName.Name = "m_checkBoxAddSectionNameToAudioFileName";
            this.m_checkBoxAddSectionNameToAudioFileName.UseVisualStyleBackColor = true;
            this.m_checkBoxAddSectionNameToAudioFileName.CheckedChanged += new System.EventHandler(this.m_checkBoxAddSectionNameToAudioFileName_CheckedChanged);
            // 
            // m_grpBoxMP3Encoding
            // 
            this.m_grpBoxMP3Encoding.Controls.Add(this.m_comboBoxEncodingType);
            this.m_grpBoxMP3Encoding.Controls.Add(this.m_btnAdvance);
            this.m_grpBoxMP3Encoding.Controls.Add(this.m_Bitrate);
            this.m_grpBoxMP3Encoding.Controls.Add(this.m_ComboBoxBitrate);
            this.m_grpBoxMP3Encoding.Controls.Add(this.m_checkBoxEncoder);
            resources.ApplyResources(this.m_grpBoxMP3Encoding, "m_grpBoxMP3Encoding");
            this.m_grpBoxMP3Encoding.Name = "m_grpBoxMP3Encoding";
            this.m_grpBoxMP3Encoding.TabStop = false;
            // 
            // m_comboBoxEncodingType
            // 
            resources.ApplyResources(this.m_comboBoxEncodingType, "m_comboBoxEncodingType");
            this.m_comboBoxEncodingType.FormattingEnabled = true;
            this.m_comboBoxEncodingType.Items.AddRange(new object[] {
            resources.GetString("m_comboBoxEncodingType.Items"),
            resources.GetString("m_comboBoxEncodingType.Items1"),
            resources.GetString("m_comboBoxEncodingType.Items2"),
            resources.GetString("m_comboBoxEncodingType.Items3")});
            this.m_comboBoxEncodingType.Name = "m_comboBoxEncodingType";
            this.m_comboBoxEncodingType.SelectedIndexChanged += new System.EventHandler(this.m_comboBoxEncodingType_SelectedIndexChanged);
            // 
            // m_btnAdvance
            // 
            resources.ApplyResources(this.m_btnAdvance, "m_btnAdvance");
            this.m_btnAdvance.Name = "m_btnAdvance";
            this.m_btnAdvance.UseVisualStyleBackColor = true;
            this.m_btnAdvance.Click += new System.EventHandler(this.mbtnAdvance_Click);
            // 
            // m_grpBoxSectionNameOperation
            // 
            this.m_grpBoxSectionNameOperation.Controls.Add(this.m_chkBoxFilenameLengthLimit);
            this.m_grpBoxSectionNameOperation.Controls.Add(this.m_numericUpDownFilenameLengthLimit);
            this.m_grpBoxSectionNameOperation.Controls.Add(this.m_checkBoxAddSectionNameToAudioFileName);
            resources.ApplyResources(this.m_grpBoxSectionNameOperation, "m_grpBoxSectionNameOperation");
            this.m_grpBoxSectionNameOperation.Name = "m_grpBoxSectionNameOperation";
            this.m_grpBoxSectionNameOperation.TabStop = false;
            // 
            // m_chkBoxFilenameLengthLimit
            // 
            resources.ApplyResources(this.m_chkBoxFilenameLengthLimit, "m_chkBoxFilenameLengthLimit");
            this.m_chkBoxFilenameLengthLimit.Name = "m_chkBoxFilenameLengthLimit";
            this.m_chkBoxFilenameLengthLimit.UseVisualStyleBackColor = true;
            this.m_chkBoxFilenameLengthLimit.CheckedChanged += new System.EventHandler(this.m_chkBoxFilenameLengthLimit_CheckedChanged);
            // 
            // m_numericUpDownFilenameLengthLimit
            // 
            resources.ApplyResources(this.m_numericUpDownFilenameLengthLimit, "m_numericUpDownFilenameLengthLimit");
            this.m_numericUpDownFilenameLengthLimit.Name = "m_numericUpDownFilenameLengthLimit";
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // m_chkBoxDummyTextHTMLfiles
            // 
            resources.ApplyResources(this.m_chkBoxDummyTextHTMLfiles, "m_chkBoxDummyTextHTMLfiles");
            this.m_chkBoxDummyTextHTMLfiles.Name = "m_chkBoxDummyTextHTMLfiles";
            this.helpProvider1.SetShowHelp(this.m_chkBoxDummyTextHTMLfiles, ((bool)(resources.GetObject("m_chkBoxDummyTextHTMLfiles.ShowHelp"))));
            this.m_chkBoxDummyTextHTMLfiles.UseVisualStyleBackColor = true;
            // 
            // m_EpubFileNamegroupBox
            // 
            this.m_EpubFileNamegroupBox.Controls.Add(this.m_numericUpDownEpubFilenameLengthLimit);
            this.m_EpubFileNamegroupBox.Controls.Add(this.m_chkBoxEpubFilenameLengthLimit);
            resources.ApplyResources(this.m_EpubFileNamegroupBox, "m_EpubFileNamegroupBox");
            this.m_EpubFileNamegroupBox.Name = "m_EpubFileNamegroupBox";
            this.m_EpubFileNamegroupBox.TabStop = false;
            // 
            // m_numericUpDownEpubFilenameLengthLimit
            // 
            resources.ApplyResources(this.m_numericUpDownEpubFilenameLengthLimit, "m_numericUpDownEpubFilenameLengthLimit");
            this.m_numericUpDownEpubFilenameLengthLimit.Name = "m_numericUpDownEpubFilenameLengthLimit";
            // 
            // m_chkBoxEpubFilenameLengthLimit
            // 
            resources.ApplyResources(this.m_chkBoxEpubFilenameLengthLimit, "m_chkBoxEpubFilenameLengthLimit");
            this.m_chkBoxEpubFilenameLengthLimit.Name = "m_chkBoxEpubFilenameLengthLimit";
            this.m_chkBoxEpubFilenameLengthLimit.UseVisualStyleBackColor = true;
            this.m_chkBoxEpubFilenameLengthLimit.CheckedChanged += new System.EventHandler(this.m_chkBoxEpubFilenameLengthLimit_CheckedChanged);
            // 
            // ExportDirectory
            // 
            this.AcceptButton = this.mOKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.Controls.Add(this.m_chkBoxDummyTextHTMLfiles);
            this.Controls.Add(this.m_EpubFileNamegroupBox);
            this.Controls.Add(this.m_grpBoxSectionNameOperation);
            this.Controls.Add(this.m_grpBoxMP3Encoding);
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
            this.m_grpBoxMP3Encoding.ResumeLayout(false);
            this.m_grpBoxMP3Encoding.PerformLayout();
            this.m_grpBoxSectionNameOperation.ResumeLayout(false);
            this.m_grpBoxSectionNameOperation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDownFilenameLengthLimit)).EndInit();
            this.m_EpubFileNamegroupBox.ResumeLayout(false);
            this.m_EpubFileNamegroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numericUpDownEpubFilenameLengthLimit)).EndInit();
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
        private System.Windows.Forms.CheckBox m_checkBoxEncoder;
        private System.Windows.Forms.Label m_Bitrate;
        private System.Windows.Forms.ComboBox m_ComboBoxBitrate;
        private System.Windows.Forms.CheckBox m_checkBoxAddSectionNameToAudioFileName;
        private System.Windows.Forms.GroupBox m_grpBoxMP3Encoding;
        private System.Windows.Forms.GroupBox m_grpBoxSectionNameOperation;
        private System.Windows.Forms.CheckBox m_chkBoxFilenameLengthLimit;
        private System.Windows.Forms.NumericUpDown m_numericUpDownFilenameLengthLimit;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.Button m_btnAdvance;
        private System.Windows.Forms.GroupBox m_EpubFileNamegroupBox;
        private System.Windows.Forms.NumericUpDown m_numericUpDownEpubFilenameLengthLimit;
        private System.Windows.Forms.CheckBox m_chkBoxEpubFilenameLengthLimit;
        private System.Windows.Forms.CheckBox m_chkBoxDummyTextHTMLfiles;
        private System.Windows.Forms.ComboBox m_comboBoxEncodingType;
    }
}