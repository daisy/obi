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
            this.m_rdbDaisy3 = new System.Windows.Forms.RadioButton();
            this.m_rdbDaisy202 = new System.Windows.Forms.RadioButton();
            this.m_grpDaisyExport = new System.Windows.Forms.GroupBox();
            this.m_grpDaisyExport.SuspendLayout();
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
            // m_rdbDaisy3
            // 
            resources.ApplyResources(this.m_rdbDaisy3, "m_rdbDaisy3");
            this.m_rdbDaisy3.Checked = true;
            this.m_rdbDaisy3.Name = "m_rdbDaisy3";
            this.m_rdbDaisy3.TabStop = true;
            this.m_rdbDaisy3.UseVisualStyleBackColor = true;
            // 
            // m_rdbDaisy202
            // 
            resources.ApplyResources(this.m_rdbDaisy202, "m_rdbDaisy202");
            this.m_rdbDaisy202.Name = "m_rdbDaisy202";
            this.m_rdbDaisy202.UseVisualStyleBackColor = true;
            // 
            // m_grpDaisyExport
            // 
            this.m_grpDaisyExport.Controls.Add(this.m_rdbDaisy3);
            this.m_grpDaisyExport.Controls.Add(this.m_rdbDaisy202);
            resources.ApplyResources(this.m_grpDaisyExport, "m_grpDaisyExport");
            this.m_grpDaisyExport.Name = "m_grpDaisyExport";
            this.m_grpDaisyExport.TabStop = false;            
            // 
            // ExportDirectory
            // 
            this.AcceptButton = this.mOKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.Controls.Add(this.m_grpDaisyExport);
            this.Controls.Add(this.m_ComboSelectLevelForAudioFiles);
            this.Controls.Add(this.m_lblSelectLevelForAudioFiles);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.Controls.Add(this.mSelectButton);
            this.Controls.Add(this.mPathTextBox);
            this.Controls.Add(this.m_lblDirectoryPath);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportDirectory";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectDirectoryPath_FormClosing);
            this.m_grpDaisyExport.ResumeLayout(false);
            this.m_grpDaisyExport.PerformLayout();
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
        private System.Windows.Forms.RadioButton m_rdbDaisy3;
        private System.Windows.Forms.RadioButton m_rdbDaisy202;
        private System.Windows.Forms.GroupBox m_grpDaisyExport;
    }
}