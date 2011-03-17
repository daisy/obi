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
            this.m_OperationDurationUpDown = new System.Windows.Forms.NumericUpDown();
            this.m_cbOperation = new System.Windows.Forms.ComboBox();
            this.mlbOperation = new System.Windows.Forms.Label();
            this.mNoiseLevelComboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.mAudioCluesCheckBox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.mChannelsTextbox = new System.Windows.Forms.TextBox();
            this.mSampleRateTextbox = new System.Windows.Forms.TextBox();
            this.mChannelsCombo = new System.Windows.Forms.ComboBox();
            this.labelChannels = new System.Windows.Forms.Label();
            this.mSampleRateCombo = new System.Windows.Forms.ComboBox();
            this.labelSampleRate = new System.Windows.Forms.Label();
            this.mUserProfileTab = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.mCultureBox = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.mOrganizationTextbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.mFullNameTextbox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.mKeyboardShortcutTab = new System.Windows.Forms.TabPage();
            this.label12 = new System.Windows.Forms.Label();
            this.m_grpKeyboardShortcutList = new System.Windows.Forms.GroupBox();
            this.m_lvShortcutKeysList = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.m_RestoreDefaults = new System.Windows.Forms.Button();
            this.m_btnRemove = new System.Windows.Forms.Button();
            this.m_btnAssign = new System.Windows.Forms.Button();
            this.m_txtShortcutKeys = new System.Windows.Forms.TextBox();
            this.m_lblShortcutKeys = new System.Windows.Forms.Label();
            this.m_cbShortcutKeys = new System.Windows.Forms.ComboBox();
            this.m_CheckBoxListView = new System.Windows.Forms.ListView();
            this.mTab.SuspendLayout();
            this.mProjectTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MnumAutoSaveInterval)).BeginInit();
            this.mAudioTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_OperationDurationUpDown)).BeginInit();
            this.mUserProfileTab.SuspendLayout();
            this.mKeyboardShortcutTab.SuspendLayout();
            this.m_grpKeyboardShortcutList.SuspendLayout();
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
            this.mTab.Controls.Add(this.mUserProfileTab);
            this.mTab.Controls.Add(this.mKeyboardShortcutTab);
            this.mTab.Name = "mTab";
            this.mTab.SelectedIndex = 0;
            this.mTab.SelectedIndexChanged += new System.EventHandler(this.mTab_SelectedIndexChanged);
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
            this.mAudioTab.Controls.Add(this.m_OperationDurationUpDown);
            this.mAudioTab.Controls.Add(this.m_cbOperation);
            this.mAudioTab.Controls.Add(this.mlbOperation);
            this.mAudioTab.Controls.Add(this.mNoiseLevelComboBox);
            this.mAudioTab.Controls.Add(this.label8);
            this.mAudioTab.Controls.Add(this.mAudioCluesCheckBox);
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
            // m_OperationDurationUpDown
            // 
            this.m_OperationDurationUpDown.Increment = new decimal(new int[] {
            50,
            0,
            0,
            0});
            resources.ApplyResources(this.m_OperationDurationUpDown, "m_OperationDurationUpDown");
            this.m_OperationDurationUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.m_OperationDurationUpDown.Name = "m_OperationDurationUpDown";
            this.m_OperationDurationUpDown.ValueChanged += new System.EventHandler(this.m_OperationDurationUpDown_ValueChanged);
            // 
            // m_cbOperation
            // 
            this.m_cbOperation.FormattingEnabled = true;
            this.m_cbOperation.Items.AddRange(new object[] {
            resources.GetString("m_cbOperation.Items"),
            resources.GetString("m_cbOperation.Items1"),
            resources.GetString("m_cbOperation.Items2")});
            resources.ApplyResources(this.m_cbOperation, "m_cbOperation");
            this.m_cbOperation.Name = "m_cbOperation";
            this.m_cbOperation.SelectedIndexChanged += new System.EventHandler(this.m_cbOperation_SelectedIndexChanged);
            // 
            // mlbOperation
            // 
            resources.ApplyResources(this.mlbOperation, "mlbOperation");
            this.mlbOperation.Name = "mlbOperation";
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
            // mUserProfileTab
            // 
            this.mUserProfileTab.Controls.Add(this.label6);
            this.mUserProfileTab.Controls.Add(this.mCultureBox);
            this.mUserProfileTab.Controls.Add(this.label5);
            this.mUserProfileTab.Controls.Add(this.mOrganizationTextbox);
            this.mUserProfileTab.Controls.Add(this.label4);
            this.mUserProfileTab.Controls.Add(this.mFullNameTextbox);
            this.mUserProfileTab.Controls.Add(this.label1);
            resources.ApplyResources(this.mUserProfileTab, "mUserProfileTab");
            this.mUserProfileTab.Name = "mUserProfileTab";
            this.mUserProfileTab.UseVisualStyleBackColor = true;
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
            // mKeyboardShortcutTab
            // 
            this.mKeyboardShortcutTab.Controls.Add(this.label12);
            this.mKeyboardShortcutTab.Controls.Add(this.m_grpKeyboardShortcutList);
            this.mKeyboardShortcutTab.Controls.Add(this.m_RestoreDefaults);
            this.mKeyboardShortcutTab.Controls.Add(this.m_btnRemove);
            this.mKeyboardShortcutTab.Controls.Add(this.m_btnAssign);
            this.mKeyboardShortcutTab.Controls.Add(this.m_txtShortcutKeys);
            this.mKeyboardShortcutTab.Controls.Add(this.m_lblShortcutKeys);
            this.mKeyboardShortcutTab.Controls.Add(this.m_cbShortcutKeys);
            resources.ApplyResources(this.mKeyboardShortcutTab, "mKeyboardShortcutTab");
            this.mKeyboardShortcutTab.Name = "mKeyboardShortcutTab";
            this.mKeyboardShortcutTab.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // m_grpKeyboardShortcutList
            // 
            this.m_grpKeyboardShortcutList.Controls.Add(this.m_lvShortcutKeysList);
            resources.ApplyResources(this.m_grpKeyboardShortcutList, "m_grpKeyboardShortcutList");
            this.m_grpKeyboardShortcutList.Name = "m_grpKeyboardShortcutList";
            this.m_grpKeyboardShortcutList.TabStop = false;
            // 
            // m_lvShortcutKeysList
            // 
            this.m_lvShortcutKeysList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.m_lvShortcutKeysList.FullRowSelect = true;
            resources.ApplyResources(this.m_lvShortcutKeysList, "m_lvShortcutKeysList");
            this.m_lvShortcutKeysList.MultiSelect = false;
            this.m_lvShortcutKeysList.Name = "m_lvShortcutKeysList";
            this.m_lvShortcutKeysList.UseCompatibleStateImageBehavior = false;
            this.m_lvShortcutKeysList.View = System.Windows.Forms.View.Details;
            this.m_lvShortcutKeysList.SelectedIndexChanged += new System.EventHandler(this.m_lvShortcutKeysList_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // m_RestoreDefaults
            // 
            resources.ApplyResources(this.m_RestoreDefaults, "m_RestoreDefaults");
            this.m_RestoreDefaults.Name = "m_RestoreDefaults";
            this.m_RestoreDefaults.UseVisualStyleBackColor = true;
            this.m_RestoreDefaults.Click += new System.EventHandler(this.m_RestoreDefaults_Click);
            // 
            // m_btnRemove
            // 
            resources.ApplyResources(this.m_btnRemove, "m_btnRemove");
            this.m_btnRemove.Name = "m_btnRemove";
            this.m_btnRemove.UseVisualStyleBackColor = true;
            this.m_btnRemove.Click += new System.EventHandler(this.m_btnRemove_Click);
            // 
            // m_btnAssign
            // 
            resources.ApplyResources(this.m_btnAssign, "m_btnAssign");
            this.m_btnAssign.Name = "m_btnAssign";
            this.m_btnAssign.UseVisualStyleBackColor = true;
            this.m_btnAssign.Click += new System.EventHandler(this.m_btnAssign_Click);
            // 
            // m_txtShortcutKeys
            // 
            resources.ApplyResources(this.m_txtShortcutKeys, "m_txtShortcutKeys");
            this.m_txtShortcutKeys.Name = "m_txtShortcutKeys";
            this.m_txtShortcutKeys.KeyDown += new System.Windows.Forms.KeyEventHandler(this.m_txtShortcutKeys_KeyDown);
            this.m_txtShortcutKeys.KeyUp += new System.Windows.Forms.KeyEventHandler(this.m_txtShortcutKeys_KeyUp);
            this.m_txtShortcutKeys.Enter += new System.EventHandler(this.m_txtShortcutKeys_Enter);
            // 
            // m_lblShortcutKeys
            // 
            resources.ApplyResources(this.m_lblShortcutKeys, "m_lblShortcutKeys");
            this.m_lblShortcutKeys.Name = "m_lblShortcutKeys";
            // 
            // m_cbShortcutKeys
            // 
            resources.ApplyResources(this.m_cbShortcutKeys, "m_cbShortcutKeys");
            this.m_cbShortcutKeys.FormattingEnabled = true;
            this.m_cbShortcutKeys.Items.AddRange(new object[] {
            resources.GetString("m_cbShortcutKeys.Items"),
            resources.GetString("m_cbShortcutKeys.Items1")});
            this.m_cbShortcutKeys.Name = "m_cbShortcutKeys";
            this.m_cbShortcutKeys.SelectionChangeCommitted += new System.EventHandler(this.m_cbShortcutKeys_SelectionChangeCommitted);
            // 
            // m_CheckBoxListView
            // 
            this.m_CheckBoxListView.CheckBoxes = true;
            resources.ApplyResources(this.m_CheckBoxListView, "m_CheckBoxListView");
            this.m_CheckBoxListView.Name = "m_CheckBoxListView";
            this.m_CheckBoxListView.UseCompatibleStateImageBehavior = false;
            this.m_CheckBoxListView.View = System.Windows.Forms.View.List;
            this.m_CheckBoxListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.m_CheckBoxListView_ItemChecked);
            // 
            // Preferences
            // 
            this.AcceptButton = this.mOKButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.mCancelButton;
            this.Controls.Add(this.m_CheckBoxListView);
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
            ((System.ComponentModel.ISupportInitialize)(this.m_OperationDurationUpDown)).EndInit();
            this.mUserProfileTab.ResumeLayout(false);
            this.mUserProfileTab.PerformLayout();
            this.mKeyboardShortcutTab.ResumeLayout(false);
            this.mKeyboardShortcutTab.PerformLayout();
            this.m_grpKeyboardShortcutList.ResumeLayout(false);
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
        private System.Windows.Forms.TabPage mUserProfileTab;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox mFullNameTextbox;
        private System.Windows.Forms.TextBox mOrganizationTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox mCultureBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox mAudioCluesCheckBox;
        private System.Windows.Forms.ComboBox mNoiseLevelComboBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button mPipelineBrowseButton;
        private System.Windows.Forms.TextBox mPipelineTextbox;
        private System.Windows.Forms.NumericUpDown MnumAutoSaveInterval;
        private System.Windows.Forms.CheckBox mChkAutoSaveOnRecordingEnd;
        private System.Windows.Forms.CheckBox m_ChkAutoSaveInterval;
        private System.Windows.Forms.TabPage mKeyboardShortcutTab;
        private System.Windows.Forms.ComboBox m_cbShortcutKeys;
        private System.Windows.Forms.ListView m_lvShortcutKeysList;
        private System.Windows.Forms.Button m_btnRemove;
        private System.Windows.Forms.Button m_btnAssign;
        private System.Windows.Forms.TextBox m_txtShortcutKeys;
        private System.Windows.Forms.Label m_lblShortcutKeys;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button m_RestoreDefaults;
        private System.Windows.Forms.GroupBox m_grpKeyboardShortcutList;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown m_OperationDurationUpDown;
        private System.Windows.Forms.ComboBox m_cbOperation;
        private System.Windows.Forms.Label mlbOperation;
        private System.Windows.Forms.ListView m_CheckBoxListView;
    }
}