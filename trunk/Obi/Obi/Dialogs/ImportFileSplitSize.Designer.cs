namespace Obi.Dialogs
{
    partial class ImportFileSplitSize
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportFileSplitSize));
            this.label1 = new System.Windows.Forms.Label();
            this.mPhraseSizeTextBox = new System.Windows.Forms.TextBox();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.mCreateAudioFilePerSectionCheckBox = new System.Windows.Forms.CheckBox();
            this.lstManualArrange = new System.Windows.Forms.ListBox();
            this.m_btnMoveUp = new System.Windows.Forms.Button();
            this.m_btnMoveDown = new System.Windows.Forms.Button();
            this.m_btnAdd = new System.Windows.Forms.Button();
            this.m_grpAddFiles = new System.Windows.Forms.GroupBox();
            this.m_btnRemove = new System.Windows.Forms.Button();
            this.m_grpArrangeAudioFiles = new System.Windows.Forms.GroupBox();
            this.mbtnDesendingOrder = new System.Windows.Forms.Button();
            this.mbtnAscendingOrder = new System.Windows.Forms.Button();
            this.m_txtCharToReplaceWithSpace = new System.Windows.Forms.TextBox();
            this.m_txtPageIdentificationString = new System.Windows.Forms.TextBox();
            this.m_numCharCountToTruncateFromStart = new System.Windows.Forms.NumericUpDown();
            this.m_grpCreateSectionForEachAudioFile = new System.Windows.Forms.GroupBox();
            this.mchktPageIdentificationString = new System.Windows.Forms.CheckBox();
            this.mchkCountToTruncateFromStart = new System.Windows.Forms.CheckBox();
            this.mchkToReplaceWithSpace = new System.Windows.Forms.CheckBox();
            this.m_grpSplitPhraseOrPhraseDetection = new System.Windows.Forms.GroupBox();
            this.m_rdbPhraseDetectionOnImportedFiles = new System.Windows.Forms.RadioButton();
            this.m_rdbSplitPhrasesOnImport = new System.Windows.Forms.RadioButton();
            this.helpProvider1 = new System.Windows.Forms.HelpProvider();
            this.m_grpAddFiles.SuspendLayout();
            this.m_grpArrangeAudioFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numCharCountToTruncateFromStart)).BeginInit();
            this.m_grpCreateSectionForEachAudioFile.SuspendLayout();
            this.m_grpSplitPhraseOrPhraseDetection.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // mPhraseSizeTextBox
            // 
            resources.ApplyResources(this.mPhraseSizeTextBox, "mPhraseSizeTextBox");
            this.mPhraseSizeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPhraseSizeTextBox.Name = "mPhraseSizeTextBox";
            // 
            // mOKButton
            // 
            this.mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.mOKButton, "mOKButton");
            this.mOKButton.Name = "mOKButton";
            this.mOKButton.UseVisualStyleBackColor = true;
            this.mOKButton.Click += new System.EventHandler(this.mOKButton_Click);
            // 
            // mCancelButton
            // 
            this.mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.mCancelButton, "mCancelButton");
            this.mCancelButton.Name = "mCancelButton";
            this.mCancelButton.UseVisualStyleBackColor = true;
            // 
            // mCreateAudioFilePerSectionCheckBox
            // 
            resources.ApplyResources(this.mCreateAudioFilePerSectionCheckBox, "mCreateAudioFilePerSectionCheckBox");
            this.mCreateAudioFilePerSectionCheckBox.Name = "mCreateAudioFilePerSectionCheckBox";
            this.mCreateAudioFilePerSectionCheckBox.UseVisualStyleBackColor = true;
            this.mCreateAudioFilePerSectionCheckBox.CheckedChanged += new System.EventHandler(this.mCreateAudioFilePerSectionCheckBox_CheckedChanged);
            // 
            // lstManualArrange
            // 
            resources.ApplyResources(this.lstManualArrange, "lstManualArrange");
            this.lstManualArrange.FormattingEnabled = true;
            this.lstManualArrange.Name = "lstManualArrange";
            this.lstManualArrange.SelectedIndexChanged += new System.EventHandler(this.lstManualArrange_SelectedIndexChanged);
            // 
            // m_btnMoveUp
            // 
            resources.ApplyResources(this.m_btnMoveUp, "m_btnMoveUp");
            this.m_btnMoveUp.Name = "m_btnMoveUp";
            this.m_btnMoveUp.UseVisualStyleBackColor = true;
            this.m_btnMoveUp.Click += new System.EventHandler(this.m_btnMoveUp_Click);
            // 
            // m_btnMoveDown
            // 
            resources.ApplyResources(this.m_btnMoveDown, "m_btnMoveDown");
            this.m_btnMoveDown.Name = "m_btnMoveDown";
            this.m_btnMoveDown.UseVisualStyleBackColor = true;
            this.m_btnMoveDown.Click += new System.EventHandler(this.m_btnMoveDown_Click);
            // 
            // m_btnAdd
            // 
            resources.ApplyResources(this.m_btnAdd, "m_btnAdd");
            this.m_btnAdd.Name = "m_btnAdd";
            this.m_btnAdd.UseVisualStyleBackColor = true;
            this.m_btnAdd.Click += new System.EventHandler(this.m_btnAdd_Click);
            // 
            // m_grpAddFiles
            // 
            resources.ApplyResources(this.m_grpAddFiles, "m_grpAddFiles");
            this.m_grpAddFiles.Controls.Add(this.m_btnRemove);
            this.m_grpAddFiles.Controls.Add(this.m_grpArrangeAudioFiles);
            this.m_grpAddFiles.Controls.Add(this.lstManualArrange);
            this.m_grpAddFiles.Controls.Add(this.m_btnAdd);
            this.m_grpAddFiles.Controls.Add(this.m_btnMoveUp);
            this.m_grpAddFiles.Controls.Add(this.m_btnMoveDown);
            this.m_grpAddFiles.Name = "m_grpAddFiles";
            this.m_grpAddFiles.TabStop = false;
            // 
            // m_btnRemove
            // 
            resources.ApplyResources(this.m_btnRemove, "m_btnRemove");
            this.m_btnRemove.Name = "m_btnRemove";
            this.m_btnRemove.UseVisualStyleBackColor = true;
            this.m_btnRemove.Click += new System.EventHandler(this.m_btnRemove_Click);
            // 
            // m_grpArrangeAudioFiles
            // 
            resources.ApplyResources(this.m_grpArrangeAudioFiles, "m_grpArrangeAudioFiles");
            this.m_grpArrangeAudioFiles.Controls.Add(this.mbtnDesendingOrder);
            this.m_grpArrangeAudioFiles.Controls.Add(this.mbtnAscendingOrder);
            this.m_grpArrangeAudioFiles.Name = "m_grpArrangeAudioFiles";
            this.m_grpArrangeAudioFiles.TabStop = false;
            // 
            // mbtnDesendingOrder
            // 
            resources.ApplyResources(this.mbtnDesendingOrder, "mbtnDesendingOrder");
            this.mbtnDesendingOrder.Name = "mbtnDesendingOrder";
            this.mbtnDesendingOrder.UseVisualStyleBackColor = true;
            this.mbtnDesendingOrder.Click += new System.EventHandler(this.mbtnDesendingOrder_Click);
            // 
            // mbtnAscendingOrder
            // 
            resources.ApplyResources(this.mbtnAscendingOrder, "mbtnAscendingOrder");
            this.mbtnAscendingOrder.Name = "mbtnAscendingOrder";
            this.mbtnAscendingOrder.UseVisualStyleBackColor = true;
            this.mbtnAscendingOrder.Click += new System.EventHandler(this.mbtnAscendingOrder_Click);
            // 
            // m_txtCharToReplaceWithSpace
            // 
            resources.ApplyResources(this.m_txtCharToReplaceWithSpace, "m_txtCharToReplaceWithSpace");
            this.m_txtCharToReplaceWithSpace.Name = "m_txtCharToReplaceWithSpace";
            // 
            // m_txtPageIdentificationString
            // 
            resources.ApplyResources(this.m_txtPageIdentificationString, "m_txtPageIdentificationString");
            this.m_txtPageIdentificationString.Name = "m_txtPageIdentificationString";
            // 
            // m_numCharCountToTruncateFromStart
            // 
            resources.ApplyResources(this.m_numCharCountToTruncateFromStart, "m_numCharCountToTruncateFromStart");
            this.m_numCharCountToTruncateFromStart.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.m_numCharCountToTruncateFromStart.Name = "m_numCharCountToTruncateFromStart";
            // 
            // m_grpCreateSectionForEachAudioFile
            // 
            resources.ApplyResources(this.m_grpCreateSectionForEachAudioFile, "m_grpCreateSectionForEachAudioFile");
            this.m_grpCreateSectionForEachAudioFile.Controls.Add(this.mchktPageIdentificationString);
            this.m_grpCreateSectionForEachAudioFile.Controls.Add(this.mchkCountToTruncateFromStart);
            this.m_grpCreateSectionForEachAudioFile.Controls.Add(this.mchkToReplaceWithSpace);
            this.m_grpCreateSectionForEachAudioFile.Controls.Add(this.mCreateAudioFilePerSectionCheckBox);
            this.m_grpCreateSectionForEachAudioFile.Controls.Add(this.m_txtCharToReplaceWithSpace);
            this.m_grpCreateSectionForEachAudioFile.Controls.Add(this.m_numCharCountToTruncateFromStart);
            this.m_grpCreateSectionForEachAudioFile.Controls.Add(this.m_txtPageIdentificationString);
            this.m_grpCreateSectionForEachAudioFile.Name = "m_grpCreateSectionForEachAudioFile";
            this.m_grpCreateSectionForEachAudioFile.TabStop = false;
            // 
            // mchktPageIdentificationString
            // 
            resources.ApplyResources(this.mchktPageIdentificationString, "mchktPageIdentificationString");
            this.mchktPageIdentificationString.Name = "mchktPageIdentificationString";
            this.mchktPageIdentificationString.UseVisualStyleBackColor = true;
            this.mchktPageIdentificationString.CheckedChanged += new System.EventHandler(this.mchktPageIdentificationString_CheckedChanged);
            // 
            // mchkCountToTruncateFromStart
            // 
            resources.ApplyResources(this.mchkCountToTruncateFromStart, "mchkCountToTruncateFromStart");
            this.mchkCountToTruncateFromStart.Name = "mchkCountToTruncateFromStart";
            this.mchkCountToTruncateFromStart.UseVisualStyleBackColor = true;
            this.mchkCountToTruncateFromStart.CheckedChanged += new System.EventHandler(this.mchkCountToTruncateFromStart_CheckedChanged);
            // 
            // mchkToReplaceWithSpace
            // 
            resources.ApplyResources(this.mchkToReplaceWithSpace, "mchkToReplaceWithSpace");
            this.mchkToReplaceWithSpace.Name = "mchkToReplaceWithSpace";
            this.mchkToReplaceWithSpace.UseVisualStyleBackColor = true;
            this.mchkToReplaceWithSpace.CheckedChanged += new System.EventHandler(this.mchkToReplaceWithSpace_CheckedChanged);
            // 
            // m_grpSplitPhraseOrPhraseDetection
            // 
            this.m_grpSplitPhraseOrPhraseDetection.Controls.Add(this.m_rdbPhraseDetectionOnImportedFiles);
            this.m_grpSplitPhraseOrPhraseDetection.Controls.Add(this.m_rdbSplitPhrasesOnImport);
            this.m_grpSplitPhraseOrPhraseDetection.Controls.Add(this.label1);
            this.m_grpSplitPhraseOrPhraseDetection.Controls.Add(this.mPhraseSizeTextBox);
            resources.ApplyResources(this.m_grpSplitPhraseOrPhraseDetection, "m_grpSplitPhraseOrPhraseDetection");
            this.m_grpSplitPhraseOrPhraseDetection.Name = "m_grpSplitPhraseOrPhraseDetection";
            this.m_grpSplitPhraseOrPhraseDetection.TabStop = false;
            // 
            // m_rdbPhraseDetectionOnImportedFiles
            // 
            resources.ApplyResources(this.m_rdbPhraseDetectionOnImportedFiles, "m_rdbPhraseDetectionOnImportedFiles");
            this.m_rdbPhraseDetectionOnImportedFiles.Checked = true;
            this.m_rdbPhraseDetectionOnImportedFiles.Name = "m_rdbPhraseDetectionOnImportedFiles";
            this.m_rdbPhraseDetectionOnImportedFiles.TabStop = true;
            this.m_rdbPhraseDetectionOnImportedFiles.UseVisualStyleBackColor = true;
            // 
            // m_rdbSplitPhrasesOnImport
            // 
            resources.ApplyResources(this.m_rdbSplitPhrasesOnImport, "m_rdbSplitPhrasesOnImport");
            this.m_rdbSplitPhrasesOnImport.Name = "m_rdbSplitPhrasesOnImport";
            this.m_rdbSplitPhrasesOnImport.UseVisualStyleBackColor = true;
            this.m_rdbSplitPhrasesOnImport.CheckedChanged += new System.EventHandler(this.m_rdbSplitPhrasesOnImport_CheckedChanged);
            // 
            // helpProvider1
            // 
            resources.ApplyResources(this.helpProvider1, "helpProvider1");
            // 
            // ImportFileSplitSize
            // 
            this.AcceptButton = this.mOKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.Controls.Add(this.m_grpSplitPhraseOrPhraseDetection);
            this.Controls.Add(this.m_grpCreateSectionForEachAudioFile);
            this.Controls.Add(this.m_grpAddFiles);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportFileSplitSize";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ImportFileSplitSize_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImportFileSplitSize_FormClosing);
            this.m_grpAddFiles.ResumeLayout(false);
            this.m_grpArrangeAudioFiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_numCharCountToTruncateFromStart)).EndInit();
            this.m_grpCreateSectionForEachAudioFile.ResumeLayout(false);
            this.m_grpCreateSectionForEachAudioFile.PerformLayout();
            this.m_grpSplitPhraseOrPhraseDetection.ResumeLayout(false);
            this.m_grpSplitPhraseOrPhraseDetection.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mPhraseSizeTextBox;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.CheckBox mCreateAudioFilePerSectionCheckBox;
        private System.Windows.Forms.ListBox lstManualArrange;
        private System.Windows.Forms.Button m_btnMoveUp;
        private System.Windows.Forms.Button m_btnMoveDown;
        private System.Windows.Forms.Button m_btnAdd;
        private System.Windows.Forms.GroupBox m_grpAddFiles;
        private System.Windows.Forms.TextBox m_txtCharToReplaceWithSpace;
        private System.Windows.Forms.TextBox m_txtPageIdentificationString;
        private System.Windows.Forms.NumericUpDown m_numCharCountToTruncateFromStart;
        private System.Windows.Forms.GroupBox m_grpCreateSectionForEachAudioFile;
        private System.Windows.Forms.GroupBox m_grpSplitPhraseOrPhraseDetection;
        private System.Windows.Forms.RadioButton m_rdbSplitPhrasesOnImport;
        private System.Windows.Forms.RadioButton m_rdbPhraseDetectionOnImportedFiles;
        private System.Windows.Forms.GroupBox m_grpArrangeAudioFiles;
        private System.Windows.Forms.Button mbtnDesendingOrder;
        private System.Windows.Forms.Button mbtnAscendingOrder;
        private System.Windows.Forms.Button m_btnRemove;
        private System.Windows.Forms.HelpProvider helpProvider1;
        private System.Windows.Forms.CheckBox mchkToReplaceWithSpace;
        private System.Windows.Forms.CheckBox mchktPageIdentificationString;
        private System.Windows.Forms.CheckBox mchkCountToTruncateFromStart;
    }
}