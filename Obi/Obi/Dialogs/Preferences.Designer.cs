namespace Obi.Dialogs
{
    partial class Preferences
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preferences));
            this.label2 = new System.Windows.Forms.Label();
            this.mDirectoryTextbox = new System.Windows.Forms.TextBox();
            this.mBrowseButton = new System.Windows.Forms.Button();
            this.mOKButton = new System.Windows.Forms.Button();
            this.mCancelButton = new System.Windows.Forms.Button();
            this.labelInputDeviceName = new System.Windows.Forms.Label();
            this.mInputDeviceCombo = new System.Windows.Forms.ComboBox();
            this.labelOutputDeviceName = new System.Windows.Forms.Label();
            this.mOutputDeviceCombo = new System.Windows.Forms.ComboBox();
            this.mTab = new System.Windows.Forms.TabControl();
            this.mProjectTab = new System.Windows.Forms.TabPage();
            this.m_ChkAutoSaveInterval = new System.Windows.Forms.CheckBox();
            this.mChkAutoSaveOnRecordingEnd = new System.Windows.Forms.CheckBox();
            this.MnumAutoSaveInterval = new System.Windows.Forms.NumericUpDown();
            this.mPipelineTextbox = new System.Windows.Forms.TextBox();
            this.mPipelineBrowseButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.mLastOpenCheckBox = new System.Windows.Forms.CheckBox();
            this.mAudioTab = new System.Windows.Forms.TabPage();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.mNudgeDurationUpDown = new System.Windows.Forms.NumericUpDown();
            this.mNoiseLevelComboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.mAudioCluesCheckBox = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.mPreviewDurationUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.mChannelsTextbox = new System.Windows.Forms.TextBox();
            this.mSampleRateTextbox = new System.Windows.Forms.TextBox();
            this.mChannelsCombo = new System.Windows.Forms.ComboBox();
            this.labelChannels = new System.Windows.Forms.Label();
            this.mSampleRateCombo = new System.Windows.Forms.ComboBox();
            this.labelSampleRate = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.mCultureBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.mOrganizationTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.mFullNameTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.m_btnRemove = new System.Windows.Forms.Button();
            this.m_btnAssign = new System.Windows.Forms.Button();
            this.m_txtShortcutKeys = new System.Windows.Forms.TextBox();
            this.m_lblShortcutKeys = new System.Windows.Forms.Label();
            this.m_lvShortcutKeysList = new System.Windows.Forms.ListView();
            this.m_cbShortcutKeys = new System.Windows.Forms.ComboBox();
            this.mTab.SuspendLayout();
            this.mProjectTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MnumAutoSaveInterval)).BeginInit();
            this.mAudioTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mNudgeDurationUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewDurationUpDown)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // mDirectoryTextbox
            // 
            resources.ApplyResources(this.mDirectoryTextbox, "mDirectoryTextbox");
            this.mDirectoryTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mDirectoryTextbox.Name = "mDirectoryTextbox";
            // 
            // mBrowseButton
            // 
            resources.ApplyResources(this.mBrowseButton, "mBrowseButton");
            this.mBrowseButton.Name = "mBrowseButton";
            this.mBrowseButton.UseVisualStyleBackColor = true;
            this.mBrowseButton.Click += new System.EventHandler(this.mBrowseButton_Click);
            // 
            // mOKButton
            // 
            resources.ApplyResources(this.mOKButton, "mOKButton");
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
            // labelInputDeviceName
            // 
            resources.ApplyResources(this.labelInputDeviceName, "labelInputDeviceName");
            this.labelInputDeviceName.Name = "labelInputDeviceName";
            // 
            // mInputDeviceCombo
            // 
            resources.ApplyResources(this.mInputDeviceCombo, "mInputDeviceCombo");
            this.mInputDeviceCombo.AllowDrop = true;
            this.mInputDeviceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mInputDeviceCombo.FormattingEnabled = true;
            this.mInputDeviceCombo.Name = "mInputDeviceCombo";
            // 
            // labelOutputDeviceName
            // 
            resources.ApplyResources(this.labelOutputDeviceName, "labelOutputDeviceName");
            this.labelOutputDeviceName.Name = "labelOutputDeviceName";
            // 
            // mOutputDeviceCombo
            // 
            resources.ApplyResources(this.mOutputDeviceCombo, "mOutputDeviceCombo");
            this.mOutputDeviceCombo.AllowDrop = true;
            this.mOutputDeviceCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mOutputDeviceCombo.FormattingEnabled = true;
            this.mOutputDeviceCombo.Name = "mOutputDeviceCombo";
            // 
            // mTab
            // 
            resources.ApplyResources(this.mTab, "mTab");
            this.mTab.Controls.Add(this.mProjectTab);
            this.mTab.Controls.Add(this.mAudioTab);
            this.mTab.Controls.Add(this.tabPage1);
            this.mTab.Controls.Add(this.tabPage2);
            this.mTab.Name = "mTab";
            this.mTab.SelectedIndex = 0;
            // 
            // mProjectTab
            // 
            this.mProjectTab.Controls.Add(this.m_ChkAutoSaveInterval);
            this.mProjectTab.Controls.Add(this.mChkAutoSaveOnRecordingEnd);
            this.mProjectTab.Controls.Add(this.MnumAutoSaveInterval);
            this.mProjectTab.Controls.Add(this.mPipelineTextbox);
            this.mProjectTab.Controls.Add(this.mPipelineBrowseButton);
            this.mProjectTab.Controls.Add(this.label9);
            this.mProjectTab.Controls.Add(this.mLastOpenCheckBox);
            this.mProjectTab.Controls.Add(this.label2);
            this.mProjectTab.Controls.Add(this.mBrowseButton);
            this.mProjectTab.Controls.Add(this.mDirectoryTextbox);
            resources.ApplyResources(this.mProjectTab, "mProjectTab");
            this.mProjectTab.Name = "mProjectTab";
            this.mProjectTab.UseVisualStyleBackColor = true;
            // 
            // m_ChkAutoSaveInterval
            // 
            resources.ApplyResources(this.m_ChkAutoSaveInterval, "m_ChkAutoSaveInterval");
            this.m_ChkAutoSaveInterval.Name = "m_ChkAutoSaveInterval";
            this.m_ChkAutoSaveInterval.UseVisualStyleBackColor = true;
            this.m_ChkAutoSaveInterval.CheckStateChanged += new System.EventHandler(this.m_ChkAutoSaveInterval_CheckStateChanged);
            // 
            // mChkAutoSaveOnRecordingEnd
            // 
            resources.ApplyResources(this.mChkAutoSaveOnRecordingEnd, "mChkAutoSaveOnRecordingEnd");
            this.mChkAutoSaveOnRecordingEnd.Name = "mChkAutoSaveOnRecordingEnd";
            this.mChkAutoSaveOnRecordingEnd.UseVisualStyleBackColor = true;
            // 
            // MnumAutoSaveInterval
            // 
            resources.ApplyResources(this.MnumAutoSaveInterval, "MnumAutoSaveInterval");
            this.MnumAutoSaveInterval.Maximum = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.MnumAutoSaveInterval.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.MnumAutoSaveInterval.Name = "MnumAutoSaveInterval";
            this.MnumAutoSaveInterval.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // mPipelineTextbox
            // 
            resources.ApplyResources(this.mPipelineTextbox, "mPipelineTextbox");
            this.mPipelineTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPipelineTextbox.Name = "mPipelineTextbox";
            // 
            // mPipelineBrowseButton
            // 
            resources.ApplyResources(this.mPipelineBrowseButton, "mPipelineBrowseButton");
            this.mPipelineBrowseButton.Name = "mPipelineBrowseButton";
            this.mPipelineBrowseButton.UseVisualStyleBackColor = true;
            this.mPipelineBrowseButton.Click += new System.EventHandler(this.mPipelineBrowseButton_Click);
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // mLastOpenCheckBox
            // 
            resources.ApplyResources(this.mLastOpenCheckBox, "mLastOpenCheckBox");
            this.mLastOpenCheckBox.Name = "mLastOpenCheckBox";
            this.mLastOpenCheckBox.UseVisualStyleBackColor = true;
            // 
            // mAudioTab
            // 
            this.mAudioTab.Controls.Add(this.label11);
            this.mAudioTab.Controls.Add(this.label10);
            this.mAudioTab.Controls.Add(this.mNudgeDurationUpDown);
            this.mAudioTab.Controls.Add(this.mNoiseLevelComboBox);
            this.mAudioTab.Controls.Add(this.label8);
            this.mAudioTab.Controls.Add(this.mAudioCluesCheckBox);
            this.mAudioTab.Controls.Add(this.label7);
            this.mAudioTab.Controls.Add(this.mPreviewDurationUpDown);
            this.mAudioTab.Controls.Add(this.label3);
            this.mAudioTab.Controls.Add(this.mChannelsTextbox);
            this.mAudioTab.Controls.Add(this.mSampleRateTextbox);
            this.mAudioTab.Controls.Add(this.mChannelsCombo);
            this.mAudioTab.Controls.Add(this.labelChannels);
            this.mAudioTab.Controls.Add(this.mSampleRateCombo);
            this.mAudioTab.Controls.Add(this.labelSampleRate);
            this.mAudioTab.Controls.Add(this.mOutputDeviceCombo);
            this.mAudioTab.Controls.Add(this.mInputDeviceCombo);
            this.mAudioTab.Controls.Add(this.labelOutputDeviceName);
            this.mAudioTab.Controls.Add(this.labelInputDeviceName);
            resources.ApplyResources(this.mAudioTab, "mAudioTab");
            this.mAudioTab.Name = "mAudioTab";
            this.mAudioTab.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // mNudgeDurationUpDown
            // 
            resources.ApplyResources(this.mNudgeDurationUpDown, "mNudgeDurationUpDown");
            this.mNudgeDurationUpDown.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.mNudgeDurationUpDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.mNudgeDurationUpDown.Name = "mNudgeDurationUpDown";
            this.mNudgeDurationUpDown.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            // 
            // mNoiseLevelComboBox
            // 
            this.mNoiseLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.mNoiseLevelComboBox, "mNoiseLevelComboBox");
            this.mNoiseLevelComboBox.FormattingEnabled = true;
            this.mNoiseLevelComboBox.Items.AddRange(new object[] {
            resources.GetString("mNoiseLevelComboBox.Items"),
            resources.GetString("mNoiseLevelComboBox.Items1"),
            resources.GetString("mNoiseLevelComboBox.Items2")});
            this.mNoiseLevelComboBox.Name = "mNoiseLevelComboBox";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // mAudioCluesCheckBox
            // 
            resources.ApplyResources(this.mAudioCluesCheckBox, "mAudioCluesCheckBox");
            this.mAudioCluesCheckBox.Name = "mAudioCluesCheckBox";
            this.mAudioCluesCheckBox.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // mPreviewDurationUpDown
            // 
            this.mPreviewDurationUpDown.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mPreviewDurationUpDown.DecimalPlaces = 1;
            this.mPreviewDurationUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.mPreviewDurationUpDown, "mPreviewDurationUpDown");
            this.mPreviewDurationUpDown.Name = "mPreviewDurationUpDown";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // mChannelsTextbox
            // 
            resources.ApplyResources(this.mChannelsTextbox, "mChannelsTextbox");
            this.mChannelsTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mChannelsTextbox.Name = "mChannelsTextbox";
            this.mChannelsTextbox.ReadOnly = true;
            // 
            // mSampleRateTextbox
            // 
            resources.ApplyResources(this.mSampleRateTextbox, "mSampleRateTextbox");
            this.mSampleRateTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mSampleRateTextbox.Name = "mSampleRateTextbox";
            this.mSampleRateTextbox.ReadOnly = true;
            // 
            // mChannelsCombo
            // 
            resources.ApplyResources(this.mChannelsCombo, "mChannelsCombo");
            this.mChannelsCombo.AllowDrop = true;
            this.mChannelsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mChannelsCombo.FormattingEnabled = true;
            this.mChannelsCombo.Name = "mChannelsCombo";
            // 
            // labelChannels
            // 
            resources.ApplyResources(this.labelChannels, "labelChannels");
            this.labelChannels.Name = "labelChannels";
            // 
            // mSampleRateCombo
            // 
            resources.ApplyResources(this.mSampleRateCombo, "mSampleRateCombo");
            this.mSampleRateCombo.AllowDrop = true;
            this.mSampleRateCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mSampleRateCombo.FormattingEnabled = true;
            this.mSampleRateCombo.Name = "mSampleRateCombo";
            // 
            // labelSampleRate
            // 
            resources.ApplyResources(this.labelSampleRate, "labelSampleRate");
            this.labelSampleRate.Name = "labelSampleRate";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.mCultureBox);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.mOrganizationTextbox);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.mFullNameTextbox);
            this.tabPage1.Controls.Add(this.label1);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // mCultureBox
            // 
            resources.ApplyResources(this.mCultureBox, "mCultureBox");
            this.mCultureBox.AllowDrop = true;
            this.mCultureBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mCultureBox.FormattingEnabled = true;
            this.mCultureBox.Name = "mCultureBox";
            this.mCultureBox.Sorted = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // mOrganizationTextbox
            // 
            resources.ApplyResources(this.mOrganizationTextbox, "mOrganizationTextbox");
            this.mOrganizationTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mOrganizationTextbox.Name = "mOrganizationTextbox";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // mFullNameTextbox
            // 
            resources.ApplyResources(this.mFullNameTextbox, "mFullNameTextbox");
            this.mFullNameTextbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mFullNameTextbox.Name = "mFullNameTextbox";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.m_btnRemove);
            this.tabPage2.Controls.Add(this.m_btnAssign);
            this.tabPage2.Controls.Add(this.m_txtShortcutKeys);
            this.tabPage2.Controls.Add(this.m_lblShortcutKeys);
            this.tabPage2.Controls.Add(this.m_lvShortcutKeysList);
            this.tabPage2.Controls.Add(this.m_cbShortcutKeys);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // m_btnRemove
            // 
            resources.ApplyResources(this.m_btnRemove, "m_btnRemove");
            this.m_btnRemove.Name = "m_btnRemove";
            this.m_btnRemove.UseVisualStyleBackColor = true;
            // 
            // m_btnAssign
            // 
            resources.ApplyResources(this.m_btnAssign, "m_btnAssign");
            this.m_btnAssign.Name = "m_btnAssign";
            this.m_btnAssign.UseVisualStyleBackColor = true;
            // 
            // m_txtShortcutKeys
            // 
            resources.ApplyResources(this.m_txtShortcutKeys, "m_txtShortcutKeys");
            this.m_txtShortcutKeys.Name = "m_txtShortcutKeys";
            // 
            // m_lblShortcutKeys
            // 
            resources.ApplyResources(this.m_lblShortcutKeys, "m_lblShortcutKeys");
            this.m_lblShortcutKeys.Name = "m_lblShortcutKeys";
            // 
            // m_lvShortcutKeysList
            // 
            resources.ApplyResources(this.m_lvShortcutKeysList, "m_lvShortcutKeysList");
            this.m_lvShortcutKeysList.Name = "m_lvShortcutKeysList";
            this.m_lvShortcutKeysList.UseCompatibleStateImageBehavior = false;
            // 
            // m_cbShortcutKeys
            // 
            resources.ApplyResources(this.m_cbShortcutKeys, "m_cbShortcutKeys");
            this.m_cbShortcutKeys.FormattingEnabled = true;
            this.m_cbShortcutKeys.Items.AddRange(new object[] {
            resources.GetString("m_cbShortcutKeys.Items"),
            resources.GetString("m_cbShortcutKeys.Items1"),
            resources.GetString("m_cbShortcutKeys.Items2")});
            this.m_cbShortcutKeys.Name = "m_cbShortcutKeys";
            // 
            // Preferences
            // 
            this.AcceptButton = this.mOKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.Controls.Add(this.mTab);
            this.Controls.Add(this.mCancelButton);
            this.Controls.Add(this.mOKButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Preferences";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.mTab.ResumeLayout(false);
            this.mProjectTab.ResumeLayout(false);
            this.mProjectTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MnumAutoSaveInterval)).EndInit();
            this.mAudioTab.ResumeLayout(false);
            this.mAudioTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mNudgeDurationUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mPreviewDurationUpDown)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox mDirectoryTextbox;
        private System.Windows.Forms.Button mBrowseButton;
        private System.Windows.Forms.Button mOKButton;
        private System.Windows.Forms.Button mCancelButton;
        private System.Windows.Forms.Label labelInputDeviceName;
        private System.Windows.Forms.ComboBox mInputDeviceCombo;
        private System.Windows.Forms.Label labelOutputDeviceName;
        private System.Windows.Forms.ComboBox mOutputDeviceCombo;
        private System.Windows.Forms.TabControl mTab;
        private System.Windows.Forms.TabPage mProjectTab;
        private System.Windows.Forms.TabPage mAudioTab;
        private System.Windows.Forms.Label labelSampleRate;
        private System.Windows.Forms.ComboBox mSampleRateCombo;
        private System.Windows.Forms.ComboBox mChannelsCombo;
        private System.Windows.Forms.Label labelChannels;
        private System.Windows.Forms.CheckBox mLastOpenCheckBox;
        private System.Windows.Forms.TextBox mChannelsTextbox;
        private System.Windows.Forms.TextBox mSampleRateTextbox;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mFullNameTextbox;
        private System.Windows.Forms.TextBox mOrganizationTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox mCultureBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox mAudioCluesCheckBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown mPreviewDurationUpDown;
        private System.Windows.Forms.ComboBox mNoiseLevelComboBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button mPipelineBrowseButton;
        private System.Windows.Forms.TextBox mPipelineTextbox;
        private System.Windows.Forms.NumericUpDown MnumAutoSaveInterval;
        private System.Windows.Forms.CheckBox mChkAutoSaveOnRecordingEnd;
        private System.Windows.Forms.CheckBox m_ChkAutoSaveInterval;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown mNudgeDurationUpDown;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ComboBox m_cbShortcutKeys;
        private System.Windows.Forms.ListView m_lvShortcutKeysList;
        private System.Windows.Forms.Button m_btnRemove;
        private System.Windows.Forms.Button m_btnAssign;
        private System.Windows.Forms.TextBox m_txtShortcutKeys;
        private System.Windows.Forms.Label m_lblShortcutKeys;
    }
}