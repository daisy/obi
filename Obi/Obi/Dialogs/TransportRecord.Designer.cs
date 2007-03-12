namespace Obi.Dialogs
{
    partial class TransportRecord
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransportRecord));
            this.mRecordButton = new System.Windows.Forms.Button();
            this.mPauseButton = new System.Windows.Forms.Button();
            this.mStopButton = new System.Windows.Forms.Button();
            this.mPhraseMarkButton = new System.Windows.Forms.Button();
            this.mBeginSectionButton = new System.Windows.Forms.Button();
            this.mPageMarkButton = new System.Windows.Forms.Button();
            this.mTimeDisplay = new System.Windows.Forms.Timer(this.components);
            this.mTimeDisplayBox = new System.Windows.Forms.TextBox();
            this.mTextVuMeter = new Obi.UserControls.TextVUMeterPanel();
            this.SuspendLayout();
            // 
            // mRecordButton
            // 
            this.mRecordButton.Location = new System.Drawing.Point(12, 12);
            this.mRecordButton.Name = "mRecordButton";
            this.mRecordButton.Size = new System.Drawing.Size(75, 25);
            this.mRecordButton.TabIndex = 0;
            this.mRecordButton.Text = "&Record";
            this.mRecordButton.UseVisualStyleBackColor = true;
            this.mRecordButton.Click += new System.EventHandler(this.mRecordButton_Click);
            // 
            // mPauseButton
            // 
            this.mPauseButton.Location = new System.Drawing.Point(12, 12);
            this.mPauseButton.Name = "mPauseButton";
            this.mPauseButton.Size = new System.Drawing.Size(75, 25);
            this.mPauseButton.TabIndex = 1;
            this.mPauseButton.Text = "&Pause";
            this.mPauseButton.UseVisualStyleBackColor = true;
            this.mPauseButton.Click += new System.EventHandler(this.mPauseButton_Click);
            // 
            // mStopButton
            // 
            this.mStopButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mStopButton.Location = new System.Drawing.Point(195, 43);
            this.mStopButton.Name = "mStopButton";
            this.mStopButton.Size = new System.Drawing.Size(75, 25);
            this.mStopButton.TabIndex = 3;
            this.mStopButton.Text = "&Stop/Close";
            this.mStopButton.UseVisualStyleBackColor = true;
            this.mStopButton.Click += new System.EventHandler(this.mStopButton_Click);
            // 
            // mPhraseMarkButton
            // 
            this.mPhraseMarkButton.Location = new System.Drawing.Point(12, 43);
            this.mPhraseMarkButton.Name = "mPhraseMarkButton";
            this.mPhraseMarkButton.Size = new System.Drawing.Size(75, 23);
            this.mPhraseMarkButton.TabIndex = 4;
            this.mPhraseMarkButton.Text = "Phrase &mark";
            this.mPhraseMarkButton.UseVisualStyleBackColor = true;
            this.mPhraseMarkButton.Click += new System.EventHandler(this.btnPhraseMark_Click);
            // 
            // mBeginSectionButton
            // 
            this.mBeginSectionButton.Location = new System.Drawing.Point(105, 43);
            this.mBeginSectionButton.Name = "mBeginSectionButton";
            this.mBeginSectionButton.Size = new System.Drawing.Size(75, 23);
            this.mBeginSectionButton.TabIndex = 5;
            this.mBeginSectionButton.Text = "Begin Se&ction";
            this.mBeginSectionButton.UseVisualStyleBackColor = true;
            this.mBeginSectionButton.Click += new System.EventHandler(this.btnBeginSection_Click);
            // 
            // mPageMarkButton
            // 
            this.mPageMarkButton.Location = new System.Drawing.Point(195, 12);
            this.mPageMarkButton.Name = "mPageMarkButton";
            this.mPageMarkButton.Size = new System.Drawing.Size(75, 23);
            this.mPageMarkButton.TabIndex = 6;
            this.mPageMarkButton.Text = "P&age Mark";
            this.mPageMarkButton.UseVisualStyleBackColor = true;
            this.mPageMarkButton.Click += new System.EventHandler(this.mPageMarkButton_Click);
            // 
            // mTimeDisplay
            // 
            this.mTimeDisplay.Interval = 333;
            this.mTimeDisplay.Tick += new System.EventHandler(this.tmDisplayTime_Tick);
            // 
            // mTimeDisplayBox
            // 
            this.mTimeDisplayBox.AccessibleName = "Record Time:";
            this.mTimeDisplayBox.Location = new System.Drawing.Point(105, 12);
            this.mTimeDisplayBox.Name = "mTimeDisplayBox";
            this.mTimeDisplayBox.ReadOnly = true;
            this.mTimeDisplayBox.Size = new System.Drawing.Size(75, 20);
            this.mTimeDisplayBox.TabIndex = 8;
            // 
            // mTextVuMeter
            // 
            this.mTextVuMeter.BackColor = System.Drawing.Color.Transparent;
            this.mTextVuMeter.Location = new System.Drawing.Point(12, 72);
            this.mTextVuMeter.Name = "mTextVuMeter";
            this.mTextVuMeter.PlayListObj = null;
            this.mTextVuMeter.RecordingSessionObj = null;
            this.mTextVuMeter.Size = new System.Drawing.Size(205, 26);
            this.mTextVuMeter.TabIndex = 7;
            // 
            // TransportRecord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 111);
            this.Controls.Add(this.mTimeDisplayBox);
            this.Controls.Add(this.mTextVuMeter);
            this.Controls.Add(this.mPageMarkButton);
            this.Controls.Add(this.mBeginSectionButton);
            this.Controls.Add(this.mPhraseMarkButton);
            this.Controls.Add(this.mStopButton);
            this.Controls.Add(this.mPauseButton);
            this.Controls.Add(this.mRecordButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TransportRecord";
            this.Text = "Record";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TransportRecord_FormClosing);
            this.Load += new System.EventHandler(this.TransportRecord_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button mRecordButton;
        private System.Windows.Forms.Button mPauseButton;
        private System.Windows.Forms.Button mStopButton;
        private System.Windows.Forms.Button mPhraseMarkButton;
        private System.Windows.Forms.Button mBeginSectionButton;
        private System.Windows.Forms.Button mPageMarkButton;
        private Obi.UserControls.TextVUMeterPanel mTextVuMeter;
        private System.Windows.Forms.TextBox mTimeDisplayBox;
        private System.Windows.Forms.Timer mTimeDisplay;
    }
}