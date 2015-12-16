namespace Obi.ProjectView
{
    partial class TransportBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransportBar));
            this.mTimeDisplayBox = new System.Windows.Forms.TextBox();
            this.mTransportBarTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.mPreviousPageButton = new System.Windows.Forms.Button();
            this.mRewindButton = new System.Windows.Forms.Button();
            this.mToDo_CustomClassMarkButton = new System.Windows.Forms.Button();
            this.mNextPageButton = new System.Windows.Forms.Button();
            this.mPlayButton = new System.Windows.Forms.Button();
            this.mPauseButton = new System.Windows.Forms.Button();
            this.mStopButton = new System.Windows.Forms.Button();
            this.mNextPhrase = new System.Windows.Forms.Button();
            this.mPrevPhraseButton = new System.Windows.Forms.Button();
            this.mNextSectionButton = new System.Windows.Forms.Button();
            this.mPrevSectionButton = new System.Windows.Forms.Button();
            this.mRecordButton = new System.Windows.Forms.Button();
            this.mFastForwardButton = new System.Windows.Forms.Button();
            this.m_btnRecordingOptions = new System.Windows.Forms.Button();
            this.m_btnPlayingOptions = new System.Windows.Forms.Button();
            this.m_btnSwitchProfile = new System.Windows.Forms.Button();
            this.mDisplayTimer = new System.Windows.Forms.Timer(this.components);
            this.mDisplayBox = new System.Windows.Forms.ComboBox();
            this.mFastPlayRateCombobox = new System.Windows.Forms.ComboBox();
            this.m_RecordingOptionsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.m_MonitoringtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_RecordingtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_DeletePhrasestoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mPreviewBeforeRecToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mMonitorContinuouslyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_PlayingOptionsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.m_PlayAlltoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_PlaySectiontoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_playHeadingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_PreviewFromtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_PreviewUptotoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.m_SwitchProfileContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mVUMeterPanel = new Obi.UserControls.TextVUMeterPanel();
            this.m_RecordingOptionsContextMenuStrip.SuspendLayout();
            this.m_PlayingOptionsContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mTimeDisplayBox
            // 
            this.mTimeDisplayBox.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.mTimeDisplayBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.mTimeDisplayBox, "mTimeDisplayBox");
            this.mTimeDisplayBox.Name = "mTimeDisplayBox";
            this.mTimeDisplayBox.ReadOnly = true;
            // 
            // mTransportBarTooltip
            // 
            this.mTransportBarTooltip.IsBalloon = true;
            this.mTransportBarTooltip.ToolTipTitle = "Transport bar";
            // 
            // mPreviousPageButton
            // 
            resources.ApplyResources(this.mPreviousPageButton, "mPreviousPageButton");
            this.mPreviousPageButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mPreviousPageButton.FlatAppearance.BorderSize = 0;
            this.mPreviousPageButton.Name = "mPreviousPageButton";
            this.mTransportBarTooltip.SetToolTip(this.mPreviousPageButton, resources.GetString("mPreviousPageButton.ToolTip"));
            this.mPreviousPageButton.UseVisualStyleBackColor = true;
            this.mPreviousPageButton.Click += new System.EventHandler(this.mPreviousPageButton_Click);
            // 
            // mRewindButton
            // 
            resources.ApplyResources(this.mRewindButton, "mRewindButton");
            this.mRewindButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mRewindButton.FlatAppearance.BorderSize = 0;
            this.mRewindButton.Name = "mRewindButton";
            this.mTransportBarTooltip.SetToolTip(this.mRewindButton, resources.GetString("mRewindButton.ToolTip"));
            this.mRewindButton.UseVisualStyleBackColor = true;
            this.mRewindButton.Click += new System.EventHandler(this.mRewindButton_Click);
            // 
            // mToDo_CustomClassMarkButton
            // 
            resources.ApplyResources(this.mToDo_CustomClassMarkButton, "mToDo_CustomClassMarkButton");
            this.mToDo_CustomClassMarkButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mToDo_CustomClassMarkButton.FlatAppearance.BorderSize = 0;
            this.mToDo_CustomClassMarkButton.Name = "mToDo_CustomClassMarkButton";
            this.mTransportBarTooltip.SetToolTip(this.mToDo_CustomClassMarkButton, resources.GetString("mToDo_CustomClassMarkButton.ToolTip"));
            this.mToDo_CustomClassMarkButton.UseVisualStyleBackColor = true;
            this.mToDo_CustomClassMarkButton.Click += new System.EventHandler(this.mToDoMarkButton_Click);
            // 
            // mNextPageButton
            // 
            resources.ApplyResources(this.mNextPageButton, "mNextPageButton");
            this.mNextPageButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mNextPageButton.FlatAppearance.BorderSize = 0;
            this.mNextPageButton.Name = "mNextPageButton";
            this.mTransportBarTooltip.SetToolTip(this.mNextPageButton, resources.GetString("mNextPageButton.ToolTip"));
            this.mNextPageButton.UseVisualStyleBackColor = true;
            this.mNextPageButton.Click += new System.EventHandler(this.mNextPageButton_Click);
            // 
            // mPlayButton
            // 
            resources.ApplyResources(this.mPlayButton, "mPlayButton");
            this.mPlayButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mPlayButton.FlatAppearance.BorderSize = 0;
            this.mPlayButton.Name = "mPlayButton";
            this.mTransportBarTooltip.SetToolTip(this.mPlayButton, resources.GetString("mPlayButton.ToolTip"));
            this.mPlayButton.UseVisualStyleBackColor = true;
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mPauseButton
            // 
            resources.ApplyResources(this.mPauseButton, "mPauseButton");
            this.mPauseButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mPauseButton.FlatAppearance.BorderSize = 0;
            this.mPauseButton.Name = "mPauseButton";
            this.mTransportBarTooltip.SetToolTip(this.mPauseButton, resources.GetString("mPauseButton.ToolTip"));
            this.mPauseButton.UseVisualStyleBackColor = true;
            this.mPauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
            // 
            // mStopButton
            // 
            resources.ApplyResources(this.mStopButton, "mStopButton");
            this.mStopButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mStopButton.FlatAppearance.BorderSize = 0;
            this.mStopButton.Name = "mStopButton";
            this.mTransportBarTooltip.SetToolTip(this.mStopButton, resources.GetString("mStopButton.ToolTip"));
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // mNextPhrase
            // 
            resources.ApplyResources(this.mNextPhrase, "mNextPhrase");
            this.mNextPhrase.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mNextPhrase.FlatAppearance.BorderSize = 0;
            this.mNextPhrase.Name = "mNextPhrase";
            this.mTransportBarTooltip.SetToolTip(this.mNextPhrase, resources.GetString("mNextPhrase.ToolTip"));
            this.mNextPhrase.UseVisualStyleBackColor = true;
            this.mNextPhrase.Click += new System.EventHandler(this.mNextPhrase_Click);
            // 
            // mPrevPhraseButton
            // 
            resources.ApplyResources(this.mPrevPhraseButton, "mPrevPhraseButton");
            this.mPrevPhraseButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mPrevPhraseButton.FlatAppearance.BorderSize = 0;
            this.mPrevPhraseButton.Name = "mPrevPhraseButton";
            this.mTransportBarTooltip.SetToolTip(this.mPrevPhraseButton, resources.GetString("mPrevPhraseButton.ToolTip"));
            this.mPrevPhraseButton.UseVisualStyleBackColor = true;
            this.mPrevPhraseButton.Click += new System.EventHandler(this.mPrevPhraseButton_Click);
            // 
            // mNextSectionButton
            // 
            resources.ApplyResources(this.mNextSectionButton, "mNextSectionButton");
            this.mNextSectionButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mNextSectionButton.FlatAppearance.BorderSize = 0;
            this.mNextSectionButton.Name = "mNextSectionButton";
            this.mTransportBarTooltip.SetToolTip(this.mNextSectionButton, resources.GetString("mNextSectionButton.ToolTip"));
            this.mNextSectionButton.UseVisualStyleBackColor = true;
            this.mNextSectionButton.Click += new System.EventHandler(this.mNextSectionButton_Click);
            // 
            // mPrevSectionButton
            // 
            resources.ApplyResources(this.mPrevSectionButton, "mPrevSectionButton");
            this.mPrevSectionButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mPrevSectionButton.FlatAppearance.BorderSize = 0;
            this.mPrevSectionButton.Name = "mPrevSectionButton";
            this.mTransportBarTooltip.SetToolTip(this.mPrevSectionButton, resources.GetString("mPrevSectionButton.ToolTip"));
            this.mPrevSectionButton.UseVisualStyleBackColor = true;
            this.mPrevSectionButton.Click += new System.EventHandler(this.mPrevSectionButton_Click);
            // 
            // mRecordButton
            // 
            resources.ApplyResources(this.mRecordButton, "mRecordButton");
            this.mRecordButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mRecordButton.FlatAppearance.BorderSize = 0;
            this.mRecordButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mRecordButton.Name = "mRecordButton";
            this.mTransportBarTooltip.SetToolTip(this.mRecordButton, resources.GetString("mRecordButton.ToolTip"));
            this.mRecordButton.UseVisualStyleBackColor = true;
            this.mRecordButton.Click += new System.EventHandler(this.mRecordButton_Click);
            // 
            // mFastForwardButton
            // 
            resources.ApplyResources(this.mFastForwardButton, "mFastForwardButton");
            this.mFastForwardButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mFastForwardButton.FlatAppearance.BorderSize = 0;
            this.mFastForwardButton.Name = "mFastForwardButton";
            this.mTransportBarTooltip.SetToolTip(this.mFastForwardButton, resources.GetString("mFastForwardButton.ToolTip"));
            this.mFastForwardButton.UseVisualStyleBackColor = true;
            this.mFastForwardButton.Click += new System.EventHandler(this.mFastForwardButton_Click);
            // 
            // m_btnRecordingOptions
            // 
            resources.ApplyResources(this.m_btnRecordingOptions, "m_btnRecordingOptions");
            this.m_btnRecordingOptions.Name = "m_btnRecordingOptions";
            this.mTransportBarTooltip.SetToolTip(this.m_btnRecordingOptions, resources.GetString("m_btnRecordingOptions.ToolTip"));
            this.m_btnRecordingOptions.UseVisualStyleBackColor = true;
            this.m_btnRecordingOptions.Click += new System.EventHandler(this.m_btnRecordingOptions_Click);
            // 
            // m_btnPlayingOptions
            // 
            resources.ApplyResources(this.m_btnPlayingOptions, "m_btnPlayingOptions");
            this.m_btnPlayingOptions.Name = "m_btnPlayingOptions";
            this.mTransportBarTooltip.SetToolTip(this.m_btnPlayingOptions, resources.GetString("m_btnPlayingOptions.ToolTip"));
            this.m_btnPlayingOptions.UseVisualStyleBackColor = true;
            this.m_btnPlayingOptions.Click += new System.EventHandler(this.m_btnPlayingOptions_Click);
            // 
            // m_btnSwitchProfile
            // 
            resources.ApplyResources(this.m_btnSwitchProfile, "m_btnSwitchProfile");
            this.m_btnSwitchProfile.Name = "m_btnSwitchProfile";
            this.mTransportBarTooltip.SetToolTip(this.m_btnSwitchProfile, resources.GetString("m_btnSwitchProfile.ToolTip"));
            this.m_btnSwitchProfile.UseVisualStyleBackColor = true;
            this.m_btnSwitchProfile.Click += new System.EventHandler(this.m_SwitchProfile_Click);
            // 
            // mDisplayTimer
            // 
            this.mDisplayTimer.Tick += new System.EventHandler(this.mDisplayTimer_Tick);
            // 
            // mDisplayBox
            // 
            this.mDisplayBox.AllowDrop = true;
            this.mDisplayBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.mDisplayBox, "mDisplayBox");
            this.mDisplayBox.FormattingEnabled = true;
            this.mDisplayBox.Items.AddRange(new object[] {
            resources.GetString("mDisplayBox.Items"),
            resources.GetString("mDisplayBox.Items1"),
            resources.GetString("mDisplayBox.Items2"),
            resources.GetString("mDisplayBox.Items3")});
            this.mDisplayBox.Name = "mDisplayBox";
            this.mDisplayBox.SelectionChangeCommitted += new System.EventHandler(this.mDisplayBox_SelectionChangeCommitted);
            this.mDisplayBox.SelectedIndexChanged += new System.EventHandler(this.mDisplayBox_SelectedIndexChanged);
            // 
            // mFastPlayRateCombobox
            // 
            resources.ApplyResources(this.mFastPlayRateCombobox, "mFastPlayRateCombobox");
            this.mFastPlayRateCombobox.AllowDrop = true;
            this.mFastPlayRateCombobox.FormattingEnabled = true;
            this.mFastPlayRateCombobox.Items.AddRange(new object[] {
            resources.GetString("mFastPlayRateCombobox.Items"),
            resources.GetString("mFastPlayRateCombobox.Items1"),
            resources.GetString("mFastPlayRateCombobox.Items2"),
            resources.GetString("mFastPlayRateCombobox.Items3"),
            resources.GetString("mFastPlayRateCombobox.Items4"),
            resources.GetString("mFastPlayRateCombobox.Items5"),
            resources.GetString("mFastPlayRateCombobox.Items6"),
            resources.GetString("mFastPlayRateCombobox.Items7")});
            this.mFastPlayRateCombobox.Name = "mFastPlayRateCombobox";
            this.mFastPlayRateCombobox.SelectionChangeCommitted += new System.EventHandler(this.mFastPlayRateComboBox_SelectionChangeCommitted);
            // 
            // m_RecordingOptionsContextMenuStrip
            // 
            this.m_RecordingOptionsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_MonitoringtoolStripMenuItem,
            this.m_RecordingtoolStripMenuItem,
            this.m_DeletePhrasestoolStripMenuItem,
            this.mPreviewBeforeRecToolStripMenuItem,
            this.mMonitorContinuouslyToolStripMenuItem});
            this.m_RecordingOptionsContextMenuStrip.Name = "m_RecordingOptionsContextMenuStrip";
            resources.ApplyResources(this.m_RecordingOptionsContextMenuStrip, "m_RecordingOptionsContextMenuStrip");
            this.m_RecordingOptionsContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.m_RecordingOptionsContextMenuStrip_Opening);
            // 
            // m_MonitoringtoolStripMenuItem
            // 
            this.m_MonitoringtoolStripMenuItem.Name = "m_MonitoringtoolStripMenuItem";
            resources.ApplyResources(this.m_MonitoringtoolStripMenuItem, "m_MonitoringtoolStripMenuItem");
            this.m_MonitoringtoolStripMenuItem.Click += new System.EventHandler(this.RecordingOptions_Monitoring_Click);
            // 
            // m_RecordingtoolStripMenuItem
            // 
            this.m_RecordingtoolStripMenuItem.Name = "m_RecordingtoolStripMenuItem";
            resources.ApplyResources(this.m_RecordingtoolStripMenuItem, "m_RecordingtoolStripMenuItem");
            this.m_RecordingtoolStripMenuItem.Click += new System.EventHandler(this.m_RecordingtoolStripMenuItem_Click);
            // 
            // m_DeletePhrasestoolStripMenuItem
            // 
            this.m_DeletePhrasestoolStripMenuItem.Name = "m_DeletePhrasestoolStripMenuItem";
            resources.ApplyResources(this.m_DeletePhrasestoolStripMenuItem, "m_DeletePhrasestoolStripMenuItem");
            this.m_DeletePhrasestoolStripMenuItem.Click += new System.EventHandler(this.RecordingOptions_RecordWithDeleteFollowing_Click);
            // 
            // mPreviewBeforeRecToolStripMenuItem
            // 
            this.mPreviewBeforeRecToolStripMenuItem.Name = "mPreviewBeforeRecToolStripMenuItem";
            resources.ApplyResources(this.mPreviewBeforeRecToolStripMenuItem, "mPreviewBeforeRecToolStripMenuItem");
            this.mPreviewBeforeRecToolStripMenuItem.Click += new System.EventHandler(this.mPreviewBeforeRecToolStripMenuItem_Click);
            // 
            // mMonitorContinuouslyToolStripMenuItem
            // 
            this.mMonitorContinuouslyToolStripMenuItem.CheckOnClick = true;
            this.mMonitorContinuouslyToolStripMenuItem.Name = "mMonitorContinuouslyToolStripMenuItem";
            resources.ApplyResources(this.mMonitorContinuouslyToolStripMenuItem, "mMonitorContinuouslyToolStripMenuItem");
            this.mMonitorContinuouslyToolStripMenuItem.Click += new System.EventHandler(this.mMonitorContinuouslyToolStripMenuItem_Click);
            // 
            // m_PlayingOptionsContextMenuStrip
            // 
            this.m_PlayingOptionsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_PlayAlltoolStripMenuItem,
            this.m_PlaySectiontoolStripMenuItem,
            this.m_playHeadingToolStripMenuItem,
            this.m_PreviewFromtoolStripMenuItem,
            this.m_PreviewUptotoolStripMenuItem});
            this.m_PlayingOptionsContextMenuStrip.Name = "m_PlayingOptionsContextMenuStrip";
            resources.ApplyResources(this.m_PlayingOptionsContextMenuStrip, "m_PlayingOptionsContextMenuStrip");
            this.m_PlayingOptionsContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.m_PlayingOptionsContextMenuStrip_Opening);
            // 
            // m_PlayAlltoolStripMenuItem
            // 
            this.m_PlayAlltoolStripMenuItem.Name = "m_PlayAlltoolStripMenuItem";
            resources.ApplyResources(this.m_PlayAlltoolStripMenuItem, "m_PlayAlltoolStripMenuItem");
            this.m_PlayAlltoolStripMenuItem.Click += new System.EventHandler(this.m_PlayAlltoolStripMenuItem_Click);
            // 
            // m_PlaySectiontoolStripMenuItem
            // 
            this.m_PlaySectiontoolStripMenuItem.Name = "m_PlaySectiontoolStripMenuItem";
            resources.ApplyResources(this.m_PlaySectiontoolStripMenuItem, "m_PlaySectiontoolStripMenuItem");
            this.m_PlaySectiontoolStripMenuItem.Click += new System.EventHandler(this.m_PlaySectiontoolStripMenuItem_Click);
            // 
            // m_playHeadingToolStripMenuItem
            // 
            this.m_playHeadingToolStripMenuItem.Name = "m_playHeadingToolStripMenuItem";
            resources.ApplyResources(this.m_playHeadingToolStripMenuItem, "m_playHeadingToolStripMenuItem");
            this.m_playHeadingToolStripMenuItem.Click += new System.EventHandler(this.m_playHeadingToolStripMenuItem_Click);
            // 
            // m_PreviewFromtoolStripMenuItem
            // 
            this.m_PreviewFromtoolStripMenuItem.Name = "m_PreviewFromtoolStripMenuItem";
            resources.ApplyResources(this.m_PreviewFromtoolStripMenuItem, "m_PreviewFromtoolStripMenuItem");
            this.m_PreviewFromtoolStripMenuItem.Click += new System.EventHandler(this.m_PreviewFromtoolStripMenuItem_Click);
            // 
            // m_PreviewUptotoolStripMenuItem
            // 
            this.m_PreviewUptotoolStripMenuItem.Name = "m_PreviewUptotoolStripMenuItem";
            resources.ApplyResources(this.m_PreviewUptotoolStripMenuItem, "m_PreviewUptotoolStripMenuItem");
            this.m_PreviewUptotoolStripMenuItem.Click += new System.EventHandler(this.m_PreviewUptotoolStripMenuItem_Click);
            // 
            // m_SwitchProfileContextMenuStrip
            // 
            this.m_SwitchProfileContextMenuStrip.Name = "m_SwitchProfileContextMenuStrip";
            resources.ApplyResources(this.m_SwitchProfileContextMenuStrip, "m_SwitchProfileContextMenuStrip");
            // 
            // mVUMeterPanel
            // 
            this.mVUMeterPanel.BackColor = System.Drawing.Color.Transparent;
            this.mVUMeterPanel.BeepEnable = false;
            resources.ApplyResources(this.mVUMeterPanel, "mVUMeterPanel");
            this.mVUMeterPanel.Name = "mVUMeterPanel";
            this.mVUMeterPanel.ShowMaxMinValues = false;
            this.mVUMeterPanel.VuMeter = null;
            // 
            // TransportBar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.LightSalmon;
            this.Controls.Add(this.mPreviousPageButton);
            this.Controls.Add(this.mRewindButton);
            this.Controls.Add(this.m_btnRecordingOptions);
            this.Controls.Add(this.m_btnPlayingOptions);
            this.Controls.Add(this.mPrevSectionButton);
            this.Controls.Add(this.mPrevPhraseButton);
            this.Controls.Add(this.mPlayButton);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mPauseButton);
            this.Controls.Add(this.mRecordButton);
            this.Controls.Add(this.mFastForwardButton);
            this.Controls.Add(this.mNextPageButton);
            this.Controls.Add(this.mNextPhrase);
            this.Controls.Add(this.mTimeDisplayBox);
            this.Controls.Add(this.mFastPlayRateCombobox);
            this.Controls.Add(this.mDisplayBox);
            this.Controls.Add(this.mVUMeterPanel);
            this.Controls.Add(this.m_btnSwitchProfile);
            this.Controls.Add(this.mToDo_CustomClassMarkButton);
            this.Controls.Add(this.mNextSectionButton);
            this.DoubleBuffered = true;
            resources.ApplyResources(this, "$this");
            this.Name = "TransportBar";
            this.Leave += new System.EventHandler(this.TransportBar_Leave);
            this.Enter += new System.EventHandler(this.TransportBar_Enter);
            this.m_RecordingOptionsContextMenuStrip.ResumeLayout(false);
            this.m_PlayingOptionsContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mPlayButton;
        private System.Windows.Forms.Button mPauseButton;
        private System.Windows.Forms.Button mStopButton;
        private System.Windows.Forms.Button mNextPhrase;
        private System.Windows.Forms.Button mPrevPhraseButton;
        private System.Windows.Forms.Button mNextSectionButton;
        private System.Windows.Forms.Button mPrevSectionButton;
        private System.Windows.Forms.Button mRecordButton;
        private System.Windows.Forms.TextBox mTimeDisplayBox;
        private System.Windows.Forms.ToolTip mTransportBarTooltip;
        private System.Windows.Forms.Timer mDisplayTimer;
        private System.Windows.Forms.ComboBox mDisplayBox;
        private Obi.UserControls.TextVUMeterPanel mVUMeterPanel;
        private System.Windows.Forms.Button mRewindButton;
        private System.Windows.Forms.Button mFastForwardButton;
        private System.Windows.Forms.ComboBox mFastPlayRateCombobox;
        private System.Windows.Forms.Button mNextPageButton;
        private System.Windows.Forms.Button mPreviousPageButton;
        private System.Windows.Forms.Button mToDo_CustomClassMarkButton;
        private System.Windows.Forms.Button m_btnRecordingOptions;
        private System.Windows.Forms.Button m_btnPlayingOptions;
        private System.Windows.Forms.ContextMenuStrip m_RecordingOptionsContextMenuStrip;
        private System.Windows.Forms.ContextMenuStrip m_PlayingOptionsContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem m_MonitoringtoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_DeletePhrasestoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_RecordingtoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mPreviewBeforeRecToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_PlaySectiontoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_PlayAlltoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_PreviewFromtoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_PreviewUptotoolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem m_playHeadingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mMonitorContinuouslyToolStripMenuItem;
        private System.Windows.Forms.Button m_btnSwitchProfile;
        private System.Windows.Forms.ContextMenuStrip m_SwitchProfileContextMenuStrip;
    }
}
