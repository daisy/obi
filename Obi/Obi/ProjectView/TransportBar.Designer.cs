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
            this.mPlayButton = new System.Windows.Forms.Button();
            this.mPauseButton = new System.Windows.Forms.Button();
            this.mStopButton = new System.Windows.Forms.Button();
            this.mNextPhrase = new System.Windows.Forms.Button();
            this.mPrevPhraseButton = new System.Windows.Forms.Button();
            this.mNextSectionButton = new System.Windows.Forms.Button();
            this.mPrevSectionButton = new System.Windows.Forms.Button();
            this.mRecordButton = new System.Windows.Forms.Button();
            this.mTimeDisplayBox = new System.Windows.Forms.TextBox();
            this.mTransportBarTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.mRewindButton = new System.Windows.Forms.Button();
            this.mFastForwardButton = new System.Windows.Forms.Button();
            this.mNextPageButton = new System.Windows.Forms.Button();
            this.mPreviousPageButton = new System.Windows.Forms.Button();
            this.mToDo_CustomClassMarkButton = new System.Windows.Forms.Button();
            this.mDisplayTimer = new System.Windows.Forms.Timer(this.components);
            this.mDisplayBox = new System.Windows.Forms.ComboBox();
            this.mFastPlayRateCombobox = new System.Windows.Forms.ComboBox();
            this.mVUMeterPanel = new Obi.UserControls.TextVUMeterPanel();
            this.SuspendLayout();
            // 
            // mPlayButton
            // 
            this.mPlayButton.AccessibleDescription = "button";
            this.mPlayButton.AccessibleName = "Play";
            this.mPlayButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mPlayButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mPlayButton.FlatAppearance.BorderSize = 0;
            this.mPlayButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPlayButton.Image = ((System.Drawing.Image)(resources.GetObject("mPlayButton.Image")));
            this.mPlayButton.Location = new System.Drawing.Point(143, 0);
            this.mPlayButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(32, 32);
            this.mPlayButton.TabIndex = 4;
            this.mTransportBarTooltip.SetToolTip(this.mPlayButton, "Start or resume audio playback (Space)");
            this.mPlayButton.UseVisualStyleBackColor = true;
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mPauseButton
            // 
            this.mPauseButton.AccessibleDescription = "button";
            this.mPauseButton.AccessibleName = "Pause";
            this.mPauseButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mPauseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mPauseButton.FlatAppearance.BorderSize = 0;
            this.mPauseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPauseButton.Image = ((System.Drawing.Image)(resources.GetObject("mPauseButton.Image")));
            this.mPauseButton.Location = new System.Drawing.Point(143, 0);
            this.mPauseButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mPauseButton.Name = "mPauseButton";
            this.mPauseButton.Size = new System.Drawing.Size(32, 32);
            this.mPauseButton.TabIndex = 5;
            this.mTransportBarTooltip.SetToolTip(this.mPauseButton, "Pause audio playback or recording (Space)");
            this.mPauseButton.UseVisualStyleBackColor = true;
            this.mPauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
            // 
            // mStopButton
            // 
            this.mStopButton.AccessibleDescription = "button";
            this.mStopButton.AccessibleName = "Stop";
            this.mStopButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mStopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mStopButton.FlatAppearance.BorderSize = 0;
            this.mStopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mStopButton.Image = ((System.Drawing.Image)(resources.GetObject("mStopButton.Image")));
            this.mStopButton.Location = new System.Drawing.Point(178, 0);
            this.mStopButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(32, 32);
            this.mStopButton.TabIndex = 6;
            this.mTransportBarTooltip.SetToolTip(this.mStopButton, "Stop audio playback or recording. (Ctrl+Space)");
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // mNextPhrase
            // 
            this.mNextPhrase.AccessibleDescription = "button";
            this.mNextPhrase.AccessibleName = "Next phrase";
            this.mNextPhrase.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mNextPhrase.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mNextPhrase.FlatAppearance.BorderSize = 0;
            this.mNextPhrase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mNextPhrase.Image = ((System.Drawing.Image)(resources.GetObject("mNextPhrase.Image")));
            this.mNextPhrase.Location = new System.Drawing.Point(283, 0);
            this.mNextPhrase.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mNextPhrase.Name = "mNextPhrase";
            this.mNextPhrase.Size = new System.Drawing.Size(32, 32);
            this.mNextPhrase.TabIndex = 9;
            this.mTransportBarTooltip.SetToolTip(this.mNextPhrase, "Go to the following phrase. (K)");
            this.mNextPhrase.UseVisualStyleBackColor = true;
            this.mNextPhrase.Click += new System.EventHandler(this.mNextPhrase_Click);
            // 
            // mPrevPhraseButton
            // 
            this.mPrevPhraseButton.AccessibleDescription = "button";
            this.mPrevPhraseButton.AccessibleName = "Previous phrase";
            this.mPrevPhraseButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mPrevPhraseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mPrevPhraseButton.FlatAppearance.BorderSize = 0;
            this.mPrevPhraseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPrevPhraseButton.Image = ((System.Drawing.Image)(resources.GetObject("mPrevPhraseButton.Image")));
            this.mPrevPhraseButton.Location = new System.Drawing.Point(73, 0);
            this.mPrevPhraseButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mPrevPhraseButton.Name = "mPrevPhraseButton";
            this.mPrevPhraseButton.Size = new System.Drawing.Size(32, 32);
            this.mPrevPhraseButton.TabIndex = 2;
            this.mTransportBarTooltip.SetToolTip(this.mPrevPhraseButton, "Go to the preceding phrase. (J)");
            this.mPrevPhraseButton.UseVisualStyleBackColor = true;
            this.mPrevPhraseButton.Click += new System.EventHandler(this.mPrevPhraseButton_Click);
            // 
            // mNextSectionButton
            // 
            this.mNextSectionButton.AccessibleDescription = "button";
            this.mNextSectionButton.AccessibleName = "Next section";
            this.mNextSectionButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mNextSectionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mNextSectionButton.FlatAppearance.BorderSize = 0;
            this.mNextSectionButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mNextSectionButton.Image = ((System.Drawing.Image)(resources.GetObject("mNextSectionButton.Image")));
            this.mNextSectionButton.Location = new System.Drawing.Point(353, 0);
            this.mNextSectionButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mNextSectionButton.Name = "mNextSectionButton";
            this.mNextSectionButton.Size = new System.Drawing.Size(32, 32);
            this.mNextSectionButton.TabIndex = 11;
            this.mTransportBarTooltip.SetToolTip(this.mNextSectionButton, "Go to the following section. (H)");
            this.mNextSectionButton.UseVisualStyleBackColor = true;
            this.mNextSectionButton.Click += new System.EventHandler(this.mNextSectionButton_Click);
            // 
            // mPrevSectionButton
            // 
            this.mPrevSectionButton.AccessibleDescription = "button";
            this.mPrevSectionButton.AccessibleName = "Previous section";
            this.mPrevSectionButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mPrevSectionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mPrevSectionButton.FlatAppearance.BorderSize = 0;
            this.mPrevSectionButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPrevSectionButton.Image = ((System.Drawing.Image)(resources.GetObject("mPrevSectionButton.Image")));
            this.mPrevSectionButton.Location = new System.Drawing.Point(3, 0);
            this.mPrevSectionButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mPrevSectionButton.Name = "mPrevSectionButton";
            this.mPrevSectionButton.Size = new System.Drawing.Size(32, 32);
            this.mPrevSectionButton.TabIndex = 0;
            this.mTransportBarTooltip.SetToolTip(this.mPrevSectionButton, "Go to the preceding section. (Shift+H)");
            this.mPrevSectionButton.UseVisualStyleBackColor = true;
            this.mPrevSectionButton.Click += new System.EventHandler(this.mPrevSectionButton_Click);
            // 
            // mRecordButton
            // 
            this.mRecordButton.AccessibleDescription = "button";
            this.mRecordButton.AccessibleName = "Record";
            this.mRecordButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mRecordButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mRecordButton.FlatAppearance.BorderSize = 0;
            this.mRecordButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mRecordButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mRecordButton.Image = ((System.Drawing.Image)(resources.GetObject("mRecordButton.Image")));
            this.mRecordButton.Location = new System.Drawing.Point(213, 0);
            this.mRecordButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mRecordButton.Name = "mRecordButton";
            this.mRecordButton.Size = new System.Drawing.Size(32, 32);
            this.mRecordButton.TabIndex = 7;
            this.mTransportBarTooltip.SetToolTip(this.mRecordButton, "Start monitoring or recording. (Ctrl+R)");
            this.mRecordButton.UseVisualStyleBackColor = true;
            this.mRecordButton.Click += new System.EventHandler(this.mRecordButton_Click);
            // 
            // mTimeDisplayBox
            // 
            this.mTimeDisplayBox.BackColor = System.Drawing.Color.MediumSlateBlue;
            this.mTimeDisplayBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mTimeDisplayBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTimeDisplayBox.Location = new System.Drawing.Point(429, 0);
            this.mTimeDisplayBox.Name = "mTimeDisplayBox";
            this.mTimeDisplayBox.ReadOnly = true;
            this.mTimeDisplayBox.Size = new System.Drawing.Size(113, 33);
            this.mTimeDisplayBox.TabIndex = 13;
            this.mTimeDisplayBox.Text = "00:00:00";
            this.mTimeDisplayBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // mTransportBarTooltip
            // 
            this.mTransportBarTooltip.IsBalloon = true;
            this.mTransportBarTooltip.ToolTipTitle = "Transport bar";
            // 
            // mRewindButton
            // 
            this.mRewindButton.AccessibleDescription = "button";
            this.mRewindButton.AccessibleName = "Rewind";
            this.mRewindButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mRewindButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mRewindButton.FlatAppearance.BorderSize = 0;
            this.mRewindButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mRewindButton.Image = ((System.Drawing.Image)(resources.GetObject("mRewindButton.Image")));
            this.mRewindButton.Location = new System.Drawing.Point(108, 0);
            this.mRewindButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mRewindButton.Name = "mRewindButton";
            this.mRewindButton.Size = new System.Drawing.Size(32, 32);
            this.mRewindButton.TabIndex = 3;
            this.mTransportBarTooltip.SetToolTip(this.mRewindButton, "Play backward at faster speed.");
            this.mRewindButton.UseVisualStyleBackColor = true;
            this.mRewindButton.Click += new System.EventHandler(this.mRewindButton_Click);
            // 
            // mFastForwardButton
            // 
            this.mFastForwardButton.AccessibleDescription = "button";
            this.mFastForwardButton.AccessibleName = "Fast forward";
            this.mFastForwardButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mFastForwardButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mFastForwardButton.FlatAppearance.BorderSize = 0;
            this.mFastForwardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mFastForwardButton.Image = ((System.Drawing.Image)(resources.GetObject("mFastForwardButton.Image")));
            this.mFastForwardButton.Location = new System.Drawing.Point(248, 0);
            this.mFastForwardButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mFastForwardButton.Name = "mFastForwardButton";
            this.mFastForwardButton.Size = new System.Drawing.Size(32, 32);
            this.mFastForwardButton.TabIndex = 8;
            this.mTransportBarTooltip.SetToolTip(this.mFastForwardButton, "Play forward at faster speed.");
            this.mFastForwardButton.UseVisualStyleBackColor = true;
            this.mFastForwardButton.Click += new System.EventHandler(this.mFastForwardButton_Click);
            // 
            // mNextPageButton
            // 
            this.mNextPageButton.AccessibleDescription = "button";
            this.mNextPageButton.AccessibleName = "Next page";
            this.mNextPageButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mNextPageButton.FlatAppearance.BorderSize = 0;
            this.mNextPageButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mNextPageButton.Image = ((System.Drawing.Image)(resources.GetObject("mNextPageButton.Image")));
            this.mNextPageButton.Location = new System.Drawing.Point(318, 0);
            this.mNextPageButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mNextPageButton.Name = "mNextPageButton";
            this.mNextPageButton.Size = new System.Drawing.Size(32, 32);
            this.mNextPageButton.TabIndex = 10;
            this.mTransportBarTooltip.SetToolTip(this.mNextPageButton, "Go to the following page. (P)");
            this.mNextPageButton.UseVisualStyleBackColor = true;
            this.mNextPageButton.Click += new System.EventHandler(this.mNextPageButton_Click);
            // 
            // mPreviousPageButton
            // 
            this.mPreviousPageButton.AccessibleDescription = "button";
            this.mPreviousPageButton.AccessibleName = "Previous page";
            this.mPreviousPageButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mPreviousPageButton.FlatAppearance.BorderSize = 0;
            this.mPreviousPageButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPreviousPageButton.Image = ((System.Drawing.Image)(resources.GetObject("mPreviousPageButton.Image")));
            this.mPreviousPageButton.Location = new System.Drawing.Point(38, 0);
            this.mPreviousPageButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mPreviousPageButton.Name = "mPreviousPageButton";
            this.mPreviousPageButton.Size = new System.Drawing.Size(32, 32);
            this.mPreviousPageButton.TabIndex = 1;
            this.mTransportBarTooltip.SetToolTip(this.mPreviousPageButton, "Go to the preceding page. (Shift+P)");
            this.mPreviousPageButton.UseVisualStyleBackColor = true;
            this.mPreviousPageButton.Click += new System.EventHandler(this.mPreviousPageButton_Click);
            // 
            // mToDo_CustomClassMarkButton
            // 
            this.mToDo_CustomClassMarkButton.AccessibleDescription = "button";
            this.mToDo_CustomClassMarkButton.AccessibleName = "Mark phrase";
            this.mToDo_CustomClassMarkButton.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.mToDo_CustomClassMarkButton.FlatAppearance.BorderSize = 0;
            this.mToDo_CustomClassMarkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mToDo_CustomClassMarkButton.Image = ((System.Drawing.Image)(resources.GetObject("mToDo_CustomClassMarkButton.Image")));
            this.mToDo_CustomClassMarkButton.Location = new System.Drawing.Point(391, 0);
            this.mToDo_CustomClassMarkButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.mToDo_CustomClassMarkButton.Name = "mToDo_CustomClassMarkButton";
            this.mToDo_CustomClassMarkButton.Size = new System.Drawing.Size(32, 32);
            this.mToDo_CustomClassMarkButton.TabIndex = 12;
            this.mTransportBarTooltip.SetToolTip(this.mToDo_CustomClassMarkButton, "Add a TODO mark while recording. (F9)");
            this.mToDo_CustomClassMarkButton.UseVisualStyleBackColor = true;
            this.mToDo_CustomClassMarkButton.Click += new System.EventHandler(this.mToDoMarkButton_Click);
            // 
            // mDisplayTimer
            // 
            this.mDisplayTimer.Tick += new System.EventHandler(this.mDisplayTimer_Tick);
            // 
            // mDisplayBox
            // 
            this.mDisplayBox.AllowDrop = true;
            this.mDisplayBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mDisplayBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mDisplayBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDisplayBox.FormattingEnabled = true;
            this.mDisplayBox.Items.AddRange(new object[] {
            global::Obi.messages.phrase_extra_Plain});
            this.mDisplayBox.Location = new System.Drawing.Point(548, 5);
            this.mDisplayBox.Name = "mDisplayBox";
            this.mDisplayBox.Size = new System.Drawing.Size(116, 24);
            this.mDisplayBox.TabIndex = 14;
            this.mDisplayBox.SelectionChangeCommitted += new System.EventHandler(this.mDisplayBox_SelectionChangeCommitted);
            this.mDisplayBox.SelectedIndexChanged += new System.EventHandler(this.mDisplayBox_SelectedIndexChanged);
            // 
            // mFastPlayRateCombobox
            // 
            this.mFastPlayRateCombobox.AccessibleName = "Fast Play Rate";
            this.mFastPlayRateCombobox.AllowDrop = true;
            this.mFastPlayRateCombobox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mFastPlayRateCombobox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mFastPlayRateCombobox.FormattingEnabled = true;
            this.mFastPlayRateCombobox.ItemHeight = 16;
            this.mFastPlayRateCombobox.Items.AddRange(new object[] {
            "1",
            "1.125",
            "1.25",
            "1.5",
            "1.75",
            "2"});
            this.mFastPlayRateCombobox.Location = new System.Drawing.Point(670, 5);
            this.mFastPlayRateCombobox.Name = "mFastPlayRateCombobox";
            this.mFastPlayRateCombobox.Size = new System.Drawing.Size(62, 24);
            this.mFastPlayRateCombobox.TabIndex = 15;
            this.mFastPlayRateCombobox.SelectionChangeCommitted += new System.EventHandler(this.mFastPlayRateComboBox_SelectionChangeCommitted);
            // 
            // mVUMeterPanel
            // 
            this.mVUMeterPanel.BackColor = System.Drawing.Color.Transparent;
            this.mVUMeterPanel.BeepEnable = false;
            this.mVUMeterPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mVUMeterPanel.Location = new System.Drawing.Point(740, 2);
            this.mVUMeterPanel.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.mVUMeterPanel.Name = "mVUMeterPanel";
            this.mVUMeterPanel.ShowMaxMinValues = false;
            this.mVUMeterPanel.Size = new System.Drawing.Size(208, 28);
            this.mVUMeterPanel.TabIndex = 16;
            this.mVUMeterPanel.VuMeter = null;
            // 
            // TransportBar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.LightSalmon;
            this.Controls.Add(this.mToDo_CustomClassMarkButton);
            this.Controls.Add(this.mPreviousPageButton);
            this.Controls.Add(this.mNextPageButton);
            this.Controls.Add(this.mFastPlayRateCombobox);
            this.Controls.Add(this.mFastForwardButton);
            this.Controls.Add(this.mRewindButton);
            this.Controls.Add(this.mVUMeterPanel);
            this.Controls.Add(this.mDisplayBox);
            this.Controls.Add(this.mTimeDisplayBox);
            this.Controls.Add(this.mRecordButton);
            this.Controls.Add(this.mPrevSectionButton);
            this.Controls.Add(this.mNextSectionButton);
            this.Controls.Add(this.mPrevPhraseButton);
            this.Controls.Add(this.mNextPhrase);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mPlayButton);
            this.Controls.Add(this.mPauseButton);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TransportBar";
            this.Size = new System.Drawing.Size(948, 32);
            this.Leave += new System.EventHandler(this.TransportBar_Leave);
            this.Enter += new System.EventHandler(this.TransportBar_Enter);
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
    }
}
