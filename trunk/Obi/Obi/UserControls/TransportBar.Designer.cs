namespace Obi.UserControls
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
            this.mDisplayTimer = new System.Windows.Forms.Timer(this.components);
            this.mDisplayBox = new System.Windows.Forms.ComboBox();
            this.mVUMeterPanel = new Obi.UserControls.TextVUMeterPanel();
            this.SuspendLayout();
            // 
            // mPlayButton
            // 
            this.mPlayButton.AccessibleName = "Play";
            this.mPlayButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mPlayButton.FlatAppearance.BorderSize = 0;
            this.mPlayButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPlayButton.Image = ((System.Drawing.Image)(resources.GetObject("mPlayButton.Image")));
            this.mPlayButton.Location = new System.Drawing.Point(114, 0);
            this.mPlayButton.Name = "mPlayButton";
            this.mPlayButton.Size = new System.Drawing.Size(32, 35);
            this.mPlayButton.TabIndex = 0;
            this.mTransportBarTooltip.SetToolTip(this.mPlayButton, "Start or resume audio playback or recording.");
            this.mPlayButton.UseVisualStyleBackColor = true;
            this.mPlayButton.Click += new System.EventHandler(this.mPlayButton_Click);
            // 
            // mPauseButton
            // 
            this.mPauseButton.AccessibleName = "Pause";
            this.mPauseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mPauseButton.FlatAppearance.BorderSize = 0;
            this.mPauseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPauseButton.Image = ((System.Drawing.Image)(resources.GetObject("mPauseButton.Image")));
            this.mPauseButton.Location = new System.Drawing.Point(114, 0);
            this.mPauseButton.Name = "mPauseButton";
            this.mPauseButton.Size = new System.Drawing.Size(32, 35);
            this.mPauseButton.TabIndex = 1;
            this.mTransportBarTooltip.SetToolTip(this.mPauseButton, "Pause audio playback or recording.");
            this.mPauseButton.UseVisualStyleBackColor = true;
            this.mPauseButton.Visible = false;
            this.mPauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
            // 
            // mStopButton
            // 
            this.mStopButton.AccessibleName = "Stop";
            this.mStopButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mStopButton.FlatAppearance.BorderSize = 0;
            this.mStopButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mStopButton.Image = ((System.Drawing.Image)(resources.GetObject("mStopButton.Image")));
            this.mStopButton.Location = new System.Drawing.Point(190, 0);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(32, 35);
            this.mStopButton.TabIndex = 2;
            this.mTransportBarTooltip.SetToolTip(this.mStopButton, "Stop audio playback or recording.");
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // mNextPhrase
            // 
            this.mNextPhrase.AccessibleName = "Next phrase";
            this.mNextPhrase.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mNextPhrase.FlatAppearance.BorderSize = 0;
            this.mNextPhrase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mNextPhrase.Image = ((System.Drawing.Image)(resources.GetObject("mNextPhrase.Image")));
            this.mNextPhrase.Location = new System.Drawing.Point(266, 0);
            this.mNextPhrase.Name = "mNextPhrase";
            this.mNextPhrase.Size = new System.Drawing.Size(32, 35);
            this.mNextPhrase.TabIndex = 3;
            this.mTransportBarTooltip.SetToolTip(this.mNextPhrase, "Move to the next phrase.");
            this.mNextPhrase.UseVisualStyleBackColor = true;
            this.mNextPhrase.Click += new System.EventHandler(this.mNextPhrase_Click);
            // 
            // mPrevPhraseButton
            // 
            this.mPrevPhraseButton.AccessibleName = "Previous phrase";
            this.mPrevPhraseButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mPrevPhraseButton.FlatAppearance.BorderSize = 0;
            this.mPrevPhraseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPrevPhraseButton.Image = ((System.Drawing.Image)(resources.GetObject("mPrevPhraseButton.Image")));
            this.mPrevPhraseButton.Location = new System.Drawing.Point(38, 0);
            this.mPrevPhraseButton.Name = "mPrevPhraseButton";
            this.mPrevPhraseButton.Size = new System.Drawing.Size(32, 35);
            this.mPrevPhraseButton.TabIndex = 4;
            this.mTransportBarTooltip.SetToolTip(this.mPrevPhraseButton, "Move to the previous phrase.");
            this.mPrevPhraseButton.UseVisualStyleBackColor = true;
            this.mPrevPhraseButton.Click += new System.EventHandler(this.mPrevPhraseButton_Click);
            // 
            // mNextSectionButton
            // 
            this.mNextSectionButton.AccessibleName = "Next section";
            this.mNextSectionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mNextSectionButton.FlatAppearance.BorderSize = 0;
            this.mNextSectionButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mNextSectionButton.Image = ((System.Drawing.Image)(resources.GetObject("mNextSectionButton.Image")));
            this.mNextSectionButton.Location = new System.Drawing.Point(304, 0);
            this.mNextSectionButton.Name = "mNextSectionButton";
            this.mNextSectionButton.Size = new System.Drawing.Size(32, 35);
            this.mNextSectionButton.TabIndex = 5;
            this.mTransportBarTooltip.SetToolTip(this.mNextSectionButton, "Move to the next section.");
            this.mNextSectionButton.UseVisualStyleBackColor = true;
            this.mNextSectionButton.Click += new System.EventHandler(this.mNextSectionButton_Click);
            // 
            // mPrevSectionButton
            // 
            this.mPrevSectionButton.AccessibleName = "Previous section";
            this.mPrevSectionButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mPrevSectionButton.FlatAppearance.BorderSize = 0;
            this.mPrevSectionButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mPrevSectionButton.Image = ((System.Drawing.Image)(resources.GetObject("mPrevSectionButton.Image")));
            this.mPrevSectionButton.Location = new System.Drawing.Point(0, 0);
            this.mPrevSectionButton.Name = "mPrevSectionButton";
            this.mPrevSectionButton.Size = new System.Drawing.Size(32, 35);
            this.mPrevSectionButton.TabIndex = 6;
            this.mTransportBarTooltip.SetToolTip(this.mPrevSectionButton, "Move to the previous section.");
            this.mPrevSectionButton.UseVisualStyleBackColor = true;
            this.mPrevSectionButton.Click += new System.EventHandler(this.mPrevSectionButton_Click);
            // 
            // mRecordButton
            // 
            this.mRecordButton.AccessibleName = "Record";
            this.mRecordButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mRecordButton.FlatAppearance.BorderSize = 0;
            this.mRecordButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mRecordButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mRecordButton.Image = ((System.Drawing.Image)(resources.GetObject("mRecordButton.Image")));
            this.mRecordButton.Location = new System.Drawing.Point(152, 0);
            this.mRecordButton.Name = "mRecordButton";
            this.mRecordButton.Size = new System.Drawing.Size(32, 35);
            this.mRecordButton.TabIndex = 7;
            this.mTransportBarTooltip.SetToolTip(this.mRecordButton, "Start listening or recording.");
            this.mRecordButton.UseVisualStyleBackColor = true;
            this.mRecordButton.Click += new System.EventHandler(this.mRecordButton_Click);
            // 
            // mTimeDisplayBox
            // 
            this.mTimeDisplayBox.BackColor = System.Drawing.Color.Azure;
            this.mTimeDisplayBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mTimeDisplayBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mTimeDisplayBox.Location = new System.Drawing.Point(342, 0);
            this.mTimeDisplayBox.Name = "mTimeDisplayBox";
            this.mTimeDisplayBox.ReadOnly = true;
            this.mTimeDisplayBox.Size = new System.Drawing.Size(111, 33);
            this.mTimeDisplayBox.TabIndex = 9;
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
            this.mRewindButton.AccessibleName = "Rewind";
            this.mRewindButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mRewindButton.FlatAppearance.BorderSize = 0;
            this.mRewindButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mRewindButton.Image = ((System.Drawing.Image)(resources.GetObject("mRewindButton.Image")));
            this.mRewindButton.Location = new System.Drawing.Point(76, 0);
            this.mRewindButton.Name = "mRewindButton";
            this.mRewindButton.Size = new System.Drawing.Size(32, 35);
            this.mRewindButton.TabIndex = 12;
            this.mTransportBarTooltip.SetToolTip(this.mRewindButton, "Move to the next section.");
            this.mRewindButton.UseVisualStyleBackColor = true;
            this.mRewindButton.Click += new System.EventHandler(this.mRewindButton_Click);
            // 
            // mFastForwardButton
            // 
            this.mFastForwardButton.AccessibleName = "Fast Forward";
            this.mFastForwardButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.mFastForwardButton.FlatAppearance.BorderSize = 0;
            this.mFastForwardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.mFastForwardButton.Image = ((System.Drawing.Image)(resources.GetObject("mFastForwardButton.Image")));
            this.mFastForwardButton.Location = new System.Drawing.Point(228, 0);
            this.mFastForwardButton.Name = "mFastForwardButton";
            this.mFastForwardButton.Size = new System.Drawing.Size(32, 35);
            this.mFastForwardButton.TabIndex = 13;
            this.mTransportBarTooltip.SetToolTip(this.mFastForwardButton, "Move to the next section.");
            this.mFastForwardButton.UseVisualStyleBackColor = true;
            this.mFastForwardButton.Click += new System.EventHandler(this.mFastForwardButton_Click);
            // 
            // mDisplayTimer
            // 
            this.mDisplayTimer.Interval = 333;
            this.mDisplayTimer.Tick += new System.EventHandler(this.mDisplayTimer_Tick);
            // 
            // mDisplayBox
            // 
            this.mDisplayBox.AllowDrop = true;
            this.mDisplayBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mDisplayBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mDisplayBox.FormattingEnabled = true;
            this.mDisplayBox.Items.AddRange(new object[] {
            "elapsed",
            "elapsed (total)",
            "remaining",
            "remaining (total)"});
            this.mDisplayBox.Location = new System.Drawing.Point(459, 4);
            this.mDisplayBox.Name = "mDisplayBox";
            this.mDisplayBox.Size = new System.Drawing.Size(108, 23);
            this.mDisplayBox.TabIndex = 10;
            this.mDisplayBox.SelectionChangeCommitted += new System.EventHandler(this.mDisplayBox_SelectionChangeCommitted);
            // 
            // mVUMeterPanel
            // 
            this.mVUMeterPanel.BackColor = System.Drawing.Color.Transparent;
            this.mVUMeterPanel.Location = new System.Drawing.Point(573, 0);
            this.mVUMeterPanel.Name = "mVUMeterPanel";
            this.mVUMeterPanel.PlayListObj = null;
            this.mVUMeterPanel.RecordingSessionObj = null;
            this.mVUMeterPanel.Size = new System.Drawing.Size(204, 44);
            this.mVUMeterPanel.TabIndex = 11;
            // 
            // TransportBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
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
            this.Controls.Add(this.mPauseButton);
            this.Controls.Add(this.mPlayButton);
            this.Name = "TransportBar";
            this.Size = new System.Drawing.Size(785, 35);
            this.ParentChanged += new System.EventHandler(this.TransportBar_ParentChanged);
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
        private TextVUMeterPanel mVUMeterPanel;
        private System.Windows.Forms.Button mRewindButton;
        private System.Windows.Forms.Button mFastForwardButton;
    }
}
