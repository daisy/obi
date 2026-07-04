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
            label1 = new System.Windows.Forms.Label();
            mPhraseSizeTextBox = new System.Windows.Forms.TextBox();
            mOKButton = new System.Windows.Forms.Button();
            mCancelButton = new System.Windows.Forms.Button();
            lstManualArrange = new System.Windows.Forms.ListBox();
            m_btnMoveUp = new System.Windows.Forms.Button();
            m_btnMoveDown = new System.Windows.Forms.Button();
            m_btnAdd = new System.Windows.Forms.Button();
            m_grpAddFiles = new System.Windows.Forms.GroupBox();
            m_btnRemove = new System.Windows.Forms.Button();
            m_grpArrangeAudioFiles = new System.Windows.Forms.GroupBox();
            mbtnDesendingOrder = new System.Windows.Forms.Button();
            mbtnAscendingOrder = new System.Windows.Forms.Button();
            m_txtCharToReplaceWithSpace = new System.Windows.Forms.TextBox();
            m_txtPageIdentificationString = new System.Windows.Forms.TextBox();
            m_numCharCountToTruncateFromStart = new System.Windows.Forms.NumericUpDown();
            m_grpCreateSectionForEachAudioFile = new System.Windows.Forms.GroupBox();
            m_rdbSplitAtCuePoints = new System.Windows.Forms.RadioButton();
            m_rdbCreateAudioFilePerSection = new System.Windows.Forms.RadioButton();
            m_rdbImportAudioFileInEachSection = new System.Windows.Forms.RadioButton();
            m_rdbImportAudioInSelectedSection = new System.Windows.Forms.RadioButton();
            mchktPageIdentificationString = new System.Windows.Forms.CheckBox();
            mchkCountToTruncateFromStart = new System.Windows.Forms.CheckBox();
            mchkToReplaceWithSpace = new System.Windows.Forms.CheckBox();
            m_grpSplitPhraseOrPhraseDetection = new System.Windows.Forms.GroupBox();
            m_rdbPhraseDetectionWhisper = new System.Windows.Forms.RadioButton();
            m_rdbPhraseDetectionOnImportedFiles = new System.Windows.Forms.RadioButton();
            m_rdbSplitPhrasesOnImport = new System.Windows.Forms.RadioButton();
            helpProvider1 = new System.Windows.Forms.HelpProvider();
            m_grpAddFiles.SuspendLayout();
            m_grpArrangeAudioFiles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)m_numCharCountToTruncateFromStart).BeginInit();
            m_grpCreateSectionForEachAudioFile.SuspendLayout();
            m_grpSplitPhraseOrPhraseDetection.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // mPhraseSizeTextBox
            // 
            resources.ApplyResources(mPhraseSizeTextBox, "mPhraseSizeTextBox");
            mPhraseSizeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            mPhraseSizeTextBox.Name = "mPhraseSizeTextBox";
            // 
            // mOKButton
            // 
            mOKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(mOKButton, "mOKButton");
            mOKButton.Name = "mOKButton";
            mOKButton.UseVisualStyleBackColor = true;
            mOKButton.Click += mOKButton_Click;
            // 
            // mCancelButton
            // 
            mCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(mCancelButton, "mCancelButton");
            mCancelButton.Name = "mCancelButton";
            mCancelButton.UseVisualStyleBackColor = true;
            // 
            // lstManualArrange
            // 
            resources.ApplyResources(lstManualArrange, "lstManualArrange");
            lstManualArrange.FormattingEnabled = true;
            lstManualArrange.Name = "lstManualArrange";
            lstManualArrange.SelectedIndexChanged += lstManualArrange_SelectedIndexChanged;
            // 
            // m_btnMoveUp
            // 
            resources.ApplyResources(m_btnMoveUp, "m_btnMoveUp");
            m_btnMoveUp.Name = "m_btnMoveUp";
            m_btnMoveUp.UseVisualStyleBackColor = true;
            m_btnMoveUp.Click += m_btnMoveUp_Click;
            // 
            // m_btnMoveDown
            // 
            resources.ApplyResources(m_btnMoveDown, "m_btnMoveDown");
            m_btnMoveDown.Name = "m_btnMoveDown";
            m_btnMoveDown.UseVisualStyleBackColor = true;
            m_btnMoveDown.Click += m_btnMoveDown_Click;
            // 
            // m_btnAdd
            // 
            resources.ApplyResources(m_btnAdd, "m_btnAdd");
            m_btnAdd.Name = "m_btnAdd";
            m_btnAdd.UseVisualStyleBackColor = true;
            m_btnAdd.Click += m_btnAdd_Click;
            // 
            // m_grpAddFiles
            // 
            resources.ApplyResources(m_grpAddFiles, "m_grpAddFiles");
            m_grpAddFiles.Controls.Add(m_btnRemove);
            m_grpAddFiles.Controls.Add(m_grpArrangeAudioFiles);
            m_grpAddFiles.Controls.Add(lstManualArrange);
            m_grpAddFiles.Controls.Add(m_btnAdd);
            m_grpAddFiles.Controls.Add(m_btnMoveUp);
            m_grpAddFiles.Controls.Add(m_btnMoveDown);
            m_grpAddFiles.Name = "m_grpAddFiles";
            m_grpAddFiles.TabStop = false;
            // 
            // m_btnRemove
            // 
            resources.ApplyResources(m_btnRemove, "m_btnRemove");
            m_btnRemove.Name = "m_btnRemove";
            m_btnRemove.UseVisualStyleBackColor = true;
            m_btnRemove.Click += m_btnRemove_Click;
            // 
            // m_grpArrangeAudioFiles
            // 
            resources.ApplyResources(m_grpArrangeAudioFiles, "m_grpArrangeAudioFiles");
            m_grpArrangeAudioFiles.Controls.Add(mbtnDesendingOrder);
            m_grpArrangeAudioFiles.Controls.Add(mbtnAscendingOrder);
            m_grpArrangeAudioFiles.Name = "m_grpArrangeAudioFiles";
            m_grpArrangeAudioFiles.TabStop = false;
            // 
            // mbtnDesendingOrder
            // 
            resources.ApplyResources(mbtnDesendingOrder, "mbtnDesendingOrder");
            mbtnDesendingOrder.Name = "mbtnDesendingOrder";
            mbtnDesendingOrder.UseVisualStyleBackColor = true;
            mbtnDesendingOrder.Click += mbtnDesendingOrder_Click;
            // 
            // mbtnAscendingOrder
            // 
            resources.ApplyResources(mbtnAscendingOrder, "mbtnAscendingOrder");
            mbtnAscendingOrder.Name = "mbtnAscendingOrder";
            helpProvider1.SetShowHelp(mbtnAscendingOrder, (bool)resources.GetObject("mbtnAscendingOrder.ShowHelp"));
            mbtnAscendingOrder.UseVisualStyleBackColor = true;
            mbtnAscendingOrder.Click += mbtnAscendingOrder_Click;
            // 
            // m_txtCharToReplaceWithSpace
            // 
            resources.ApplyResources(m_txtCharToReplaceWithSpace, "m_txtCharToReplaceWithSpace");
            m_txtCharToReplaceWithSpace.Name = "m_txtCharToReplaceWithSpace";
            // 
            // m_txtPageIdentificationString
            // 
            resources.ApplyResources(m_txtPageIdentificationString, "m_txtPageIdentificationString");
            m_txtPageIdentificationString.Name = "m_txtPageIdentificationString";
            // 
            // m_numCharCountToTruncateFromStart
            // 
            resources.ApplyResources(m_numCharCountToTruncateFromStart, "m_numCharCountToTruncateFromStart");
            m_numCharCountToTruncateFromStart.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            m_numCharCountToTruncateFromStart.Name = "m_numCharCountToTruncateFromStart";
            // 
            // m_grpCreateSectionForEachAudioFile
            // 
            resources.ApplyResources(m_grpCreateSectionForEachAudioFile, "m_grpCreateSectionForEachAudioFile");
            m_grpCreateSectionForEachAudioFile.Controls.Add(m_rdbSplitAtCuePoints);
            m_grpCreateSectionForEachAudioFile.Controls.Add(m_rdbCreateAudioFilePerSection);
            m_grpCreateSectionForEachAudioFile.Controls.Add(m_rdbImportAudioFileInEachSection);
            m_grpCreateSectionForEachAudioFile.Controls.Add(m_rdbImportAudioInSelectedSection);
            m_grpCreateSectionForEachAudioFile.Controls.Add(mchktPageIdentificationString);
            m_grpCreateSectionForEachAudioFile.Controls.Add(mchkCountToTruncateFromStart);
            m_grpCreateSectionForEachAudioFile.Controls.Add(mchkToReplaceWithSpace);
            m_grpCreateSectionForEachAudioFile.Controls.Add(m_txtCharToReplaceWithSpace);
            m_grpCreateSectionForEachAudioFile.Controls.Add(m_numCharCountToTruncateFromStart);
            m_grpCreateSectionForEachAudioFile.Controls.Add(m_txtPageIdentificationString);
            m_grpCreateSectionForEachAudioFile.Name = "m_grpCreateSectionForEachAudioFile";
            m_grpCreateSectionForEachAudioFile.TabStop = false;
            // 
            // m_rdbSplitAtCuePoints
            // 
            resources.ApplyResources(m_rdbSplitAtCuePoints, "m_rdbSplitAtCuePoints");
            m_rdbSplitAtCuePoints.Name = "m_rdbSplitAtCuePoints";
            helpProvider1.SetShowHelp(m_rdbSplitAtCuePoints, (bool)resources.GetObject("m_rdbSplitAtCuePoints.ShowHelp"));
            m_rdbSplitAtCuePoints.TabStop = true;
            m_rdbSplitAtCuePoints.UseVisualStyleBackColor = true;
            m_rdbSplitAtCuePoints.CheckedChanged += m_rdbSplitAtCuePoints_CheckedChanged;
            // 
            // m_rdbCreateAudioFilePerSection
            // 
            resources.ApplyResources(m_rdbCreateAudioFilePerSection, "m_rdbCreateAudioFilePerSection");
            m_rdbCreateAudioFilePerSection.Name = "m_rdbCreateAudioFilePerSection";
            m_rdbCreateAudioFilePerSection.TabStop = true;
            m_rdbCreateAudioFilePerSection.UseVisualStyleBackColor = true;
            m_rdbCreateAudioFilePerSection.CheckedChanged += m_rdbCreateAudioFilePerSection_CheckedChanged;
            // 
            // m_rdbImportAudioFileInEachSection
            // 
            resources.ApplyResources(m_rdbImportAudioFileInEachSection, "m_rdbImportAudioFileInEachSection");
            m_rdbImportAudioFileInEachSection.Name = "m_rdbImportAudioFileInEachSection";
            m_rdbImportAudioFileInEachSection.TabStop = true;
            m_rdbImportAudioFileInEachSection.UseVisualStyleBackColor = true;
            m_rdbImportAudioFileInEachSection.CheckedChanged += m_rdbImportAudioFileInEachSection_CheckedChanged;
            // 
            // m_rdbImportAudioInSelectedSection
            // 
            resources.ApplyResources(m_rdbImportAudioInSelectedSection, "m_rdbImportAudioInSelectedSection");
            m_rdbImportAudioInSelectedSection.Name = "m_rdbImportAudioInSelectedSection";
            helpProvider1.SetShowHelp(m_rdbImportAudioInSelectedSection, (bool)resources.GetObject("m_rdbImportAudioInSelectedSection.ShowHelp"));
            m_rdbImportAudioInSelectedSection.TabStop = true;
            m_rdbImportAudioInSelectedSection.UseVisualStyleBackColor = true;
            // 
            // mchktPageIdentificationString
            // 
            resources.ApplyResources(mchktPageIdentificationString, "mchktPageIdentificationString");
            mchktPageIdentificationString.Name = "mchktPageIdentificationString";
            mchktPageIdentificationString.UseVisualStyleBackColor = true;
            mchktPageIdentificationString.CheckedChanged += mchktPageIdentificationString_CheckedChanged;
            // 
            // mchkCountToTruncateFromStart
            // 
            resources.ApplyResources(mchkCountToTruncateFromStart, "mchkCountToTruncateFromStart");
            mchkCountToTruncateFromStart.Name = "mchkCountToTruncateFromStart";
            mchkCountToTruncateFromStart.UseVisualStyleBackColor = true;
            mchkCountToTruncateFromStart.CheckedChanged += mchkCountToTruncateFromStart_CheckedChanged;
            // 
            // mchkToReplaceWithSpace
            // 
            resources.ApplyResources(mchkToReplaceWithSpace, "mchkToReplaceWithSpace");
            mchkToReplaceWithSpace.Name = "mchkToReplaceWithSpace";
            mchkToReplaceWithSpace.UseVisualStyleBackColor = true;
            mchkToReplaceWithSpace.CheckedChanged += mchkToReplaceWithSpace_CheckedChanged;
            // 
            // m_grpSplitPhraseOrPhraseDetection
            // 
            m_grpSplitPhraseOrPhraseDetection.Controls.Add(m_rdbPhraseDetectionWhisper);
            m_grpSplitPhraseOrPhraseDetection.Controls.Add(m_rdbPhraseDetectionOnImportedFiles);
            m_grpSplitPhraseOrPhraseDetection.Controls.Add(m_rdbSplitPhrasesOnImport);
            m_grpSplitPhraseOrPhraseDetection.Controls.Add(label1);
            m_grpSplitPhraseOrPhraseDetection.Controls.Add(mPhraseSizeTextBox);
            resources.ApplyResources(m_grpSplitPhraseOrPhraseDetection, "m_grpSplitPhraseOrPhraseDetection");
            m_grpSplitPhraseOrPhraseDetection.Name = "m_grpSplitPhraseOrPhraseDetection";
            helpProvider1.SetShowHelp(m_grpSplitPhraseOrPhraseDetection, (bool)resources.GetObject("m_grpSplitPhraseOrPhraseDetection.ShowHelp"));
            m_grpSplitPhraseOrPhraseDetection.TabStop = false;
            // 
            // m_rdbPhraseDetectionWhisper
            // 
            resources.ApplyResources(m_rdbPhraseDetectionWhisper, "m_rdbPhraseDetectionWhisper");
            m_rdbPhraseDetectionWhisper.Name = "m_rdbPhraseDetectionWhisper";
            m_rdbPhraseDetectionWhisper.TabStop = true;
            m_rdbPhraseDetectionWhisper.UseVisualStyleBackColor = true;
            // 
            // m_rdbPhraseDetectionOnImportedFiles
            // 
            resources.ApplyResources(m_rdbPhraseDetectionOnImportedFiles, "m_rdbPhraseDetectionOnImportedFiles");
            m_rdbPhraseDetectionOnImportedFiles.Checked = true;
            m_rdbPhraseDetectionOnImportedFiles.Name = "m_rdbPhraseDetectionOnImportedFiles";
            m_rdbPhraseDetectionOnImportedFiles.TabStop = true;
            m_rdbPhraseDetectionOnImportedFiles.UseVisualStyleBackColor = true;
            // 
            // m_rdbSplitPhrasesOnImport
            // 
            resources.ApplyResources(m_rdbSplitPhrasesOnImport, "m_rdbSplitPhrasesOnImport");
            m_rdbSplitPhrasesOnImport.Name = "m_rdbSplitPhrasesOnImport";
            m_rdbSplitPhrasesOnImport.UseVisualStyleBackColor = true;
            m_rdbSplitPhrasesOnImport.CheckedChanged += m_rdbSplitPhrasesOnImport_CheckedChanged;
            // 
            // helpProvider1
            // 
            resources.ApplyResources(helpProvider1, "helpProvider1");
            // 
            // ImportFileSplitSize
            // 
            AcceptButton = mOKButton;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            CancelButton = mCancelButton;
            Controls.Add(m_grpSplitPhraseOrPhraseDetection);
            Controls.Add(m_grpCreateSectionForEachAudioFile);
            Controls.Add(m_grpAddFiles);
            Controls.Add(mCancelButton);
            Controls.Add(mOKButton);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            KeyPreview = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ImportFileSplitSize";
            helpProvider1.SetShowHelp(this, (bool)resources.GetObject("$this.ShowHelp"));
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            FormClosing += ImportFileSplitSize_FormClosing;
            FormClosed += ImportFileSplitSize_FormClosed;
            m_grpAddFiles.ResumeLayout(false);
            m_grpArrangeAudioFiles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)m_numCharCountToTruncateFromStart).EndInit();
            m_grpCreateSectionForEachAudioFile.ResumeLayout(false);
            m_grpCreateSectionForEachAudioFile.PerformLayout();
            m_grpSplitPhraseOrPhraseDetection.ResumeLayout(false);
            m_grpSplitPhraseOrPhraseDetection.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mPhraseSizeTextBox;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
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
        private System.Windows.Forms.RadioButton m_rdbImportAudioInSelectedSection;
        private System.Windows.Forms.RadioButton m_rdbImportAudioFileInEachSection;
        private System.Windows.Forms.RadioButton m_rdbCreateAudioFilePerSection;
        private System.Windows.Forms.RadioButton m_rdbSplitAtCuePoints;
        private System.Windows.Forms.RadioButton m_rdbPhraseDetectionWhisper;
    }
}